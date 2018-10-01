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
	public struct UDTSYM
	{
		public SymbolHeader Header;
		public UInt32 TypeIndex;
	}

	public struct UdtSymInstance
	{
		public UDTSYM Header;
		public string Name;
	}

	public class UdtSymReader : ReaderBase
	{
		public readonly UdtSymInstance Data;

		public UdtSymReader(Stream stream) : base(stream) {
			UDTSYM header = ReadStruct<UDTSYM>();
			string name = ReadSymbolString(header.Header);

			Data = new UdtSymInstance() {
				Header = header,
				Name = name
			};
		}
	}
}
