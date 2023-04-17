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
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_VFUNCTAB
{
	public class Data : ILeafData {
		public ILeafResolver? PointerType { get; set; }

		public Data(ILeafResolver? pointerType) {
			PointerType = pointerType;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data {  get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}


		public ILeafData? GetData() => Data;

		public void Read() {
			TypeDataReader r = CreateReader();

			r.ReadUInt16(); //padding
			var PointerType = r.ReadIndexedType32Lazy();
			
			Data = new Data(
				pointerType: PointerType
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_VFUNCTAB);
			w.WriteUInt16(0x00);
			w.WriteIndexedType(data.PointerType);
			w.WriteHeader();
		}
	}
}
