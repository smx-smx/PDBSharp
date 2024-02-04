#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.PE.IMAGE_OPTIONAL_HEADER;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace Smx.PDBSharp.PE
{
	public class PEFile : IDisposable
	{
		private MFile mf;
		private SpanStream stream;

		public IMAGE_DOS_HEADER.Data DosHeader;
		public IMAGE_NT_HEADERS.Data NtHeaders;

		public IMAGE_DEBUG_DIRECTORY.Data? DebugDirectory {
			get {
				return NtHeaders.OptionalHeader
					.GetDataDirectory(ImageDirectoryEntry.Debug).x_Data as IMAGE_DEBUG_DIRECTORY.Data;
			}
		}

		public long GetFileOffset(long virtualAddress) {
			var sec = NtHeaders.SectionHeaders.FirstOrDefault(s => {
				var start = s.VirtualAddress;
				var end = start + s.VirtualSize;
				return virtualAddress >= start && virtualAddress < end;
			});
			return sec.PointerToRawData + (virtualAddress - sec.VirtualAddress);
		}

		public long GetVirtualAddress(long fileAddress) {
			var sec = NtHeaders.SectionHeaders.FirstOrDefault(s => {
				var start = s.PointerToRawData;
				var end = start + s.SizeOfRawData;
				return fileAddress >= start && fileAddress < end;
			});
			return sec.VirtualAddress + (fileAddress - sec.PointerToRawData);
		}

		private void ReadImageDirectories() {
			var nDataDirectories = NtHeaders.OptionalHeader.DataDirectory.Length;
			for(int i=0; i<nDataDirectories; i++) {
				var dir = NtHeaders.OptionalHeader.DataDirectory[i];
				if (dir.VirtualAddress == 0) continue;
				var fileAddr = GetFileOffset(dir.VirtualAddress);
				var data = stream.PerformAt(fileAddr, () => {
					return stream.ReadBytes((int)dir.Size);
				});

				switch ((ImageDirectoryEntry)i) {
					case ImageDirectoryEntry.Debug:
						stream.PerformAt(fileAddr, () => {
							dir.x_Data = new IMAGE_DEBUG_DIRECTORY.Serializer(stream).Read();
						});
						break;
				}
			}
		}

		public PEFile(MFile mf) {
			this.mf = mf;
			this.stream = new SpanStream(mf);
			DosHeader = new IMAGE_DOS_HEADER.Serializer(stream).Read();
			NtHeaders = new IMAGE_NT_HEADERS.Serializer(stream.PerformAt(DosHeader.e_lfanew, () => {
				return stream.SliceHere();
			})).Read();
			ReadImageDirectories();
		}

		public static PEFile Open(string filePath) {
			var mf = MFile.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new PEFile(mf);
		}

		public void Dispose() {
			stream.Dispose();
			mf.Dispose();
		}
	}
}
