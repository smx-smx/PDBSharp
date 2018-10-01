#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Thunks
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct AdjustorThunk
	{
		// duplicated uint16 kept for documentation purposes

		[FieldOffset(0)]
		public UInt16 Delta; //with following string
		[FieldOffset(0)]
		public UInt16 VTableOffset;
	}

	public struct AdjustorThunkInstance
	{
		public AdjustorThunk Header;
		public string Name;
	}

	[ThunkReader(ThunkType.ADJUSTOR)]
	public class ADJUSTOR : ThunkReaderBase, IThunk
	{
		public readonly AdjustorThunkInstance Data;

		public ADJUSTOR(THUNKSYM32 thunk, Stream stream) : base(thunk, stream) {
			AdjustorThunk header = ReadStruct<AdjustorThunk>();
			string name = ReadSymbolString(thunk.Header);

			Data = new AdjustorThunkInstance() {
				Header = header,
				Name = name
			};
		}

		public ThunkType Type => ThunkType.ADJUSTOR;

	}
}
