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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Leaves.LF_USHORT
{
	public class Data : ILeafData {
		public UInt16 Value { get; set; }
		public Data(ushort value) {
			Value = value;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public long Read() {
			TypeDataReader r = CreateReader();
			var Value = r.ReadUInt16();
			Data = new Data(
				value: Value
			);
			
			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_USHORT);
			w.WriteUInt16(data.Value);
			w.WriteHeader();
		}
	}
}
