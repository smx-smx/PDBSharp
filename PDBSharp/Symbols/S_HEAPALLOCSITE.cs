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
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_HEAPALLOCSITE
{
	public class Data : ISymbolData
	{
		public UInt32 CallSiteOffset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public UInt16 HeapAllocationInstructionSize { get; set; }
		public ILeafResolver? FunctionSignature { get; set; }

		public Data(uint callSiteOffset, ushort sectionIndex, ushort heapAllocationInstructionSize, ILeafResolver? functionSignature) {
			CallSiteOffset = callSiteOffset;
			SectionIndex = sectionIndex;
			HeapAllocationInstructionSize = heapAllocationInstructionSize;
			FunctionSignature = functionSignature;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;
		
		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var CallSiteOffset = r.ReadUInt32();
			var SectionIndex = r.ReadUInt16();
			var HeapAllocationInstructionSize = r.ReadUInt16();
			var FunctionSignature = r.ReadIndexedType32Lazy();
			Data = new Data(
				callSiteOffset: CallSiteOffset,
				sectionIndex: SectionIndex,
				heapAllocationInstructionSize: HeapAllocationInstructionSize,
				functionSignature: FunctionSignature
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException(); 
			
			var w = CreateWriter(SymbolType.S_HEAPALLOCSITE);
			w.WriteUInt32(data.CallSiteOffset);
			w.WriteUInt16(data.SectionIndex);
			w.WriteUInt16(data.HeapAllocationInstructionSize);
			w.WriteIndexedType(data.FunctionSignature);
			w.WriteHeader();
		}
	}
}
