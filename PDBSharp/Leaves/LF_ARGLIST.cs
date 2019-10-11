#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp.Leaves
{
	public class LF_ARGLIST : ILeaf
	{
		public UInt16 NumberOfArguments;
		public ILeafContainer[] ArgumentTypes;

		public LF_ARGLIST(IServiceContainer pdb, SpanStream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			NumberOfArguments = r.ReadUInt16();
			r.ReadUInt16(); //padding
			ArgumentTypes = Enumerable.Range(1, NumberOfArguments)
											.Select(_ => r.ReadIndexedTypeLazy())
											.ToArray();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_ARGLIST);
			w.WriteUInt16(NumberOfArguments);
			w.WriteUInt16(0x00);

			foreach (ILeafContainer leaf in ArgumentTypes) {
				w.WriteIndexedType(leaf);
			}

			w.WriteLeafHeader();
		}

		public override string ToString() {
			return $"LF_ARGLIST[NumberOfArguments='{NumberOfArguments}', " +
				$"ArgumentTypes='{string.Join(", ", ArgumentTypes.Select(a => a.Data.ToString()))}']";
		}
	}
}
