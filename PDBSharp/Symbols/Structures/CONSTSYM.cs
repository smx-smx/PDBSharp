#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ConstSym
	{
		public LeafContainerBase Type { get; set; }
		public LeafContainerBase Value { get; set; }
		public string Name { get; set; }
	}

	public abstract class ConstSymBase
	{
		public readonly ILeafContainer Type;
		public readonly ILeafContainer Value;
		public readonly string Name;

		public ConstSymBase(IServiceContainer ctx, ReaderSpan stream) {
			var r = new SymbolDataReader(ctx, stream);

			Type = r.ReadIndexedTypeLazy();

			Value = r.ReadVaryingType(out uint dataSize);
			Name = r.ReadSymbolString();
		}

		public ConstSymBase(ConstSym data) {
			Type = data.Type;
			Value = data.Value;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream, SymbolType symbolType) {
			var w = new SymbolDataWriter(pdb, stream, symbolType);

			w.WriteIndexedType(Type);
			w.WriteVaryingType(Value);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
