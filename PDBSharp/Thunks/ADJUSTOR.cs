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
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AdjustorThunk
	{
		public UInt16 Delta;
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

		public ADJUSTOR(SymbolHeader symHeader, THUNKSYM32 thunk, Stream stream) : base(symHeader, thunk, stream) {
			AdjustorThunk header = ReadStruct<AdjustorThunk>();
			string name = ReadSymbolString(symHeader);

			Data = new AdjustorThunkInstance() {
				Header = header,
				Name = name
			};
		}

		THUNKSYM32 IThunk.Thunk => this.Thunk;
	}
}
