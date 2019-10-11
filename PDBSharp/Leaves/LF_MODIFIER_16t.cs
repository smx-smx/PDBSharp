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
	public class LF_MODIFIER_16t : ILeaf
	{
		public readonly CVModifier Attributes;
		public readonly ILeafContainer ModifiedType;

		public LF_MODIFIER_16t(IServiceContainer pdb, SpanStream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Attributes = r.ReadFlagsEnum<CVModifier>();
			ModifiedType = r.ReadIndexedType16Lazy();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_MODIFIER_16t);
			w.WriteEnum<CVModifier>(Attributes);
			w.WriteIndexedType16(ModifiedType);
			w.WriteLeafHeader();
		}
	}
}
