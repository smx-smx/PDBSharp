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
	public enum CV_LVARFLAGS : UInt16
	{
		IsParam = 1 << 0,
		AddressIsTaken = 1 << 1,
		IsCompilerGenerated = 1 << 2,
		IsAggregate = 1 << 3,
		IsAggregated = 1 << 4,
		IsAliased = 1 << 5,
		IsAlias = 1 << 6,
		IsReturnValue = 1 << 7,
		IsOptimizedOut = 1 << 8,
		IsEnregisteredGlobal = 1 << 9,
		IsEnregisteredStatic = 1 << 10
	}
}
