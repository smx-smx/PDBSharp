#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_VBCLASS)]
	public class LF_VBCLASS : TypeDataReader
	{
		public readonly FieldAttributes Attributes;
		public readonly UInt32 VirtualBaseClassTypeIndex;
		public readonly UInt32 VirtualBasePointerTypeIndex;


		public LF_VBCLASS(Stream stream) : base(stream) {
			Attributes = new FieldAttributes(Reader.ReadUInt16());
			VirtualBaseClassTypeIndex = Reader.ReadUInt32();
			VirtualBasePointerTypeIndex = Reader.ReadUInt32();


			//virtual base pointer offset from address point
			var dyn1 = ReadVaryingType(out uint dynSize1);
			//virtual base offset from vbtable
			var dyn2 = ReadVaryingType(out uint dynSize2);
		}
	}

	[LeafReader(LeafType.LF_IVBCLASS)]
	public class LF_IVBCLASS : LF_VBCLASS
	{
		public LF_IVBCLASS(Stream stream) : base(stream) {
		}
	}
}
