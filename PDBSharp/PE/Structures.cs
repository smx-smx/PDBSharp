#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Smx.PDBSharp.PE
{
	namespace IMAGE_DOS_HEADER {
		public class Data {
			public string signature = "MZ";
			public ushort lastsize;
			public ushort nblocks;
			public ushort nreloc;
			public ushort hdrsize;
			public ushort minalloc;
			public ushort maxalloc;
			public ushort ss;
			public ushort sb;
			public ushort checksum;
			public ushort ip;
			public ushort cs;
			public ushort relocpos;
			public ushort noverlay;
			public ushort[] reserved1 = new ushort[4];
			public ushort oem_id;
			public ushort oem_info;
			public ushort[] reserved2 = new ushort[10];
			public uint e_lfanew;
		}

		public class Serializer(SpanStream stream) {
			public Data Data { get; set; } = new Data();

			public Data Read() {
				var signature = stream.ReadString(2, Encoding.ASCII);
				if(signature != "MZ") throw new InvalidDataException();
				var lastsize = stream.ReadUInt16();
				var nblocks = stream.ReadUInt16();
				var nreloc = stream.ReadUInt16();
				var hdrsize = stream.ReadUInt16();
				var minalloc = stream.ReadUInt16();
				var maxalloc = stream.ReadUInt16();
				var ss = stream.ReadUInt16();
				var sb = stream.ReadUInt16();
				var checksum = stream.ReadUInt16();
				var ip = stream.ReadUInt16();
				var cs = stream.ReadUInt16();
				var relocpos = stream.ReadUInt16();
				var noverlay = stream.ReadUInt16();
				var reserved1 = Enumerable.Range(0, 4)
					.Select(_ => stream.ReadUInt16())
					.ToArray();
				var oem_id = stream.ReadUInt16();
				var oem_info = stream.ReadUInt16();
				var reserved2 = Enumerable.Range(0, 10)
					.Select(_ => stream.ReadUInt16())
					.ToArray();
				var e_lfanew = stream.ReadUInt32();

				Data = new Data {
					signature = signature ,
					lastsize = lastsize,
					nblocks = nblocks,
					nreloc = nreloc,
					hdrsize = hdrsize,
					minalloc = minalloc,
					maxalloc = maxalloc,
					ss = ss,
					sb = sb,
					checksum = checksum,
					ip = ip,
					cs = cs,
					relocpos = relocpos,
					noverlay = noverlay,
					reserved1 = reserved1,
					oem_id = oem_id,
					oem_info = oem_info,
					reserved2 = reserved2,
					e_lfanew = e_lfanew,
				};
				return Data;

			}
		}
	}

	namespace IMAGE_DEBUG_DIRECTORY {
		public enum ImageDebugType : uint {
			Unknown = 0,
			Coff = 1,
			CodeView = 2,
			Fpo = 3,
			Misc = 4,
			Exception = 5,
			Fixup = 6,
			Borland = 9
		}

		public class Data
		{
			public uint Characteristics;
			public uint TimeDateStamp;
			public ushort MajorVersion;
			public ushort MinorVersion;
			public ImageDebugType Type;
			public uint SizeOfData;
			public uint AddressOfRawData;
			public uint PointerToRawData;

			//

			public object? x_Data { get; set; }
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();

			public Data Read() {
				var Characteristics = stream.ReadUInt32();
				var TimeDateStamp = stream.ReadUInt32();
				var MajorVersion = stream.ReadUInt16();
				var MinorVersion = stream.ReadUInt16();
				var Type = stream.ReadEnum<ImageDebugType>();
				var SizeOfData = stream.ReadUInt32();
				var AddressOfRawData = stream.ReadUInt32();
				var PointerToRawData = stream.ReadUInt32();

				Data = new Data {
					Characteristics = Characteristics,
					TimeDateStamp = TimeDateStamp,
					MajorVersion = MajorVersion,
					MinorVersion = MinorVersion,
					Type = Type,
					SizeOfData = SizeOfData,
					AddressOfRawData = AddressOfRawData,
					PointerToRawData = PointerToRawData,
				};

				switch (Type) {
					case IMAGE_DEBUG_DIRECTORY.ImageDebugType.CodeView:
						Data.x_Data = stream.PerformAt(PointerToRawData, () => {
							return new RSDSI.Serializer(stream.SliceHere()).Read();
						});
						break;
				}

				return Data;
			}
		}
	}


	namespace IMAGE_FILE_HEADER {
		public class Data {
			public ushort Machine;
			public ushort NumberOfSections;
			public uint TimeDateStamp;
			public uint PointerToSymbolTable;
			public uint NumberOfSymbols;
			public ushort SizeOfOptionalHeader;
			public ushort Characteristics;
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();

			public Data Read() {
				var Machine = stream.ReadUInt16();
				var NumberOfSections = stream.ReadUInt16();
				var TimeDateStamp = stream.ReadUInt32();
				var PointerToSymbolTable = stream.ReadUInt32();
				var NumberOfSymbols = stream.ReadUInt32();
				var SizeOfOptionalHeader = stream.ReadUInt16();
				var Characteristics = stream.ReadUInt16();

				Data = new Data {
					Machine = Machine,
					NumberOfSections = NumberOfSections,
					TimeDateStamp = TimeDateStamp,
					PointerToSymbolTable = PointerToSymbolTable,
					NumberOfSymbols = NumberOfSymbols,
					SizeOfOptionalHeader = SizeOfOptionalHeader,
					Characteristics = Characteristics
				};
				return Data;
			}
		}
	}

	namespace IMAGE_OPTIONAL_HEADER {
		public class Data {
			public ushort Magic;
			public byte MajorLinkerVersion;
			public byte MinorLinkerVersion;
			public uint SizeOfCode;
			public uint SizeOfInitializedData;
			public uint SizeOfUninitializedData;
			public uint AddressOfEntryPoint;
			public uint BaseOfCode;
			public uint BaseOfData;
			public ulong ImageBase;
			public uint SectionAlignment;
			public uint FileAlignment;
			public ushort MajorOperatingSystemVersion;
			public ushort MinorOperatingSystemVersion;
			public ushort MajorImageVersiom;
			public ushort MinorImageVersiom;
			public ushort MajorSubsystemVersion;
			public ushort MinorSubsystemVersion;
			public uint Win32VersionValue;
			public uint SizeOfImage;
			public uint SizeOfHeaders;
			public uint CheckSum;
			public ushort Subsystem;
			public ushort DllCharacteristics;
			public ulong SizeOfStackReserve;
			public ulong SizeOfStackCommit;
			public ulong SizeOfHeapReserve;
			public ulong SizeOfHeapCommit;
			public uint LoaderFlags;
			public uint NumberOfRvaAndSizes;
			public IMAGE_DATA_DIRECTORY.Data[] DataDirectory = new IMAGE_DATA_DIRECTORY.Data[16];

			public IMAGE_DATA_DIRECTORY.Data GetDataDirectory(ImageDirectoryEntry type) {
				int index = (int)type;
				return DataDirectory[index];
			}
		}

		public enum ImageDirectoryEntry {
			Architecture = 7,
			BaseReloc = 5,
			BoundImport = 11,
			ComDescriptor = 14,
			Debug = 6,
			DelayImport = 13,
			Exception = 3,
			Export = 0,
			GlobalPtr = 8,
			Iat = 12,
			Import = 1,
			LoadConfig = 10,
			Resource = 2,
			Security = 4,
			Tls = 9
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();


			public const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10B;
			public const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20B;
			public const ushort IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x107;

			private delegate ulong WordReader();

			public Data Read() {
				var Magic = stream.ReadUInt16();
				switch (Magic) {
					case IMAGE_NT_OPTIONAL_HDR32_MAGIC:
					case IMAGE_NT_OPTIONAL_HDR64_MAGIC:
					case IMAGE_ROM_OPTIONAL_HDR_MAGIC:
						break;
					default:
						throw new InvalidDataException();
				}

				WordReader ReadWord;
				if (Magic == IMAGE_NT_OPTIONAL_HDR64_MAGIC) {
					ReadWord = () => stream.ReadUInt64();
				} else {
					ReadWord = () => stream.ReadUInt32();
				}

				var MajorLinkerVersion = stream.ReadByte();
				var MinorLinkerVersion = stream.ReadByte();
				var SizeOfCode = stream.ReadUInt32();
				var SizeOfInitializedData = stream.ReadUInt32();
				var SizeOfUninitializedData = stream.ReadUInt32();
				var AddressOfEntryPoint = stream.ReadUInt32();
				var BaseOfCode = stream.ReadUInt32();
				var BaseOfData = (Magic == IMAGE_NT_OPTIONAL_HDR32_MAGIC)
					? stream.ReadUInt32()
					: 0;
				var ImageBase = ReadWord();
				var SectionAlignment = stream.ReadUInt32();
				var FileAlignment = stream.ReadUInt32();
				var MajorOperatingSystemVersion = stream.ReadUInt16();
				var MinorOperatingSystemVersion = stream.ReadUInt16();
				var MajorImageVersiom = stream.ReadUInt16();
				var MinorImageVersiom = stream.ReadUInt16();
				var MajorSubsystemVersion = stream.ReadUInt16();
				var MinorSubsystemVersion = stream.ReadUInt16();
				var Win32VersionValue = stream.ReadUInt32();
				var SizeOfImage = stream.ReadUInt32();
				var SizeOfHeaders = stream.ReadUInt32();
				var CheckSum = stream.ReadUInt32();
				var Subsystem = stream.ReadUInt16();
				var DllCharacteristics = stream.ReadUInt16();
				var SizeOfStackReserve = ReadWord();
				var SizeOfStackCommit = ReadWord();
				var SizeOfHeapReserve = ReadWord();
				var SizeOfHeapCommit = ReadWord();
				var LoaderFlags = stream.ReadUInt32();
				var NumberOfRvaAndSizes = stream.ReadUInt32();
				var DataDirectory = Enumerable.Range(0, 16).Select(_ => {
					var dir = new IMAGE_DATA_DIRECTORY.Serializer(stream).Read();
					return dir;
				}).ToArray();

				Data = new Data {
					Magic = Magic,
					MajorLinkerVersion = MajorLinkerVersion,
					MinorLinkerVersion = MinorLinkerVersion,
					SizeOfCode = SizeOfCode,
					SizeOfInitializedData = SizeOfInitializedData,
					SizeOfUninitializedData = SizeOfUninitializedData,
					AddressOfEntryPoint = AddressOfEntryPoint,
					BaseOfCode = BaseOfCode,
					BaseOfData = BaseOfData,
					ImageBase = ImageBase,
					SectionAlignment = SectionAlignment,
					FileAlignment = FileAlignment,
					MajorOperatingSystemVersion = MajorOperatingSystemVersion,
					MinorOperatingSystemVersion = MinorOperatingSystemVersion,
					MajorImageVersiom = MajorImageVersiom,
					MinorImageVersiom = MinorImageVersiom,
					MajorSubsystemVersion = MajorSubsystemVersion,
					MinorSubsystemVersion = MinorSubsystemVersion,
					Win32VersionValue = Win32VersionValue,
					SizeOfImage = SizeOfImage,
					SizeOfHeaders = SizeOfHeaders,
					CheckSum = CheckSum,
					Subsystem = Subsystem,
					DllCharacteristics = DllCharacteristics,
					SizeOfStackReserve = SizeOfStackReserve,
					SizeOfStackCommit = SizeOfStackCommit,
					SizeOfHeapReserve = SizeOfHeapReserve,
					SizeOfHeapCommit = SizeOfHeapCommit,
					LoaderFlags = LoaderFlags,
					NumberOfRvaAndSizes = NumberOfRvaAndSizes,
					DataDirectory = DataDirectory,
				};
				return Data;
			}
		}
	}

	namespace IMAGE_DATA_DIRECTORY {
		public class Data {
			public uint VirtualAddress;
			public uint Size;

			public object? x_Data { get; set; }
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var VirtualAddress = stream.ReadUInt32();
				var Size = stream.ReadUInt32();
				Data = new Data {
					VirtualAddress = VirtualAddress,
					Size = Size,
				};
				return Data;
			}
		}
	}


	namespace IMAGE_SECTION_HEADER {
		public class Data {
			public string Name = string.Empty;
			public uint Misc;
			public uint VirtualAddress;
			public uint SizeOfRawData;
			public uint PointerToRawData;
			public uint PointerToRelocations;
			public uint PointerToLinenumbers;
			public ushort NumberOfRelocations;
			public ushort NumberOfLinenumbers;
			public uint Characteristics;

			public uint PhysicalAddress => Misc;
			public uint VirtualSize => Misc;
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var Name = stream.ReadString(8, Encoding.UTF8).TrimEnd(['\0']);
				if (Name.StartsWith("/")) {
					var stringTableOffset = int.Parse(Name.Substring(1));
					// $FIXME: reference to string table (not implemented)
				}

				var Misc = stream.ReadUInt32();
				var VirtualAddress = stream.ReadUInt32();
				var SizeOfRawData = stream.ReadUInt32();
				var PointerToRawData = stream.ReadUInt32();
				var PointerToRelocations = stream.ReadUInt32();
				var PointerToLinenumbers = stream.ReadUInt32();
				var NumberOfRelocations = stream.ReadUInt16();
				var NumberOfLinenumbers = stream.ReadUInt16();
				var Characteristics = stream.ReadUInt32();


				return new Data {
					Name = Name,
					Misc = Misc,
					VirtualAddress = VirtualAddress,
					SizeOfRawData = SizeOfRawData,
					PointerToRawData = PointerToRawData,
					PointerToRelocations = PointerToRelocations,
					PointerToLinenumbers = PointerToLinenumbers,
					NumberOfRelocations = NumberOfRelocations,
					NumberOfLinenumbers = NumberOfLinenumbers,
					Characteristics = Characteristics,
				};
			}
		}
	}

	namespace IMAGE_NT_HEADERS {
		public class Data
		{
			public string Signature = "PE";
			public IMAGE_FILE_HEADER.Data FileHeader = new IMAGE_FILE_HEADER.Data();
			public IMAGE_OPTIONAL_HEADER.Data OptionalHeader = new IMAGE_OPTIONAL_HEADER.Data();
			public IMAGE_SECTION_HEADER.Data[] SectionHeaders = new IMAGE_SECTION_HEADER.Data[0];
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var signature = stream.ReadString(4, Encoding.ASCII);
				if (signature != "PE\0\0") throw new InvalidDataException();
				var fileHeader = new IMAGE_FILE_HEADER.Serializer(stream).Read();
				var optHeader = new IMAGE_OPTIONAL_HEADER.Serializer(stream).Read();

				var sectionHeaders = Enumerable.Range(0, fileHeader.NumberOfSections)
					.Select(_ => new IMAGE_SECTION_HEADER.Serializer(stream).Read())
					.ToArray();

				Data = new Data {
					Signature = signature,
					FileHeader = fileHeader,
					OptionalHeader = optHeader,
					SectionHeaders = sectionHeaders
				};

				return Data;
			}
		}
	}
}
