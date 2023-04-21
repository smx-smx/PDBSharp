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

namespace Smx.PDBSharp.Leaves.LF_BITFIELD
{
	public class Data : ILeafData {
		public ILeafResolver? Type { get; set; }
		public byte Length { get; set; }
		public byte Position { get; set; }

		public Data(ILeafResolver? type, byte length, byte position) {
			Type = type;
			Length = length;
			Position = position;
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
			var Type = r.ReadIndexedType32Lazy();
			var Length = r.ReadByte();
			var Position = r.ReadByte();

			Data = new Data(
				type: Type,
				length: Length,
				position: Position
			);

			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_BITFIELD);
			w.WriteIndexedType(data.Type);
			w.WriteByte(data.Length);
			w.WriteByte(data.Position);
			w.WriteHeader();
		}
	}
}
