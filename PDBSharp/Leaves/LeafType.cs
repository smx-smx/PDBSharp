#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Leaves
{
	public enum LeafType : UInt16
	{
		LF_NUMERIC      =    0x8000,
		LF_CHAR         =    0x8000,
		LF_SHORT        =    0x8001,
		LF_USHORT       =    0x8002,
		LF_LONG         =    0x8003,
		LF_ULONG        =    0x8004,
		LF_REAL32       =    0x8005,
		LF_REAL64       =    0x8006,
		LF_REAL80       =    0x8007,
		LF_REAL128      =    0x8008,
		LF_QUADWORD     =    0x8009,
		LF_UQUADWORD    =    0x800a,
		LF_REAL48       =    0x800b,
		LF_COMPLEX32    =    0x800c,
		LF_COMPLEX64    =    0x800d,
		LF_COMPLEX80    =    0x800e,
		LF_COMPLEX128   =    0x800f,
		LF_VARSTRING    =    0x8010,
		LF_OCTWORD      =    0x8017,
		LF_UOCTWORD     =    0x8018
	}
}
