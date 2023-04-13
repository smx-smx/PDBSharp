#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Thunks
{
	public class VCALL : SymbolDataReader, IThunk
	{
		public readonly UInt16 VTableOffset;

		public VCALL(IServiceContainer ctx, SymbolHeader header, SpanStream stream) : base(ctx, header, stream) {
			VTableOffset = ReadUInt16();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt16(VTableOffset);
		}
	}
}
