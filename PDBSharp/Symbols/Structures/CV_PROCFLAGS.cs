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
	public enum CV_PROCFLAGS : byte
	{
		/// <summary>
		/// frame pointer present
		/// </summary>
		NoFPO = 1 << 0,
		/// <summary>
		/// interrupt return
		/// </summary>
		Int = 1 << 1,
		/// <summary>
		/// far return
		/// </summary>
		Far = 1 << 2,
		/// <summary>
		/// function does not return
		/// </summary>
		Never = 1 << 3,
		/// <summary>
		/// label isn't fallen into
		/// </summary>
		NotReached = 1 << 4,
		/// <summary>
		/// custom calling convention
		/// </summary>
		CustCall = 1 << 5,
		/// <summary>
		/// function marked as noinline
		/// </summary>
		NoInline = 1 << 6,
		/// <summary>
		/// function has debug information for optimized code
		/// </summary>
		OptDbgInfo = 1 << 7

	}

}
