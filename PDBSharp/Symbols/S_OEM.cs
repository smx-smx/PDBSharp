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
	[SymbolReader(SymbolType.S_OEM)]
	public class S_OEM : SymbolDataReader
	{
		public readonly Guid Id;
		public readonly UInt32 TypeIndex;
		public readonly byte[] UserData;

		public S_OEM(Stream stream) : base(stream) {
			Id = new Guid(ReadBytes(16));
			TypeIndex = ReadUInt32();
			UserData = ReadRemaining();
		}
	}
}
