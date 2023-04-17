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

namespace Smx.PDBSharp.Leaves.LF_INDEX
{
	public class Data : ILeafData {
		public ILeafResolver? Referenced { get; set; }
		public Data(ILeafResolver? referenced) {
			Referenced = referenced;
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
			var Referenced = r.ReadIndexedType32Lazy();
			Data = new Data(
				referenced: Referenced
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_INDEX);
			w.WriteIndexedType(data.Referenced);
			w.WriteHeader();
		}
	}
}
