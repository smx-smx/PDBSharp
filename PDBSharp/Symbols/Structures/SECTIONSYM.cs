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
	public struct SECTIONSYM
	{
		public SymbolHeader Header;
		public UInt16 SectionIndex;
		public sbyte Alignment;
		private sbyte reserved_0;
		public UInt32 RVA;
		public UInt32 Size;
		public UInt32 Characteristics;
	}

	public struct SectionSymInstance
	{
		public SECTIONSYM Header;
		public string Name;
	}

	public class SectionSymReader : ReaderBase
	{
		public readonly SectionSymInstance Data;
		public SectionSymReader(Stream stream) : base(stream) {
			SECTIONSYM header = ReadStruct<SECTIONSYM>();
			string name = ReadSymbolString(header.Header);

			Data = new SectionSymInstance() {
				Header = header,
				Name = name
			};
		}

	}
}
