#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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
	public class VCALL : SymbolDataReader, IThunk
	{
		public readonly UInt16 VTableOffset;

		public VCALL(Context ctx, SymbolHeader header, Stream stream) : base(ctx, header, stream) {
			VTableOffset = ReadUInt16();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt16(VTableOffset);
		}
	}
}
