#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_ENVBLOCK)]
	public class S_ENVBLOCK : SymbolDataReader
	{
		public readonly string[] Data;

		public S_ENVBLOCK(PDBFile pdb, Stream stream) : base(pdb, stream) {
			byte flags = ReadByte(); //fEC -> reserved (1 bit)

			List<string> strLst = new List<string>(); ;
			while (Stream.Position < Stream.Length) {
				string str = ReadSymbolString();
				if (str.Length == 0)
					break;
				strLst.Add(str);
			}

			Data = strLst.ToArray();
		}
	}
}
