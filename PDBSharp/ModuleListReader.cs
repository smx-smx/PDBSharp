#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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

		public ModuleInfo(Stream stream) : base(stream) {
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
		}
	}

	public class ModuleListReader : ReaderBase
	{
		public ModuleListReader(Stream stream) : base(stream) {
		}

		private IEnumerable<ModuleInfo> modules;
		public IEnumerable<ModuleInfo> Modules {
			get {
				if (modules == null)
					modules = ReadModules().Cached();
				return modules;
			}
		}

		private IEnumerable<ModuleInfo> ReadModules() {
			var remaining = Stream.Length;
			while (remaining > 0) {
				long savedPos = Stream.Position;
				ModuleInfo mod = new ModuleInfo(Stream);

				long moduleSize = (Stream.Position - savedPos) + AlignStream(sizeof(int));
				yield return mod;

				remaining -= moduleSize;
			}
		}
	}
}
