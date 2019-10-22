#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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
		public static long LeafDecimalValue(this ILeafContainer leaf) {
			switch (leaf.Data) {
				case LF_CHAR lfChar:
					return lfChar.Value;
				case LF_SHORT lfShort:
					return lfShort.Value;
				case LF_USHORT lfUShort:
					return lfUShort.Value;
				case LF_LONG lfLong:
					return lfLong.Value;
				case LF_ULONG lfULong:
					return lfULong.Value;
				default:
					throw new NotSupportedException($"Cannot get numeric decimal value for leaf {leaf.Type}");
			}
		}
	}
}
