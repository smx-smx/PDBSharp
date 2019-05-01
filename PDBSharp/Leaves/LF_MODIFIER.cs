#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	[Flags]
	public enum LFModifierFlags : UInt16
	{
		Const = 1 << 0,
		Volatile = 1 << 1,
		Unaligned = 1 << 2
	}

	[LeafReader(LeafType.LF_MODIFIER)]
	public class LF_MODIFIER : TypeDataReader
	{
		public readonly LFModifierFlags Flags;
		public readonly uint ModifiedType;

		public LF_MODIFIER(Stream stream) : base(stream) {
			ModifiedType = Reader.ReadUInt32();
			Flags = (LFModifierFlags)Reader.ReadUInt16();
		}
	}
}
