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
using Smx.PDBSharp.Symbols.S_SEPCODE;
using Smx.PDBSharp.Symbols.Structures;

namespace Smx.PDBSharp.Symbols.ProcSym32
{
	public class Data : ISymbolData {
		public UInt32 ParentOffset { get; set; }
		public UInt32 End { get;set; }
		public UInt32 NextOffset { get;set; }
		public UInt32 Length { get;set; }
		public UInt32 DebugStartOffset { get;set; }
		public UInt32 DebugEndOffset { get;set; }
		public ILeafResolver? Type { get;set; }
		public UInt32 Offset { get;set; }
		public short Segment { get;set; }
		public CV_PROCFLAGS Flags { get;set; }
		public string Name { get;set; }
		public ISymbolResolver? ParentSymbol { get;set; }
		public ISymbolResolver? NextSymbol { get;set; }

		public Data(uint parentOffset, uint end, uint nextOffset, uint length, uint debugStartOffset, uint debugEndOffset, ILeafResolver? type, uint offset, short segment, CV_PROCFLAGS flags, string name, ISymbolResolver? parentSymbol, ISymbolResolver? nextSymbol) {
			ParentOffset = parentOffset;
			End = end;
			NextOffset = nextOffset;
			Length = length;
			DebugStartOffset = debugStartOffset;
			DebugEndOffset = debugEndOffset;
			Type = type;
			Offset = offset;
			Segment = segment;
			Flags = flags;
			Name = name;
			ParentSymbol = parentSymbol;
			NextSymbol = nextSymbol;
		}

		public override string ToString() {
			return $"PROCSYM32(" +
				$"Type={Type?.Ctx}, Name={Name}," +
				$"Segment={Segment}, Offset=0x{Offset.ToString("X")}, Length={Length}" +
				$")";
		}
	}

	public abstract class SerializerBase : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public void Write() {
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() => Data;

		public SerializerBase(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var ParentOffset = r.ReadUInt32();
			var ParentSymbol = r.ReadSymbol(ParentOffset);

			var End = r.ReadUInt32();
			var NextOffset = r.ReadUInt32();
			var NextSymbol = r.ReadSymbol(NextOffset);

			var Length = r.ReadUInt32();
			var DebugStartOffset = r.ReadUInt32();
			var DebugEndOffset = r.ReadUInt32();
			var Type = r.ReadIndexedType32Lazy();
			var Offset = r.ReadUInt32();
			var Segment = r.ReadInt16();
			var Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			var Name = r.ReadSymbolString();

			Data = new Data(
				parentOffset: ParentOffset,
				parentSymbol: ParentSymbol,
				end: End,
				nextOffset: NextOffset,
				nextSymbol: NextSymbol,
				length: Length,
				debugStartOffset: DebugStartOffset,
				debugEndOffset: DebugEndOffset,
				type: Type,
				offset: Offset,
				segment: Segment,
				flags: Flags,
				name: Name
			);
		}

		public void Write(SymbolType symbolType) {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(symbolType);
			w.WriteUInt32(data.ParentOffset);
			w.WriteUInt32(data.End);
			w.WriteUInt32(data.NextOffset);
			w.WriteUInt32(data.Length);
			w.WriteUInt32(data.DebugStartOffset);
			w.WriteUInt32(data.DebugEndOffset);
			w.WriteIndexedType(data.Type);
			w.WriteUInt32(data.Offset);
			w.WriteInt16(data.Segment);
			w.Write<CV_PROCFLAGS>(data.Flags);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
