#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public struct ConstSymInstance
	{
		public CONSTSYM Header;
		public ILeaf Value;
		public string Name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CONSTSYM
	{
		public SymbolHeader Header;
		public UInt32 TypeIndex;
		public LeafType Value;
	}

	public class ConstSymReader : ReaderBase
	{
		public readonly ConstSymInstance Data;
		public ConstSymReader(Stream stream) : base(stream) {
			CONSTSYM header = ReadStruct<CONSTSYM>();

			ILeaf value = ReadNumericLeaf(header.Value);
			string name = ReadSymbolString(header.Header);

			Data = new ConstSymInstance() {
				Header = header,
				Value = value,
				Name = name
			};
		}
	}
}
