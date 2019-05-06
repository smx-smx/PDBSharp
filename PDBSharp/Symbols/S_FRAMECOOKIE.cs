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
	[SymbolReader(SymbolType.S_FRAMECOOKIE)]
	public class S_FRAMECOOKIE : SymbolDataReader
	{
		public readonly UInt32 Offset;
		public readonly UInt16 RegisterIndex;
		public readonly CookieType Type;
		public readonly byte Flags;

		public S_FRAMECOOKIE(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Offset = ReadUInt32();
			RegisterIndex = ReadUInt16();
			Type = ReadEnum<CookieType>();
			Flags = ReadByte();
		}
	}
}
