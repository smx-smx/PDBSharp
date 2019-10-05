#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public enum DebugType : Int16
	{
		FPO,
		Exception,
		Fixup,
		OMapToSrc,
		OMapFromSrc,
		SectionHdr,
		TokenRidMap,
		XData,
		PData,
		NewFPO,
		SectionHdrOrig,

		DebugTypeMax
	}

	public class DebugReader : ReaderBase
	{
		public readonly Int16[] DebugStreams = new short[(int)DebugType.DebugTypeMax];

		private readonly Context ctx;

		public FPOReader FPO => lazyFPO.Value;
		private readonly Lazy<FPOReader> lazyFPO;

		public byte[] GetStream(DebugType type) {
			if (!HasStream(type))
				return null;

			int streamNumber = DebugStreams[(int)type];
			return ctx.StreamTableReader.GetStream(streamNumber);
		}

		public bool HasStream(DebugType type) {
			return DebugStreams[(int)type] != -1;
		}

		private FPOReader CreateFPOReader() {
			byte[] fpo = GetStream(DebugType.FPO);
			if (fpo == null)
				return null;

			return new FPOReader(new MemoryStream(fpo));
		}

		public DebugReader(Context ctx, Stream stream) : base(stream) {
			this.ctx = ctx;

			for(int i=0; i<(int)DebugType.DebugTypeMax; i++) {
				DebugStreams[i] = ReadInt16();
			}

			lazyFPO = new Lazy<FPOReader>(CreateFPOReader);
		}
	}
}
