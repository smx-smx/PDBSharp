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
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class ManProcData : ISymbolData {
		/// <summary>
		/// Parent Symbol
		/// </summary>
		public ISymbolResolver? Parent {  get; set; }
		public UInt32 ParentOffset { get; set; }
		/// <summary>
		/// End of block
		/// </summary>
		public UInt32 End { get; set; }
		/// <summary>
		/// Next Symbol
		/// </summary>
		public ISymbolResolver? Next { get; set; }
		public UInt32 NextOffset { get; set; }
		public UInt32 ProcLength { get; set; }
		public UInt32 DebugStartOffset { get; set; }
		public UInt32 DebugEndOffset { get; set; }
		public UInt32 ComToken { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public CV_PROCFLAGS Flags { get; set; }
		public UInt16 ReturnRegister { get; set; }
		public string Name { get; set; }

		public ManProcData(ISymbolResolver? parent, uint parentOffset, uint end, ISymbolResolver? next, uint nextOffset, uint procLength, uint debugStartOffset, uint debugEndOffset, uint comToken, uint offset, ushort segment, CV_PROCFLAGS flags, ushort returnRegister, string name) {
			Parent = parent;
			ParentOffset = parentOffset;
			End = end;
			Next = next;
			NextOffset = nextOffset;
			ProcLength = procLength;
			DebugStartOffset = debugStartOffset;
			DebugEndOffset = debugEndOffset;
			ComToken = comToken;
			Offset = offset;
			Segment = segment;
			Flags = flags;
			ReturnRegister = returnRegister;
			Name = name;
		}
	}

	public abstract class ManProcSymSerializerBase : SymbolSerializerBase
	{
		public ManProcData? Data { get; set; }

		public ManProcSymSerializerBase(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();

			var ParentOffset = r.ReadUInt32();
			var Parent = r.ReadSymbol(ParentOffset);
			var End = r.ReadUInt32();
			var NextOffset = r.ReadUInt32();
			var Next = r.ReadSymbol(NextOffset);
			var ProcLength = r.ReadUInt32();
			var DebugStartOffset = r.ReadUInt32();
			var DebugEndOffset = r.ReadUInt32();
			var ComToken = r.ReadUInt32();
			var Offset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			var ReturnRegister = r.ReadUInt16();
			var Name = r.ReadSymbolString();

			Data = new ManProcData(
				parentOffset: ParentOffset,
				parent: Parent,
				end: End,
				nextOffset: NextOffset,
				next: Next,
				procLength: ProcLength,
				debugStartOffset: DebugStartOffset,
				debugEndOffset: DebugEndOffset,
				comToken: ComToken,
				offset: Offset,
				segment: Segment,
				flags: Flags,
				returnRegister: ReturnRegister,
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
			w.WriteUInt32(data.ProcLength);
			w.WriteUInt32(data.DebugStartOffset);
			w.WriteUInt32(data.DebugEndOffset);
			w.WriteUInt32(data.ComToken);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.Segment);
			w.Write<CV_PROCFLAGS>(data.Flags);
			w.WriteUInt16(data.ReturnRegister);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}
