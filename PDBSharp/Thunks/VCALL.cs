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
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VcallThunk
	{
		public UInt16 VTableOffset;
	}

	[ThunkReader(ThunkType.VCALL)]
	public class VCALL : ThunkReaderBase, IThunk
	{
		public readonly VcallThunk Data;

		public VCALL(SymbolHeader header, Stream stream) : base(header, stream) {
			Data = ReadStruct<VcallThunk>();
		}
	}
}
