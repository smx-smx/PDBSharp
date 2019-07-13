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

	public interface SectionContribution
	{
		int Size { get; }
	}

	public class SectionContribution40 : ReaderBase, SectionContribution
	{
		public readonly UInt16 SectionIndex;
		public readonly UInt32 Offset;
		public readonly UInt32 Size;
		public readonly UInt32 Characteristics;
		public readonly UInt16 ModuleIndex;

		// footer
		public readonly UInt32 DataCrc;
		public readonly UInt32 RelocCrc;

		const int SIZE = 28;

		public SectionContribution40(Stream stream) : base(stream) {
			SectionIndex = ReadUInt16();
			ReadUInt16();

			Offset = ReadUInt32();
			Size = ReadUInt32();
			Characteristics = ReadUInt32();

			ModuleIndex = ReadUInt16();
			ReadUInt16();

			DataCrc = ReadUInt32();
			RelocCrc = ReadUInt32();
		}

		int SectionContribution.Size => SIZE;
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

	public class ModuleInfo : ReaderBase
	{
		public readonly UInt32 OpenModuleHandle;
		public readonly SectionContribution SectionContribution;
		public readonly ModuleInfoFlags Flags;
		public readonly Int16 StreamNumber;
		public readonly UInt32 SymbolsSize;
		public readonly UInt32 LinesSize;
		public readonly UInt32 C13LinesSize;
		public readonly UInt16 NumberOfFiles;
		public readonly UInt32 FileNameOffsets;

		public readonly UInt32 SrcFileNameIndex;
		public readonly UInt32 PdbFileNameIndex;

		public readonly string ModuleName;
		public readonly string ObjectFileName;


		public readonly int Size;

		public ModuleInfo(Stream stream) : base(stream) {
			long modiStartOffset = stream.Position;

			OpenModuleHandle = ReadUInt32();
			SectionContribution = new SectionContribution40(stream);

			Flags = new ModuleInfoFlags(ReadUInt16());
			StreamNumber = ReadInt16();

			SymbolsSize = ReadUInt32();
			LinesSize = ReadUInt32();
			C13LinesSize = ReadUInt32();

			NumberOfFiles = ReadUInt16();
			ReadUInt16();

			FileNameOffsets = ReadUInt32();

			//ECInfo start
			SrcFileNameIndex = ReadUInt32();
			PdbFileNameIndex = ReadUInt32();
			//ECInfo end

			ModuleName = ReadCString();
			ObjectFileName = ReadCString();

			// Align stream position
			int padding = AlignStream(sizeof(int));

			Size = 38 +
				SectionContribution.Size +
				ModuleName.Length +
				ObjectFileName.Length +
				padding;
		}
	}

	public class ModuleListReader : ReaderBase
	{
		private readonly long listStartOffset;
		private readonly long listEndOffset;

		private readonly uint listSize;

		private long lastPosition;

		public ModuleListReader(Stream stream, uint moduleListSize) : base(stream) {
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

				ModuleInfo mod = new ModuleInfo(Stream);
				long moduleSize = mod.Size;
				lastPosition += moduleSize;

				yield return mod;
			}
		}
	}
}
