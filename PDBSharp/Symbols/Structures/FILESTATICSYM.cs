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
	public struct FILESTATICSYM
	{
		public UInt32 TypeIndex;
		public UInt32 ModuleFilenameOffset;
		public CV_LVARFLAGS Flags;
	}

	public struct FileStaticSymInstance
	{
		public FILESTATICSYM Header;
		public string Name;
	}

	public class FileStaticSymReader : SymbolReaderBase
	{
		public readonly FileStaticSymInstance Data;
		public FileStaticSymReader(Stream stream) : base(stream) {
			FILESTATICSYM header = ReadStruct<FILESTATICSYM>();
			string name = ReadSymbolString(Header);

			Data = new FileStaticSymInstance() {
				Header = header,
				Name = name
			};
		}
	}
}
