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
	public struct COFFGROUPSYM
	{
		public SymbolHeader Header;
		public UInt32 Size;
		public UInt32 Characteristics;
		public UInt32 SymbolOffset;
		public UInt16 SymbolSegment;
	}

	public struct CoffGroupSymInstance
	{
		public COFFGROUPSYM Header;
		public string Name;
	}

	public class CoffGroupSymReader : ReaderBase
	{
		public readonly CoffGroupSymInstance Data;
		public CoffGroupSymReader(Stream stream) : base(stream) {
			COFFGROUPSYM header = ReadStruct<COFFGROUPSYM>();
			string name = ReadSymbolString(header.Header);

			Data = new CoffGroupSymInstance() {
				Header = header,
				Name = name
			};
		}
	}
}
