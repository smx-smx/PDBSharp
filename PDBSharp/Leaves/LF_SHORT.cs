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
	[LeafReader(LeafType.LF_SHORT)]
	public class LF_SHORT : ReaderBase, ILeaf<short>
	{
		public LeafType Type => LeafType.LF_SHORT;

		public short Value { get; }

		public LF_SHORT(Stream stream) : base(stream) {
			Value = Reader.ReadInt16();
		}
	}
}
