#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_FRAMEPROC : SymbolBase
	{
		public UInt32 FrameSize { get; set; }
		public UInt32 PaddingSize { get; set; }
		public UInt32 PaddingOffset { get; set; }
		public UInt32 SavedRegistersSize { get; set; }
		public UInt32 ExceptionHandlerOffset { get; set; }
		public UInt16 ExceptionHandlerSection { get; set; }
		public FrameProcSymFlags Flags { get; set; }
		
		public S_FRAMEPROC(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			FrameSize = r.ReadUInt32();
			PaddingSize = r.ReadUInt32();
			PaddingOffset = r.ReadUInt32();
			SavedRegistersSize = r.ReadUInt32();
			ExceptionHandlerOffset = r.ReadUInt32();
			ExceptionHandlerSection = r.ReadUInt16();
			Flags = new FrameProcSymFlags(r.ReadUInt16());
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_FRAMEPROC);
			w.WriteUInt32(FrameSize);
			w.WriteUInt32(PaddingSize);
			w.WriteUInt32(PaddingOffset);
			w.WriteUInt32(SavedRegistersSize);
			w.WriteUInt32(ExceptionHandlerOffset);
			w.WriteUInt16(ExceptionHandlerSection);
			w.Write<FrameProcSymFlagsEnum>((FrameProcSymFlagsEnum)Flags);

			w.WriteHeader();
		}
	}
}
