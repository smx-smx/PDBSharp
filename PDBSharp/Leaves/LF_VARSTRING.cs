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
	public class LF_VARSTRING : SymbolReaderBase, ILeaf<string>
	{
		public LF_VARSTRING(Stream stream) : base(stream) {
			UInt16 length = Reader.ReadUInt16();
			byte[] data = Reader.ReadBytes((int)length);
			Value = Encoding.ASCII.GetString(data);
		}

		public string Value { get; }
	}
}
