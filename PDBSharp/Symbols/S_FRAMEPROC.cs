#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_FRAMEPROC
{
	public class Data : ISymbolData {
		public UInt32 FrameSize { get; set; }
		public UInt32 PaddingSize { get; set; }
		public UInt32 PaddingOffset { get; set; }
		public UInt32 SavedRegistersSize { get; set; }
		public UInt32 ExceptionHandlerOffset { get; set; }
		public UInt16 ExceptionHandlerSection { get; set; }
		public FrameProcSymFlags Flags { get; set; }

		public Data(uint frameSize, uint paddingSize, uint paddingOffset, uint savedRegistersSize, uint exceptionHandlerOffset, ushort exceptionHandlerSection, FrameProcSymFlags flags) {
			FrameSize = frameSize;
			PaddingSize = paddingSize;
			PaddingOffset = paddingOffset;
			SavedRegistersSize = savedRegistersSize;
			ExceptionHandlerOffset = exceptionHandlerOffset;
			ExceptionHandlerSection = exceptionHandlerSection;
			Flags = flags;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public void Read() {
			var r = CreateReader();
			var FrameSize = r.ReadUInt32();
			var PaddingSize = r.ReadUInt32();
			var PaddingOffset = r.ReadUInt32();
			var SavedRegistersSize = r.ReadUInt32();
			var ExceptionHandlerOffset = r.ReadUInt32();
			var ExceptionHandlerSection = r.ReadUInt16();
			var Flags = new FrameProcSymFlags(r.ReadUInt16());
			Data = new Data(
				frameSize: FrameSize,
				paddingSize: PaddingSize,
				paddingOffset: PaddingOffset,
				savedRegistersSize: SavedRegistersSize,
				exceptionHandlerOffset: ExceptionHandlerOffset,
				exceptionHandlerSection: ExceptionHandlerSection,
				flags: Flags
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_FRAMEPROC);
			w.WriteUInt32(data.FrameSize);
			w.WriteUInt32(data.PaddingSize);
			w.WriteUInt32(data.PaddingOffset);
			w.WriteUInt32(data.SavedRegistersSize);
			w.WriteUInt32(data.ExceptionHandlerOffset);
			w.WriteUInt16(data.ExceptionHandlerSection);
			w.Write<FrameProcSymFlagsEnum>((FrameProcSymFlagsEnum)data.Flags);

			w.WriteHeader();
		}
	}
}
