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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp
{
	public class InvalidAssumptionException : NotSupportedException
	{
		public InvalidAssumptionException(string message) : base(message) {
		}
	}

	public class SourceFileModuleReader : ReaderSpan, IModule
	{
		private readonly PDBFile pdb;

		public readonly UInt16 NumberOfFiles;
		public readonly SourceFileModuleReader[] Children;

		public readonly string FileName;


		public event OnSymbolDataDelegate OnSymbolData;

		public IEnumerable<Symbol> Symbols => Enumerable.Empty<Symbol>();

		public SourceFileModuleReader(IServiceContainer ctx, ReaderSpan stream) : base(stream) {
			this.pdb = ctx.GetService<PDBFile>();

			// including .c file
			NumberOfFiles = ReadUInt16();

			UInt16 hdrFlags = ReadUInt16();
			switch (hdrFlags) {
				case 0:
				case 1:
					break;
				default:
					throw new InvalidAssumptionException("hdrFlags");
			}
			bool isRoot = (hdrFlags & 1) == 1;

			// dataOffset - startOfData = headerSize
			UInt32 dataOffset = ReadUInt32();

			UInt32[] childFileOffsets = Enumerable.Range(1, NumberOfFiles - 1)
					.Select(_ => ReadUInt32())
					.ToArray();

			UInt32 unk1 = ReadUInt32();
			UInt32 unk2 = ReadUInt32();

			if (hdrFlags == 1) {
				UInt32 maybeFlags = ReadUInt32();

				UInt32 numTables = ReadUInt32();
				UInt32[] tableOffsets = Enumerable.Range(1, (int)numTables)
					.Select(_ => ReadUInt32())
					.ToArray();

				UInt32[][] unkDataPerTable = Enumerable.Range(1, (int)numTables)
						.Select(_ => {
							UInt32[] data = new UInt32[2];
							data[0] = ReadUInt32();
							data[1] = ReadUInt32();
							return data;
						}).ToArray();
			}

			FileName = ReadCString();

			// table data follows...
			// skip it for now, and go read child headers
			Children = childFileOffsets.Select(offset => {
				return PerformAt(offset, () => {
					return new SourceFileModuleReader(ctx, this);
				});
			}).ToArray();
		}
	}
}
