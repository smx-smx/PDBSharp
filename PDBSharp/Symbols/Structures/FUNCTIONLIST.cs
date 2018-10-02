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
	public struct FUNCTIONLIST
	{
		public UInt32 Count;
	}

	public struct FunctionListInstance
	{
		public FUNCTIONLIST Header;
		public UInt32[] Functions;
	}

	public class FunctionListReader : SymbolReaderBase
	{
		public readonly FunctionListInstance Data;

		public FunctionListReader(Stream stream) : base(stream) {
			FUNCTIONLIST header = ReadStruct<FUNCTIONLIST>();
			UInt32[] functions = new UInt32[header.Count];
			for(int i=0; i<header.Count; i++) {
				functions[i] = Reader.ReadUInt32();
			}

			Data = new FunctionListInstance() {
				Header = header,
				Functions = functions
			};
		}
	}
}
