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
	[LeafReader(LeafType.LF_CHAR)]
	class LF_CHAR : ReaderBase, ILeaf<char>
	{
		public LeafType Type => LeafType.LF_CHAR;
		public char Value { get; }

		public LF_CHAR(Stream stream) : base(stream) {
			Value = Convert.ToChar(Reader.ReadByte());
		}
	}
}
