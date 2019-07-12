#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp.Symbols.Structures
{
	[Flags]
	public enum CompileSymFlagsEnum : UInt32
	{
		HasPCode = 1 << 8,
		Is32Bit = 1 << 20
	}

	public class CompileSymFlags
	{
		public static explicit operator CompileSymFlagsEnum(CompileSymFlags flags) => flags.flags;

		private readonly CompileSymFlagsEnum flags;

		public CompileSymFlags(UInt32 flags) {
			this.flags = (CompileSymFlagsEnum)flags;
		}

		//0
		public byte LanguageIndex => (byte)((uint)flags & 0xFF);
		//8 [+8]
		public bool HasPCode => flags.HasFlag(CompileSymFlagsEnum.HasPCode);
		//9 [+1]
		public byte FloatPrecision => (byte)(((uint)flags >> 9) & 3);
		//11 [+2]
		public byte FloatPackage => (byte)(((uint)flags >> 11) & 3);
		//13 [+2]
		public byte AmbientDataModel => (byte)(((uint)flags >> 13) & 7);
		//16 [+3]
		public byte AmbientDataCode => (byte)(((uint)flags >> 16) & 7);
		//19 [+3]
		public bool Is32Bit => flags.HasFlag(CompileSymFlagsEnum.Is32Bit);
		//20 [+1]

	}
}
