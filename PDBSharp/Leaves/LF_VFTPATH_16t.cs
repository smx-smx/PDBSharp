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

namespace Smx.PDBSharp.Leaves.LF_VFTPATH_16t
{
	public class Data : ILeafData
	{
		public UInt16 NumElements { get; set; }
		public ILeafResolver? Bases { get; set; }

		public Data(ushort numElements, ILeafResolver? bases) {
			NumElements = numElements;
			Bases = bases;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public long Read() {
			TypeDataReader r = CreateReader();
			var NumElements = r.ReadUInt16();
			var Bases = r.ReadIndexedType16Lazy();

			Data = new Data(
				numElements: NumElements,
				bases: Bases
			);
			
			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_VFTPATH_16t);
			w.WriteUInt16(data.NumElements);
			w.WriteIndexedType16(data.Bases);
			w.WriteHeader();
		}
	}
}
