#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	public class OemSym
	{
		public Guid Id { get; set; }
		public LeafBase Type { get; set; }
		public byte[] UserData { get; set; }
	}

	public class S_OEM : ISymbol
	{
		public readonly Guid Id;
		public readonly ILeafContainer Type;
		public readonly byte[] UserData;

		public S_OEM(Context ctx, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);

			Id = new Guid(r.ReadBytes(16));
			Type = r.ReadIndexedTypeLazy();
			UserData = r.ReadRemaining();
		}

		public S_OEM(OemSym data) {
			Id = data.Id;
			Type = data.Type;
			UserData = data.UserData;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_OEM);
			w.WriteBytes(Id.ToByteArray());
			w.WriteIndexedType(Type);
			w.WriteBytes(UserData);

			w.WriteSymbolHeader();
		}
	}
}
