#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Thunks
{
	public class PCODE : SymbolDataReader, IThunk
	{
		public PCODE(IServiceContainer ctx, SymbolHeader header, Stream stream) : base(ctx, header, stream) {
		}

		public void Write(SymbolDataWriter w) {
		}
	}
}
