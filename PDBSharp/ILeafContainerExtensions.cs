#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	public static class ILeafContainerExtensions
	{
		public static long LeafDecimalValue(this LeafContext leafContext) {
			switch (leafContext.Data) {
				case Leaves.LF_CHAR.Data lfChar:
					return lfChar.Value;
				case Leaves.LF_SHORT.Data lfShort:
					return lfShort.Value;
				case Leaves.LF_USHORT.Data lfUShort:
					return lfUShort.Value;
				case Leaves.LF_LONG.Data lfLong:
					return lfLong.Value;
				case Leaves.LF_ULONG.Data lfULong:
					return lfULong.Value;
				default:
					throw new NotSupportedException($"Cannot get numeric decimal value for leaf {leafContext.Type}");
			}
		}
	}
}
