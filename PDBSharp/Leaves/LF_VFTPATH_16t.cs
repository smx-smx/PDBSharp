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
	[LeafReader(LeafType.LF_VFTPATH_16t)]
	public class LF_VFTPATH_16t : TypeDataReader
	{
		public readonly UInt16 NumElements;
		public readonly ILeaf Bases;

		public LF_VFTPATH_16t(PDBFile pdb, Stream stream) : base(pdb, stream) {
			NumElements = ReadUInt16();
			Bases = ReadIndexedType16Lazy();
		}
	}
}
