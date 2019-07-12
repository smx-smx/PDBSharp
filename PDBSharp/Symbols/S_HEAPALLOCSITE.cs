#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	public class HeapAllocSite
	{
		public UInt32 CallSiteOffset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public UInt16 HeapAllocationInstructionSize { get; set; }
		public LeafBase FunctionSignature { get; set; }
	}

	public class S_HEAPALLOCSITE : ISymbol
	{
		public readonly UInt32 CallSiteOffset;
		public readonly UInt16 SectionIndex;
		public readonly UInt16 HeapAllocationInstructionSize;
		public readonly ILeafContainer FunctionSignature;

		public S_HEAPALLOCSITE(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);

			CallSiteOffset = r.ReadUInt32();

			SectionIndex = r.ReadUInt16();
			HeapAllocationInstructionSize = r.ReadUInt16();

			FunctionSignature = r.ReadIndexedTypeLazy();
		}

		public S_HEAPALLOCSITE(HeapAllocSite data) {
			CallSiteOffset = data.CallSiteOffset;
			SectionIndex = data.SectionIndex;
			HeapAllocationInstructionSize = data.HeapAllocationInstructionSize;
			FunctionSignature = data.FunctionSignature;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_HEAPALLOCSITE);
			w.WriteUInt32(CallSiteOffset);
			w.WriteUInt16(SectionIndex);
			w.WriteUInt16(HeapAllocationInstructionSize);
			w.WriteIndexedType(FunctionSignature);

			w.WriteSymbolHeader();
		}
	}
}
