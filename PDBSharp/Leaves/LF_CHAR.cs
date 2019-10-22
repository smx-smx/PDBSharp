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
	class LF_CHAR : LeafBase
	{
		public sbyte Value { get; set; }

		public LF_CHAR(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Value = (sbyte)r.ReadByte();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_CHAR);
			w.WriteByte((byte)Value);
			w.WriteHeader();
		}
	}
}
