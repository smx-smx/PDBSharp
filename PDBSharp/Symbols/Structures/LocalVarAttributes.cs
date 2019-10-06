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
	public struct LocalVarAttributes
	{
		/// <summary>
		/// First code address where var is live
		/// </summary>
		public UInt32 Offset;

		public UInt16 Segment;
		public LocalVarFlags Flags;
	}
}
