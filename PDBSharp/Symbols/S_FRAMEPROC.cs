#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	public class FrameProcSym
	{
		public UInt32 FrameSize { get; set; }
		public UInt32 PaddingSize { get; set; }
		public UInt32 PaddingOffset { get; set; }
		public UInt32 SavedRegistersSize { get; set; }
		public UInt32 ExceptionHandlerOffset { get; set; }
		public UInt16 ExceptionHandlerSection { get; set; }
		public FrameProcSymFlags Flags { get; set; }
	}

	public class S_FRAMEPROC : ISymbol
	{
		public readonly UInt32 FrameSize;
		public readonly UInt32 PaddingSize;
		public readonly UInt32 PaddingOffset;
		public readonly UInt32 SavedRegistersSize;
		public readonly UInt32 ExceptionHandlerOffset;
		public readonly UInt16 ExceptionHandlerSection;
		public readonly FrameProcSymFlags Flags;

		public S_FRAMEPROC(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);
			FrameSize = r.ReadUInt32();
			PaddingSize = r.ReadUInt32();
			PaddingOffset = r.ReadUInt32();
			SavedRegistersSize = r.ReadUInt32();
			ExceptionHandlerOffset = r.ReadUInt32();
			ExceptionHandlerSection = r.ReadUInt16();
			Flags = new FrameProcSymFlags(r.ReadUInt16());
		}

		public S_FRAMEPROC(FrameProcSym data) {
			FrameSize = data.FrameSize;
			PaddingSize = data.PaddingSize;
			PaddingOffset = data.PaddingOffset;
			SavedRegistersSize = data.SavedRegistersSize;
			ExceptionHandlerOffset = data.ExceptionHandlerOffset;
			ExceptionHandlerSection = data.ExceptionHandlerSection;
			Flags = data.Flags;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_FRAMEPROC);
			w.WriteUInt32(FrameSize);
			w.WriteUInt32(PaddingSize);
			w.WriteUInt32(PaddingOffset);
			w.WriteUInt32(SavedRegistersSize);
			w.WriteUInt32(ExceptionHandlerOffset);
			w.WriteUInt16(ExceptionHandlerSection);
			w.WriteEnum<FrameProcSymFlagsEnum>((FrameProcSymFlagsEnum)Flags);

			w.WriteSymbolHeader();
		}
	}
}
