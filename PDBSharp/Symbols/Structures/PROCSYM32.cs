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
	public struct ProcSym32Instance
	{
		public PROCSYM32 Header;
		public string Name;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PROCSYM32
	{
		public SymbolHeader Header;
		public UInt32 Parent;
		public UInt32 End;
		public UInt32 Next;
		public UInt32 Length;
		public UInt32 DebugStartOffset;
		public UInt32 DebugEndOffset;
		public UInt32 TypeIndex;
		public UInt32 Offset;
		public UInt16 Segment;
		public CV_PROCFLAGS Flags;
	}

	public class ProcSym32Reader : ReaderBase
	{
		public readonly ProcSym32Instance Data;
		public ProcSym32Reader(Stream stream) : base(stream) {
			PROCSYM32 header = ReadStruct<PROCSYM32>();
			string name = ReadSymbolString(header.Header);

			Data = new ProcSym32Instance() {
				Header = header,
				Name = name
			};
		}
	}
}
