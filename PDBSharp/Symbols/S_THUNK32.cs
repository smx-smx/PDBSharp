#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Thunks;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_THUNK32
{
	public class Data : ISymbolData {
		public UInt32 ParentOffset {  get; set; }
		public ISymbolResolver? Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 NextOffset { get; set; }
		public ISymbolResolver? Next { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public UInt16 ThunkLength { get; set; }
		public ThunkType ThunkType { get; set; }
		public string Name { get; set; }

		public IThunk Thunk { get; set; }

		public Data(uint parentOffset, ISymbolResolver? parent, uint end, uint nextOffset, ISymbolResolver? next, uint offset, ushort segment, ushort thunkLength, ThunkType thunkType, string name, IThunk thunk) {
			ParentOffset = parentOffset;
			Parent = parent;
			End = end;
			NextOffset = nextOffset;
			Next = next;
			Offset = offset;
			Segment = segment;
			ThunkLength = thunkLength;
			ThunkType = thunkType;
			Name = name;
			Thunk = thunk;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;


		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();

			var ParentOffset = r.ReadUInt32();
			var Parent = r.ReadSymbol(ParentOffset);

			var End = r.ReadUInt32();

			var NextOffset = r.ReadUInt32();
			var Next = r.ReadSymbol(NextOffset);

			var Offset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var ThunkLength = r.ReadUInt16();
			var ThunkType = r.ReadEnum<ThunkType>();
			var Name = r.ReadSymbolString();
			var Thunk = r.ReadThunk(ThunkType);

			Data = new Data(
				parentOffset: ParentOffset,
				parent: Parent,
				end: End,
				nextOffset: NextOffset,
				next: Next,
				offset: Offset,
				segment: Segment,
				thunkLength: ThunkLength,
				thunkType: ThunkType,
				name: Name,
				thunk: Thunk
			);
		}

		public void Write() {
			/*
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_THUNK32);
			w.WriteUInt32(data.ParentOffset);
			w.WriteUInt32(data.End);
			w.WriteUInt32(data.NextOffset);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.Segment);
			w.WriteUInt16(data.ThunkLength);
			w.Write<ThunkType>(data.ThunkType);
			w.WriteSymbolString(data.Name);
			data.Thunk.Write(w);

			w.WriteHeader();
			*/
		}
	}
}
