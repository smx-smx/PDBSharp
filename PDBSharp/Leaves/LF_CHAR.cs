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

namespace Smx.PDBSharp.Leaves.LF_CHAR
{
	public class Data : ILeafData {
		public sbyte Value {  get; set;}
		public Data(sbyte value) {
			Value = value;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}

		public void Read() {
			TypeDataReader r = CreateReader();
			var Value = (sbyte)r.ReadByte();
			Data = new Data(
				value: Value
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_CHAR);
			w.WriteByte((byte)data.Value);
			w.WriteHeader();
		}
	}
}
