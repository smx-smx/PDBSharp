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
	public struct ObjNameSymInstance
	{
		public OBJNAMESYM Header;
		public string Name;
	}

	public struct OBJNAMESYM
	{
		public SymbolHeader Header;
		public UInt32 Signature;
	}

	public class ObjNameSymReader : ReaderBase
	{
		public readonly ObjNameSymInstance Data;

		public ObjNameSymReader(Stream stream) : base(stream) {
			OBJNAMESYM header = ReadStruct<OBJNAMESYM>();
			string name = ReadSymbolString(header.Header);

			Data = new ObjNameSymInstance() {
				Header = header,
				Name = name
			};
		}
	}
}
