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

namespace Smx.PDBSharp.Leaves.LF_ARRAY
{
	public class Data : ILeafData {
		public ILeafResolver? ElementType { get; set; }
		public ILeafResolver? IndexingType { get; set; }

		public ILeafResolver? Size { get; set; }

		public string Name { get; set; }

		public Data(ILeafResolver? elementType, ILeafResolver? indexingType, ILeafResolver? size, string name) {
			ElementType = elementType;
			IndexingType = indexingType;
			Size = size;
			Name = name;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">used to indicate the size of the type index</typeparam>
	public class Serializer<T> : LeafBase, ILeafSerializer where T : unmanaged
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}
		
		public void Read() {
			TypeDataReader r = CreateReader();

			var ElementType = r.ReadIndexedTypeLazy<T>();
			var IndexingType = r.ReadIndexedTypeLazy<T>();
			var Size = r.ReadVaryingType(out uint dataSize);
			var Name = r.ReadCString();

			Data = new Data(
				elementType: ElementType,
				indexingType: IndexingType,
				size: Size,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_ARRAY);
			w.WriteIndexedType(data.ElementType);
			w.WriteIndexedType(data.IndexingType);
			w.WriteVaryingType(data.Size);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}
	}
}
