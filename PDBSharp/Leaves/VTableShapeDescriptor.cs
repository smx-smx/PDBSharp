#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
namespace Smx.PDBSharp.Leaves
{
	public enum VTableShapeDescriptor : byte
	{
		Near = 0x00,
		Far = 0x01,
		Thin = 0x02,
		Outer = 0x03,
		Meta = 0x04,
		Near32 = 0x05,
		Far32 = 0x06,
		//Unused = 0x07
	}
}