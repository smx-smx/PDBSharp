#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_CALLEES)]
	public class S_CALLEES : SymbolDataReader
	{
		public readonly UInt32 NumberOfFunctions;
		public readonly UInt32[] Functions;

		public S_CALLEES(Stream stream) : base(stream) {
			NumberOfFunctions = ReadUInt32();
			Functions = Enumerable
				.Range(1, (int)NumberOfFunctions)
				.Select(_ => ReadUInt32())
				.ToArray();
		}
	}
}
