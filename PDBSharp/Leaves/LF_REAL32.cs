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

namespace Smx.PDBSharp.Leaves.LF_REAL32
{
	public class Data : ILeafData {
		public float Value { get; set; }
		public Data(float value) {
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
			var Value = r.ReadSingle();
			Data = new Data(
				value: Value
			);
			
			return r.Position;
		}

		public void Write() {
			throw new NotImplementedException();
		}

		public void Write(PDBFile pdb, Stream stream) {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_REAL32);
			w.WriteSingle(data.Value);
			w.WriteHeader();
		}
	}
}
