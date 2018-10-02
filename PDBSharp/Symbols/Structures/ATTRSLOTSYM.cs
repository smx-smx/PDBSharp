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
	public struct AttrSlotSymInstance
	{
		public ATTRSLOTSYM Header;
		public string Name;
	}

	public struct ATTRSLOTSYM
	{
		public UInt32 SlotIndex;
		public UInt32 TypeIndex;
		public LocalVarAttributes Attributes;
	}

	public class AttrSlotSymReader : SymbolReaderBase
	{
		public readonly AttrSlotSymInstance Data;

		public AttrSlotSymReader(Stream stream) : base(stream) {
			ATTRSLOTSYM header = ReadStruct<ATTRSLOTSYM>();
			string name = ReadSymbolString(Header);

			Data = new AttrSlotSymInstance() {
				Header = header,
				Name = name
			};
		}
	}
}
