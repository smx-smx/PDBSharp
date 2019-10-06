#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_VFTABLE : ILeaf
	{
		public readonly ILeafContainer Type;
		public readonly ILeafContainer BaseVfTable;
		public readonly UInt32 OffsetInObjectLayout;
		/// <summary>
		/// Size in Bytes
		/// </summary>
		public readonly UInt32 NamesSize;
		public readonly string[] Names;

		public LF_VFTABLE(IServiceContainer pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			Type = r.ReadIndexedTypeLazy();
			BaseVfTable = r.ReadIndexedTypeLazy();
			OffsetInObjectLayout = r.ReadUInt32();
			NamesSize = r.ReadUInt32();

			List<string> lstNames = new List<string>();

			uint read = 0;
			long savedPos = stream.Position;
			while (read < NamesSize) {
				lstNames.Add(r.ReadCString());
				read += (uint)(stream.Position - savedPos);
				savedPos = stream.Position;
			}
			Names = lstNames.ToArray();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_VFTABLE);
			w.WriteIndexedType(Type);
			w.WriteIndexedType(BaseVfTable);
			w.WriteUInt32(OffsetInObjectLayout);
			w.WriteUInt32(NamesSize);
			foreach (string name in Names) {
				w.WriteCString(name);
			}
			w.WriteLeafHeader();
		}
	}
}
