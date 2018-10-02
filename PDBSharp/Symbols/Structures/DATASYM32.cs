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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DATASYM32
	{
		public UInt32 Offset;
		public UInt16 Segment;
		public UInt32 TypeIndex;
	}

	public struct DataSym32Instance
	{
		public DATASYM32 Header;
		public string Name;
	}

	public class DataSym32Reader : SymbolReaderBase
	{
		public readonly DataSym32Instance Data;

		public DataSym32Reader(Stream stream) : base(stream) {
			DATASYM32 header = ReadStruct<DATASYM32>();
			string name = ReadSymbolString(Header);

			Data = new DataSym32Instance() {
				Header = header,
				Name = name
			};
		}
	}
}
