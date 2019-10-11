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
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class LF_VARSTRING : ILeaf
	{
		public readonly string Value;

		public LF_VARSTRING(IServiceContainer pdb, SpanReader stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			UInt16 length = r.ReadUInt16();
			byte[] data = r.ReadBytes((int)length);
			Value = Encoding.ASCII.GetString(data);
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_VARSTRING);
			w.WriteUInt16((ushort)Value.Length);
			w.WriteBytes(Encoding.ASCII.GetBytes(Value));
			w.WriteLeafHeader();
		}
	}
}
