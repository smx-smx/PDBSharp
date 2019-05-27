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
	[LeafReader(LeafType.LF_VARSTRING)]
	public class LF_VARSTRING : TypeDataReader
	{
		public readonly string Value;
		public LF_VARSTRING(PDBFile pdb, Stream stream) : base(pdb, stream) {
			UInt16 length = ReadUInt16();
			byte[] data = ReadBytes((int)length);
			Value = Encoding.ASCII.GetString(data);
		}
	}
}
