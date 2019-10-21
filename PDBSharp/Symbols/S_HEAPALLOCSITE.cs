#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class HeapAllocSite
	{
		public UInt32 CallSiteOffset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public UInt16 HeapAllocationInstructionSize { get; set; }
		public LeafContainerBase FunctionSignature { get; set; }
	}

	public class S_HEAPALLOCSITE : SymbolBase
	{
		public UInt32 CallSiteOffset { get; set; }
		public UInt16 SectionIndex { get; set; }
		public UInt16 HeapAllocationInstructionSize { get; set; }
		public ILeafContainer FunctionSignature { get; set; }

		public S_HEAPALLOCSITE(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();

			CallSiteOffset = r.ReadUInt32();

			SectionIndex = r.ReadUInt16();
			HeapAllocationInstructionSize = r.ReadUInt16();

			FunctionSignature = r.ReadIndexedTypeLazy();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_HEAPALLOCSITE);
			w.WriteUInt32(CallSiteOffset);
			w.WriteUInt16(SectionIndex);
			w.WriteUInt16(HeapAllocationInstructionSize);
			w.WriteIndexedType(FunctionSignature);

			w.WriteHeader();
		}
	}
}
