#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE
{
	public class Serializer : Symbols.S_DEFRANGE_FRAMEPOINTER_REL.Serializer
	{
		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){

		}

		public void Write(PDBFile pdb, Stream stream) {
			SymbolDataWriter w = CreateWriter(SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE);
			w.WriteHeader();
		}
	}
}
