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
	public struct CV_PROCFLAGS
	{
		private byte flag;
	}

	public struct ManProcSymInstance
	{
		public MANPROCSYM Header;
		public string Name;
	}

	public class ManProcSymReader : ReaderBase
	{
		public readonly ManProcSymInstance Data;

		public ManProcSymReader(Stream stream) : base(stream) {
			var header = ReadStruct<MANPROCSYM>();
			string name = ReadCString();
			Data = new ManProcSymInstance() {
				Header = header,
				Name = name
			};
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MANPROCSYM
	{
		public SymbolHeader Header;
		/// <summary>
		/// Parent Symbol
		/// </summary>
		public UInt32 Parent;
		/// <summary>
		/// End of block
		/// </summary>
		public UInt32 End;
		/// <summary>
		/// Next Symbol
		/// </summary>
		public UInt32 Next;

		public UInt32 ProcLength;

		public UInt32 DebugStartOffset;
		public UInt32 DebugEndOffset;

		public UInt32 ComToken;
		public UInt32 Offset;

		public UInt16 Segment;

		public CV_PROCFLAGS Flags;
		public UInt16 ReturnRegister;
	}

}
