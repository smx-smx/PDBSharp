#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_VFTABLE)]
	public class LF_VFTABLE : TypeDataReader
	{
		public readonly Lazy<ILeaf> Type;
		public readonly Lazy<ILeaf> BaseVfTable;
		public readonly UInt32 OffsetInObjectLayout;
		/// <summary>
		/// Size in Bytes
		/// </summary>
		public readonly UInt32 NamesSize;
		public readonly string[] Names;

		public LF_VFTABLE(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Type = ReadIndexedTypeLazy();
			BaseVfTable = ReadIndexedTypeLazy();
			OffsetInObjectLayout = ReadUInt32();
			NamesSize = ReadUInt32();

			List<string> lstNames = new List<string>();

			uint read = 0;
			long savedPos = stream.Position;
			while(read < NamesSize) {
				lstNames.Add(ReadCString());
				read += (uint)(stream.Position - savedPos);
				savedPos = stream.Position;
			}
			Names = lstNames.ToArray();
		}
	}
}
