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
	public struct REGREL32
	{
		public SymbolHeader Header;
		public UInt32 Offset;
		public UInt32 TypeIndex;
		public UInt16 RegisterIndex;
	}

	public struct RegRel32Instance
	{
		public REGREL32 Header;
		public string Name;
	}

	public class RegRel32Reader : ReaderBase
	{
		public readonly RegRel32Instance Data;

		public RegRel32Reader(Stream stream) : base(stream) {
			REGREL32 header = ReadStruct<REGREL32>();
			string name = ReadSymbolString(header.Header);

			Data = new RegRel32Instance() {
				Header = header,
				Name = name
			};
		}
	}
}
