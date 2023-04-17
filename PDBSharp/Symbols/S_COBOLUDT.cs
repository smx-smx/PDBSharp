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
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.Structures.UdtSym;

namespace Smx.PDBSharp.Symbols.S_COBOLUDT
{
	public class Serializer : UdtSym
	{
		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Write() {
			base.Write(SymbolType.S_COBOLUDT);
		}
	}
}
