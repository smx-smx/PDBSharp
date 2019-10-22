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
	public class LF_BITFIELD : LeafBase
	{
		public ILeafContainer Type { get; set; }
		public byte Length { get; set; }
		public byte Position { get; set; }

		public LF_BITFIELD(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Type = r.ReadIndexedType32Lazy();
			Length = r.ReadByte();
			Position = r.ReadByte();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_BITFIELD);
			w.WriteIndexedType(Type);
			w.WriteByte(Length);
			w.WriteByte(Position);
			w.WriteHeader();
		}
	}
}
