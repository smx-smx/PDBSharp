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
	public enum FrameProcSymFlagsEnum : UInt32
	{
		HasAlloca = 1 << 0,
		HasSetJmp = 1 << 1,
		HasLongJmp = 1 << 2,
		HasInlineAsm = 1 << 3,
		HasEH = 1 << 4,
		HasInlineSpec = 1 << 5,
		HasSEH = 1 << 6,
		IsNaked = 1 << 7,
		HasSecurityChecks = 1 << 8, // /GS
		HasAsyncEH = 1 << 9, // /EHa
		GSNoStackOrdering = 1 << 10,
		WasInlined = 1 << 11,
		HasGSCheck = 1 << 12,
		HasSafeBuffers = 1 << 13,
		//skip 14-15, 16-17
		HasPogo = 1 << 18, //PGO/PGU
		HasValidPogoCounts = 1 << 19,
		OptimizedForSpeed = 1 << 20,
		HasCFGChecks = 1 << 21,
		HasCFWChecks = 1 << 22
	}

	public class FrameProcSymFlags
	{
		public static explicit operator FrameProcSymFlagsEnum(FrameProcSymFlags flags) => flags.flags;

		private FrameProcSymFlagsEnum flags;
		public FrameProcSymFlags(UInt32 flags) {
			this.flags = (FrameProcSymFlagsEnum)flags;
		}

		public bool HasAlloca => flags.HasFlag(FrameProcSymFlagsEnum.HasAlloca);
		public bool HasSetJmp => flags.HasFlag(FrameProcSymFlagsEnum.HasSetJmp);
		public bool HasLongJmp => flags.HasFlag(FrameProcSymFlagsEnum.HasLongJmp);
		public bool HasInlineAsm => flags.HasFlag(FrameProcSymFlagsEnum.HasInlineAsm);
		public bool HasEH => flags.HasFlag(FrameProcSymFlagsEnum.HasEH);
		public bool HasInlineSpec => flags.HasFlag(FrameProcSymFlagsEnum.HasInlineSpec);
		public bool HasSEH => flags.HasFlag(FrameProcSymFlagsEnum.HasSEH);
		public bool IsNaked => flags.HasFlag(FrameProcSymFlagsEnum.IsNaked);
		public bool HasSecurityChecks => flags.HasFlag(FrameProcSymFlagsEnum.HasSecurityChecks);
		public bool HasAsyncEH => flags.HasFlag(FrameProcSymFlagsEnum.HasAsyncEH);
		public bool GSNoStackOrdering => flags.HasFlag(FrameProcSymFlagsEnum.GSNoStackOrdering);
		public bool WasInlined => flags.HasFlag(FrameProcSymFlagsEnum.WasInlined);
		public bool HasGSCheck => flags.HasFlag(FrameProcSymFlagsEnum.HasGSCheck);
		public bool HasSafeBuffers => flags.HasFlag(FrameProcSymFlagsEnum.HasSafeBuffers);

		public byte EncodedLocalBasePointer => (byte)(((uint)flags >> 13) & 2);
		public byte EncodedParamBasePointer => (byte)(((uint)flags >> 15) & 2);

		public bool HasPogo => flags.HasFlag(FrameProcSymFlagsEnum.HasPogo);
		public bool HasValidPogoCounts => flags.HasFlag(FrameProcSymFlagsEnum.HasValidPogoCounts);
		public bool OptimizedForSpeed => flags.HasFlag(FrameProcSymFlagsEnum.OptimizedForSpeed);
		public bool HasCFGChecks => flags.HasFlag(FrameProcSymFlagsEnum.HasCFGChecks);
		public bool HasCFWChecks => flags.HasFlag(FrameProcSymFlagsEnum.HasCFWChecks);
	}
}
