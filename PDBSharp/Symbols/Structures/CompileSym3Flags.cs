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
	internal enum CompileSym3FlagsEnum : UInt32
	{
		CompiledForEC = 1 << 8,
		NoDebugInfo = 1 << 9,
		HasLTCG = 1 << 10,
		NoDataAlign = 1 << 11,
		IsManaged = 1 << 12,
		HasSecurityChecks = 1 << 13,
		HasHotPatch = 1 << 14,
		ConvertedWithCVTCIL = 1 << 15,
		IsMSILMModule = 1 << 16,
		HasSDL = 1 << 17,
		HasPGO = 1 << 18,
		IsExpModule = 1 << 19
	}

	public class CompileSym3Flags
	{
		private readonly CompileSym3FlagsEnum flags;
		public CompileSym3Flags(UInt32 flags) {
			this.flags = (CompileSym3FlagsEnum)flags;
		}

		public byte LanguageIndex => (byte)((uint)flags & 8);
		public bool CompiledForEC => flags.HasFlag(CompileSym3FlagsEnum.CompiledForEC);
		public bool NoDebugInfo => flags.HasFlag(CompileSym3FlagsEnum.NoDebugInfo);
		public bool HasLTCG => flags.HasFlag(CompileSym3FlagsEnum.HasLTCG);
		public bool NoDataAlign => flags.HasFlag(CompileSym3FlagsEnum.NoDataAlign);
		public bool IsManaged => flags.HasFlag(CompileSym3FlagsEnum.IsManaged);
		public bool HasSecurityChecks => flags.HasFlag(CompileSym3FlagsEnum.HasSecurityChecks);
		public bool HasHotPatch => flags.HasFlag(CompileSym3FlagsEnum.HasHotPatch);
		public bool ConvertedWithCVTCIL => flags.HasFlag(CompileSym3FlagsEnum.ConvertedWithCVTCIL);
		public bool IsMSILMModule => flags.HasFlag(CompileSym3FlagsEnum.IsMSILMModule);
		public bool HasSDL => flags.HasFlag(CompileSym3FlagsEnum.HasSDL);
		public bool HasPGO => flags.HasFlag(CompileSym3FlagsEnum.HasPGO);
		public bool IsExpModule => flags.HasFlag(CompileSym3FlagsEnum.IsExpModule);
	}
}
