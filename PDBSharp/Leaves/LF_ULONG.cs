#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_ULONG : LeafBase
	{
		public UInt32 Value { get; set; }
		public LF_ULONG(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Value = r.ReadUInt32();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_ULONG);
			w.WriteUInt32(Value);
			w.WriteHeader();
		}
	}
}
