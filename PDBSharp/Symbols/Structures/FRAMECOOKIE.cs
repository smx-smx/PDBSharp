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
	public enum CookieType : byte
	{
		COPY = 0,
		XOR_SP,
		XOR_BP,
		XOR_R13
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct FRAMECOOKIE
	{
		public SymbolHeader Header;
		public UInt32 Offset;
		public UInt16 RegisterIndex;
		public CookieType CookieType;
		public byte flags;
	}

	public class FrameCookieReader : ReaderBase
	{
		public readonly FRAMECOOKIE Data;
		public FrameCookieReader(Stream stream) : base(stream) {
			Data = ReadStruct<FRAMECOOKIE>();
		}
	}
}
