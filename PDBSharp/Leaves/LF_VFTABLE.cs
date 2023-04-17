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
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_VFTABLE
{
	public class Data : ILeafData {
		public ILeafResolver? Type { get; set; }
		public ILeafResolver? BaseVfTable { get; set; }
		public UInt32 OffsetInObjectLayout { get; set; }
		/// <summary>
		/// Size in Bytes
		/// </summary>
		public UInt32 NamesSize { get; set; }
		public string[] Names { get; set; }

		public Data(ILeafResolver? type, ILeafResolver? baseVfTable, uint offsetInObjectLayout, uint namesSize, string[] names) {
			Type = type;
			BaseVfTable = baseVfTable;
			OffsetInObjectLayout = offsetInObjectLayout;
			NamesSize = namesSize;
			Names = names;
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

			var Type = r.ReadIndexedType32Lazy();
			var BaseVfTable = r.ReadIndexedType32Lazy();
			var OffsetInObjectLayout = r.ReadUInt32();
			var NamesSize = r.ReadUInt32();

			List<string> lstNames = new List<string>();

			uint read = 0;
			long savedPos = stream.Position;
			while (read < NamesSize) {
				lstNames.Add(r.ReadCString());
				read += (uint)(stream.Position - savedPos);
				savedPos = stream.Position;
			}
			var Names = lstNames.ToArray();

			Data = new Data(
				type: Type,
				baseVfTable: BaseVfTable,
				offsetInObjectLayout: OffsetInObjectLayout,
				namesSize: NamesSize,
				names: Names
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_VFTABLE);
			w.WriteIndexedType(data.Type);
			w.WriteIndexedType(data.BaseVfTable);
			w.WriteUInt32(data.OffsetInObjectLayout);
			w.WriteUInt32(data.NamesSize);
			foreach (string name in data.Names) {
				w.WriteCString(name);
			}
			w.WriteHeader();
		}
	}
}
