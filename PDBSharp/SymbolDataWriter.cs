#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	public class SymbolDataWriter : TypeDataWriter
	{
		protected readonly SymbolType symbolType;

		public SymbolDataWriter(
			IServiceContainer ctx, SpanStream stream,
			SymbolType type
		) : base(ctx, stream) {
			this.symbolType = type;
			Init();
		}

		private unsafe void Init() {
			Position = sizeof(SymbolHeader);
		}

		public new void WriteHeader() {
			long dataSize = Position;
			Position = 0;
			SymbolHeader hdr = new SymbolHeader() {
				Type = symbolType,
				Length = (ushort)dataSize
			};
			Write<SymbolHeader>(hdr);
		}

		public void WriteSymbolString(string value) {
			if (symbolType < SymbolType.S_ST_MAX) {
				WriteString(value);
			} else {
				WriteCString(value);
			}
		}
	}
}
