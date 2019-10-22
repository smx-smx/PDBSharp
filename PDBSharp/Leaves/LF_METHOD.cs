#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_METHOD : LeafBase
	{
		public UInt16 NumberOfOccurrences { get; set; }
		public ILeafContainer MethodListRecord { get; set; }

		public string Name { get; set; }

		public LF_METHOD(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			NumberOfOccurrences = r.ReadUInt16();
			MethodListRecord = r.ReadIndexedType32Lazy();
			Name = r.ReadCString();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_METHOD);
			w.WriteUInt16(NumberOfOccurrences);
			w.WriteIndexedType(MethodListRecord);
			w.WriteCString(Name);
			w.WriteHeader();
		}
	}
}
