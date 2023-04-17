#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_METHOD
{
	public class Data : ILeafData {
		public UInt16 NumberOfOccurrences { get; set; }
		public ILeafResolver? MethodListRecord { get; set; }
		public string Name { get; set; }

		public Data(ushort numberOfOccurrences, ILeafResolver? methodListRecord, string name) {
			NumberOfOccurrences = numberOfOccurrences;
			MethodListRecord = methodListRecord;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var NumberOfOccurrences = r.ReadUInt16();
			var MethodListRecord = r.ReadIndexedType32Lazy();
			var Name = r.ReadCString();

			Data = new Data(
				numberOfOccurrences: NumberOfOccurrences,
				methodListRecord: MethodListRecord,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_METHOD);
			w.WriteUInt16(data.NumberOfOccurrences);
			w.WriteIndexedType(data.MethodListRecord);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}
	}
}
