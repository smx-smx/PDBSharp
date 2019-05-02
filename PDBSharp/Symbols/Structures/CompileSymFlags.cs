#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;

namespace Smx.PDBSharp.Symbols.Structures
{
	[Flags]
	internal enum CompileSymFlagsEnum : UInt32
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

	public class CompileSymFlags
	{
		private readonly CompileSymFlagsEnum flags;
		public CompileSymFlags(UInt32 flags) {
			this.flags = (CompileSymFlagsEnum)flags;
		}

		public byte LanguageIndex => (byte)((uint)flags & 8);
		public bool CompiledForEC => flags.HasFlag(CompileSymFlagsEnum.CompiledForEC);
		public bool NoDebugInfo => flags.HasFlag(CompileSymFlagsEnum.NoDebugInfo);
		public bool HasLTCG => flags.HasFlag(CompileSymFlagsEnum.HasLTCG);
		public bool NoDataAlign => flags.HasFlag(CompileSymFlagsEnum.NoDataAlign);
		public bool IsManaged => flags.HasFlag(CompileSymFlagsEnum.IsManaged);
		public bool HasSecurityChecks => flags.HasFlag(CompileSymFlagsEnum.HasSecurityChecks);
		public bool HasHotPatch => flags.HasFlag(CompileSymFlagsEnum.HasHotPatch);
		public bool ConvertedWithCVTCIL => flags.HasFlag(CompileSymFlagsEnum.ConvertedWithCVTCIL);
		public bool IsMSILMModule => flags.HasFlag(CompileSymFlagsEnum.IsMSILMModule);
	}
}
