#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;

namespace Smx.PDBSharp.Symbols.Structures
{
	[Flags]
	public enum CompileSym2FlagsEnum : UInt32
	{
		CompiledForEC = 1 << 8,
		NoDebugInfo = 1 << 9,
		HasLTCG = 1 << 10,
		NoDataAlign = 1 << 11,
		IsManaged = 1 << 12,
		HasSecurityChecks = 1 << 13,
		HasHotPatch = 1 << 14,
		ConvertedWithCVTCIL = 1 << 15,
		IsMSILMModule = 1 << 16
	}

	public class CompileSym2Flags
	{
		public static explicit operator CompileSym2FlagsEnum(CompileSym2Flags flags) => flags.flags;

		private readonly CompileSym2FlagsEnum flags;
		public CompileSym2Flags(UInt32 flags) {
			this.flags = (CompileSym2FlagsEnum)flags;
		}

		public byte LanguageIndex => (byte)((uint)flags & 0xFF);
		public bool CompiledForEC => flags.HasFlag(CompileSym2FlagsEnum.CompiledForEC);
		public bool NoDebugInfo => flags.HasFlag(CompileSym2FlagsEnum.NoDebugInfo);
		public bool HasLTCG => flags.HasFlag(CompileSym2FlagsEnum.HasLTCG);
		public bool NoDataAlign => flags.HasFlag(CompileSym2FlagsEnum.NoDataAlign);
		public bool IsManaged => flags.HasFlag(CompileSym2FlagsEnum.IsManaged);
		public bool HasSecurityChecks => flags.HasFlag(CompileSym2FlagsEnum.HasSecurityChecks);
		public bool HasHotPatch => flags.HasFlag(CompileSym2FlagsEnum.HasHotPatch);
		public bool ConvertedWithCVTCIL => flags.HasFlag(CompileSym2FlagsEnum.ConvertedWithCVTCIL);
		public bool IsMSILMModule => flags.HasFlag(CompileSym2FlagsEnum.IsMSILMModule);
	}
}
