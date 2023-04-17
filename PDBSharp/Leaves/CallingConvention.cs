#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
namespace Smx.PDBSharp.Leaves
{
	public enum CallingConvention : byte
	{
		Near_C = 0x00,
		Far_C = 0x01,
		Near_Pascal = 0x02,
		Far_Pascal = 0x03,
		Near_Fast = 0x04,
		Far_Fast = 0x05,
		// unused 0x06
		Near_Std = 0x07,
		Far_Std = 0x08,
		Near_Sys = 0x09,
		Far_Sys = 0x0a,
		Thiscall = 0x0b,
		Mipscall = 0x0c,
		Generic = 0x0d,
		Alphacall = 0x0e,
		PpcCall = 0x0f,
		Shcall = 0x10,
		Armcall = 0x11,
		Am33call = 0x12,
		Tricall = 0x13,
		Sh5call = 0x14,
		M32rcall = 0x15,
		Clrcall = 0x16,
		Inline = 0x17,
		Near_Vector = 0x18
		// reserved 0x19
	}
}
