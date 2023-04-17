#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
	public class ADJUSTOR : SymbolDataReader, IThunk
	{
		public readonly UInt16 Delta;
		public readonly string Name;

		public ADJUSTOR(IServiceContainer ctx, SymbolHeader symHeader, SpanStream stream) : base(ctx, symHeader, stream) {
			Delta = ReadUInt16();
			Name = ReadSymbolString();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt16(Delta);
			w.WriteSymbolString(Name);
		}
	}
}
