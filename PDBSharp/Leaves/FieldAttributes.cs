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
	public class FieldAttributes
	{
		private readonly UInt16 fldAttr;
		public FieldAttributes(UInt16 fldAttr) {
			this.fldAttr = fldAttr;
		}

		public byte AccessProtection => (byte)(fldAttr & 3);
		public MethodProperties MethodProperties => (MethodProperties)((fldAttr >> 2) & 7);
		public bool IsPseudo => ((fldAttr >> 5) & 1) == 1;
		public bool IsNoInherit => ((fldAttr >> 6) & 1) == 1;
		public bool IsNoConstruct => ((fldAttr >> 7) & 1) == 1;
		public bool IsCompGenX => ((fldAttr >> 8) & 1) == 1;
	}
}
