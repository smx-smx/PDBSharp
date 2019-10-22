#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_NESTTYPE : LeafBase
	{
		public ILeafContainer NestedTypeDef { get; set; }
		public string Name { get; set; }

		public LF_NESTTYPE(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			r.ReadUInt16(); //padding
			NestedTypeDef = r.ReadIndexedType32Lazy();
			Name = r.ReadCString();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_NESTTYPE);
			w.WriteUInt16(0x00);
			w.WriteIndexedType(NestedTypeDef);
			w.WriteCString(Name);
			w.WriteHeader();
		}
	}
}
