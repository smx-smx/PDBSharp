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
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_GMANPROC
{
	public class Serializer : ManProcSymSerializerBase, ISymbolSerializer
	{
		public Serializer(IServiceContainer ctx, SpanStream stream, IModule cvStream) : base(ctx, stream, cvStream) {
		}

		public void Write() {
			base.Write(SymbolType.S_GMANPROC);
		}

		public ISymbolData? GetData() => Data;
	}
}
