#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp;
using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp.Thunks
{
	[ThunkReader(ThunkType.VCALL)]
	public class VCALL : SymbolDataReader, IThunk
	{
		public readonly UInt16 VTableOffset;

		public VCALL(PDBFile pdb, SymbolHeader header, Stream stream) : base(pdb, header, stream) {
			VTableOffset = ReadUInt16();
		}
	}
}
