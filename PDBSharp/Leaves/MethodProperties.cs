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
	public enum MethodProperties : byte
	{
		Vanilla	= 0x00,
		Virtual = 0x01,
		Static  = 0x02,
		Friend  = 0x03,
		Intro   = 0x04,
		PureVirt = 0x05,
		PureIntro = 0x06
	}
}