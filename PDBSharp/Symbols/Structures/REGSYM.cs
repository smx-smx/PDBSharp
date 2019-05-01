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
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp.Symbols.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct REGSYM
	{
		public UInt32 TypeIndex;
		public UInt16 Register;
	}

	public struct RegSymInstance
	{
		public REGSYM Header;
		public string Name;
	}

	public class RegSymReader : SymbolReaderBase
	{
		public readonly RegSymInstance Data;
		public RegSymReader(Stream stream) : base(stream) {
			REGSYM header = ReadStruct<REGSYM>();
			string name = ReadSymbolString(Header);

			Data = new RegSymInstance() {
				Header = header,
				Name = name
			};
		}
	}
}
