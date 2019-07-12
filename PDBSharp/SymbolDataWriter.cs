#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public class SymbolDataWriter : TypeDataWriter
	{
		protected readonly SymbolType symbolType;

		public SymbolDataWriter(PDBFile pdb, Stream stream, SymbolType type) : base(pdb, stream, Leaves.LeafType.SPECIAL_BUILTIN) {
			this.symbolType = type;
			Stream.Position = Marshal.SizeOf<SymbolHeader>();
		}

		public void WriteSymbolHeader() {
			long dataSize = Stream.Position;
			PerformAt(0, () => {
				SymbolHeader hdr = new SymbolHeader() {
					Type = symbolType,
					Length = (ushort)dataSize
				};
				WriteStruct<SymbolHeader>(hdr);
			});
		}

		public void WriteSymbolString(string value) {
			if(symbolType < SymbolType.S_ST_MAX) {
				WriteString(value);
			} else {
				WriteCString(value);
			}
		}
	}
}
