#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Smx.PDBSharp.Symbols;

namespace Smx.PDBSharp
{

	public interface ISectionContrib
	{
		int Size { get; }
	}

	public class SectionContrib : SectionContrib40 {
		public readonly UInt32 DataCrc;
		public readonly UInt32 RelocCrc;

		public const int SIZE = SectionContrib40.SIZE + 8;

		public SectionContrib(Stream stream) : base(stream) {
			DataCrc = ReadUInt32();
			RelocCrc = ReadUInt32();
		}
	}

	public class SectionContrib2 : SectionContrib
	{
		public readonly UInt32 CoffSectionIndex;

		public const int SIZE = SectionContrib.SIZE + 4;

		public SectionContrib2(Stream stream) : base(stream) {
			
		}
	}

	public class SectionContrib40 : ReaderBase
	{
		public readonly UInt16 SectionIndex;
		public readonly UInt32 Offset;
		public readonly UInt32 Size;
		public readonly UInt32 Characteristics;
		public readonly UInt16 ModuleIndex;

		public const int SIZE = 20;

		public SectionContrib40(Stream stream) : base(stream) {
			SectionIndex = ReadUInt16();
			ReadUInt16();
			
			Offset = ReadUInt32();
			Size = ReadUInt32();
			Characteristics = ReadUInt32();
			
			ModuleIndex = ReadUInt16();
			ReadUInt16();
		}
	}

	public class ModuleInfoFlags
	{
		private readonly UInt16 flags;
		public ModuleInfoFlags(UInt16 flags) {
			this.flags = flags;
		}

		public bool Written => (flags & 1) == 1;
		public bool ECEnabled => ((flags >> 1) & 1) == 1;
		public byte TSMListIndex => (byte)((flags >> 8) & 8);
	}

	public struct ECInfo
	{
		public UInt32 SrcFileNameIndex;
		public UInt32 PdbFileNameIndex;
	}

	public class ModuleInfo : ReaderBase
	{
		public readonly UInt32 OpenModuleHandle;
		public readonly SectionContrib SectionContribution;
		public readonly ModuleInfoFlags Flags;
		public readonly Int16 StreamNumber;
		public readonly UInt32 SymbolsSize;
		public readonly UInt32 LinesSize;
		public readonly UInt32 C13LinesSize;
		public readonly UInt16 NumberOfFiles;
		public readonly UInt32 FileNameOffsets;

		public readonly ECInfo ECInfo;

		public readonly string ModuleName;
		public readonly string ObjectFileName;

		//////////////////////////////

		private readonly Context ctx;
		public const int SIZE = 38 + SectionContrib.SIZE;
		public int Size => SIZE + ModuleName.Length + ObjectFileName.Length;

		public string SourceFileName => GetEcString(ECInfo.SrcFileNameIndex);
		public string PDBFileName => GetEcString(ECInfo.PdbFileNameIndex);

		private string GetEcString(uint nameIndex) {
			return ctx.DbiReader.EC.NameTable.GetString(nameIndex);
		}

		public ModuleInfo(Context ctx, Stream stream) : base(stream) {
			this.ctx = ctx;

			long modiStartOffset = stream.Position;

			OpenModuleHandle = ReadUInt32();
			SectionContribution = new SectionContrib(stream);

			Flags = new ModuleInfoFlags(ReadUInt16());
			StreamNumber = ReadInt16();

			SymbolsSize = ReadUInt32();
			LinesSize = ReadUInt32();
			C13LinesSize = ReadUInt32();

			NumberOfFiles = ReadUInt16();
			ReadUInt16();

			FileNameOffsets = ReadUInt32();

			ECInfo = ReadStruct<ECInfo>();

			ModuleName = ReadCString();
			ObjectFileName = ReadCString();
		}
	}

	public class ModuleListReader : ReaderBase
	{
		private readonly long listStartOffset;
		private readonly long listEndOffset;

		private readonly uint listSize;

		private long lastPosition;

		private readonly Context ctx;

		public ModuleListReader(Context ctx, Stream stream, uint moduleListSize) : base(stream) {
			this.ctx = ctx;

			listStartOffset = stream.Position;
			listSize = moduleListSize;
			listEndOffset = listStartOffset + listSize;
			lastPosition = listStartOffset;

			lazyModules = new Lazy<IEnumerable<ModuleInfo>>(ReadModules);
		}

		private Lazy<IEnumerable<ModuleInfo>> lazyModules;

		public IEnumerable<ModuleInfo> Modules => lazyModules.Value;

		private IEnumerable<ModuleInfo> ReadModules() {
			while (lastPosition < listEndOffset) {
				Stream.Position = lastPosition;

				ModuleInfo mod = new ModuleInfo(ctx, Stream);
				lastPosition += mod.Size + AlignStream(sizeof(int));

				yield return mod;
			}
		}
	}
}
