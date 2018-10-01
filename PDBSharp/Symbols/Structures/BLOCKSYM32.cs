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
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public struct BLOCKSYM32
	{
		public SymbolHeader Header;
		public UInt32 Parent;
		public UInt32 End;
		public UInt32 Length;
		public UInt32 Offset;
		public UInt16 Segment;
	}

	public struct BlockSym32Instance
	{
		public BLOCKSYM32 Header;
		public string Name;
	}

	public class BlockSym32Reader : ReaderBase
	{
		public readonly BlockSym32Instance Data;

		public BlockSym32Reader(Stream stream) : base(stream) {
			BLOCKSYM32 header = ReadStruct<BLOCKSYM32>();

			string name = null;
			if(Stream.Position < Stream.Length) {
				name = ReadSymbolString(header.Header);
			}

			Data = new BlockSym32Instance() {
				Header = header,
				Name = name
			};
		}
	}
}
