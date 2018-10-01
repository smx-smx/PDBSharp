#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_ULONG)]
	public class LF_ULONG : ReaderBase, ILeaf<uint>
	{
		public LeafType Type => LeafType.LF_ULONG;

		public LF_ULONG(Stream stream) : base(stream) {
			Value = Reader.ReadUInt32();
		}

		public uint Value { get; }
		
	}
}
