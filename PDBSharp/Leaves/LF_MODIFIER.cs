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

namespace Smx.PDBSharp.Leaves
{

	public class LF_MODIFIER : ILeaf
	{
		public readonly CVModifier Flags;
		public readonly ILeafContainer ModifiedType;

		public LF_MODIFIER(IServiceContainer pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			ModifiedType = r.ReadIndexedTypeLazy();
			Flags = r.ReadFlagsEnum<CVModifier>();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_MODIFIER);
			w.WriteIndexedType(ModifiedType);
			w.WriteEnum<CVModifier>(Flags);
			w.WriteLeafHeader();
		}

		public override string ToString() {
			return $"LF_MODIFIER[ModifiedType='{ModifiedType}', Flags='{Flags}']";
		}
	}
}
