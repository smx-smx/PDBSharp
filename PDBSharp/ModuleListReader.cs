#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp
{
	public interface ISectionContrib { }

	public class SectionContrib : SectionContrib40, ISectionContrib
	{
		public readonly UInt32 DataCrc;
		public readonly UInt32 RelocCrc;

		public new const int SIZE = SectionContrib40.SIZE + 8;

		public SectionContrib(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			DataCrc = ReadUInt32();
			RelocCrc = ReadUInt32();
		}
	}

	public class SectionContrib2 : SectionContrib
	{
		public readonly UInt32 CoffSectionIndex;

		public new const int SIZE = SectionContrib.SIZE + 4;

		public SectionContrib2(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			CoffSectionIndex = ReadUInt32();
		}
	}

	public class SectionContrib40 : SpanStream, IComparable<SectionContrib40>, ISectionContrib
	{
		public readonly UInt16 SectionIndex;
		public readonly UInt32 Offset;
		public readonly UInt32 Size;
		public readonly UInt32 Characteristics;
		public readonly Int16 ModuleIndex;

		public const int SIZE = 20;

		//////////////

		private readonly ILazy<IModuleContainer?> moduleLazy;
		public IModuleContainer? Module => moduleLazy.Value;

		public SectionContrib40(IServiceContainer ctx, SpanStream stream) : base(stream) {
			SectionIndex = ReadUInt16();
			ReadUInt16();

			Offset = ReadUInt32();
			Size = ReadUInt32();
			Characteristics = ReadUInt32();

			ModuleIndex = ReadInt16();
			ReadUInt16();

			////////
			DBIReader dbi = ctx.GetService<DBIReader>();

			moduleLazy = LazyFactory.CreateLazy(() => ModuleIndex == -1
				? null
				: dbi.Modules[this.ModuleIndex]);
		}

		public int CompareTo(SectionContrib40 other) {
			if(this.SectionIndex == other.SectionIndex) {
				// offset >= and within section size
				if (other.Offset - Offset < Size) {
					return 0;
				}

				return this.Offset.CompareTo(other.Offset);
			}

			return SectionIndex.CompareTo(other.SectionIndex);
		}

		public bool Contains(int sectionIndex, long offset) {
			// same section, offset >= and within section size
			return (this.SectionIndex == sectionIndex && offset - this.Offset < Size);
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

	public class ModuleInfo : SpanStream
	{
		public readonly UInt32 OpenModuleHandle;
		public readonly ISectionContrib? SectionContribution;
		public readonly ModuleInfoFlags Flags;
		public readonly Int16 StreamNumber;
		public readonly UInt32 SymbolsSize;
		public readonly UInt32 LinesSize;
		public readonly UInt32 C13LinesSize;
		public readonly UInt16 NumberOfFiles;
		public readonly UInt32 FileNameOffsets;

		public readonly ECInfo? ECInfo;

		public readonly string ModuleName;
		public readonly string ObjectFileName;

		//////////////////////////////

		public readonly IEnumerable<SectionContrib40>? SectionContribs;

		public readonly int ModuleIndex;
		private readonly DBIReader dbi;

		public int ExternalModuleIndex => ModuleIndex + 1;

		public readonly long Size;

		public string? SourceFileName => (ECInfo != null)
			? GetEcString(ECInfo.Value.SrcFileNameIndex)
			: null;

		public string? PDBFileName => (ECInfo != null)
			? GetEcString(ECInfo.Value.PdbFileNameIndex)
			: null;

		private string? GetEcString(uint nameIndex) {
			return this.dbi.EC?.NameTable.GetString(nameIndex) ?? null;
		}

		public ModuleInfo(IServiceContainer ctx, SpanStream __stream, int modIndex) : base(__stream) {
			long savedPosition = Position;

			this.ModuleIndex = modIndex;
			DBIReader dbi = ctx.GetService<DBIReader>();
			this.dbi = dbi;

			MSFReader msf = ctx.GetService<MSFReader>();

			OpenModuleHandle = ReadUInt32();

			switch (msf.FileType) {
				case PDBType.Big:
					SectionContribution = new SectionContrib(ctx, this);
					Position += SectionContrib.SIZE;
					break;
				case PDBType.Small:
					SectionContribution = new SectionContrib40(ctx, this);
					Position += SectionContrib40.SIZE;
					break;
			}

			Flags = new ModuleInfoFlags(ReadUInt16());
			StreamNumber = ReadInt16();

			SymbolsSize = ReadUInt32();
			LinesSize = ReadUInt32();
			C13LinesSize = ReadUInt32();

			NumberOfFiles = ReadUInt16();
			ReadUInt16();

			FileNameOffsets = ReadUInt32();

			if (msf.FileType == PDBType.Big) {
				ECInfo = Read<ECInfo>();
			} else {
				ECInfo = null;
			}

			ModuleName = ReadCString();
			ObjectFileName = ReadCString();

			Size = Position - savedPosition;

			///////

			var sc = this.dbi.SectionContribs?.GetByModule(this);
			if (sc != null) {
				SectionContribs = new CachedEnumerable<SectionContrib40>(sc);
			}
		}
	}

	public class ModuleListReader : SpanStream
	{
		private readonly long listStartOffset;
		private readonly long listEndOffset;

		private readonly uint listSize;

		private readonly IServiceContainer ctx;

		public ModuleListReader(IServiceContainer ctx, SpanStream __stream, uint moduleListSize) : base(__stream) {
			this.ctx = ctx;

			listStartOffset = Position;
			listSize = moduleListSize;
			listEndOffset = listStartOffset + listSize;

			Modules = new CachedEnumerable<ModuleInfo>(ReadModules());
		}

		public readonly IEnumerable<ModuleInfo> Modules;


		private IEnumerable<ModuleInfo> ReadModules() {
			for(int modIndex=0; Position < listEndOffset; modIndex++) {
				ModuleInfo mod = new ModuleInfo(ctx, this, modIndex);

				Position += mod.Size;
				AlignStream(sizeof(int));
				yield return mod;
			}
		}
	}
}
