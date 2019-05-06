#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;

namespace Smx.PDBSharp.Leaves
{
	[Flags]
	public enum FunctionAttributes : byte
	{
		CxxReturnUdt = 1 << 0,
		IsIstanceConstructor = 1 << 1,
		IsVirtualInstanceConstructor = 1 << 2,
	}
}