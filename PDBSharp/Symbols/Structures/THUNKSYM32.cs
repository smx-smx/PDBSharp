#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Thunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct THUNKSYM32
	{
		public SymbolHeader Header;
		public UInt32 Parent;
		public UInt32 End;
		public UInt32 Next;
		public UInt32 Offset;
		public UInt16 Segment;
		public UInt16 ThunkSize;
		public ThunkType Ordinal;
	}

	public struct ThunkSym32Instance
	{
		public THUNKSYM32 Header;
		public string Name;
		public IThunk Data;
	}

	public class ThunkSym32Reader : ReaderBase
	{
		public readonly ThunkSym32Instance Data;

		public ThunkSym32Reader(Stream stream) : base(stream) {
			THUNKSYM32 header = ReadStruct<THUNKSYM32>();
			string name = ReadSymbolString(header.Header);
			IThunk data = ReadThunk(header);

			Data = new ThunkSym32Instance() {
				Header = header,
				Name = name,
				Data = data
			};
		}
	}
}
