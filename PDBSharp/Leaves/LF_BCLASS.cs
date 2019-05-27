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
	[LeafReader(LeafType.LF_BCLASS)]
	public class LF_BCLASS : TypeDataReader
	{
		public readonly FieldAttributes Attributes;
		public readonly ILeaf BaseClassType;

		public LF_BCLASS(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Attributes = new FieldAttributes(ReadUInt16());
			BaseClassType = ReadIndexedTypeLazy();

			var varDataType = ReadVaryingType(out uint dataSize);
		}
	}
}
