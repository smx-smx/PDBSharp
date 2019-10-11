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
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{

	public enum FPOFrameType : byte
	{
		Fpo = 0,
		Trap = 1,
		Tss = 2,
		Std = 3
	}

	public struct FPOData
	{
		public readonly UInt32 StartOffset;
		public readonly UInt32 FunctionSize;
		public readonly UInt32 SizeLocalsDwords;
		public readonly UInt16 SizeParamsDwords;
		public readonly byte PrologSize;
		private readonly byte flags;

		// frame type determined by size
		public FPOFrameType FrameType => (FPOFrameType)FrameSize;

		public byte NumberSavedRegisters => (byte)(flags & 3);
		public bool HasSEH => ((flags >> 3) & 1) == 1;
		public bool UsesBasePointer => ((flags >> 4) & 1) == 1;
		// bit 5 is reserved
		public byte FrameSize => (byte)((flags >> 6) & 2);
	}

	public class _FPOData : SpanStream
	{
		public readonly UInt32 StartOffset;
		public readonly UInt32 FunctionSize;
		public readonly UInt32 SizeLocalsDwords;
		public readonly UInt16 SizeParamsDwords;
		public readonly byte PrologSize;
		private readonly byte flags;

		// frame type determined by size
		public FPOFrameType FrameType => (FPOFrameType)FrameSize;

		public byte NumberSavedRegisters => (byte)(flags & 3);
		public bool HasSEH => ((flags >> 3) & 1) == 1;
		public bool UsesBasePointer => ((flags >> 4) & 1) == 1;
		// bit 5 is reserved
		public byte FrameSize => (byte)((flags >> 6) & 2);

		public _FPOData(SpanStream stream) : base(stream) {
			StartOffset = ReadUInt32();
			FunctionSize = ReadUInt32();
			SizeLocalsDwords = ReadUInt32();
			SizeParamsDwords = ReadUInt16();
			PrologSize = ReadByte();
			flags = ReadByte();
		}

		public const int SIZE = 16;
	}

	public unsafe class FPOReader : SpanStream
	{
		public IEnumerable<FPOData> Frames => lazyFrames.Value;
		private readonly ILazy<IEnumerable<FPOData>> lazyFrames;

		private readonly int itemSize = sizeof(FPOData);

		private IEnumerable<FPOData> ReadFrames() {
			while (Position < Length) {
				this.Span.Slice((int)Position, itemSize);
				yield return Span.Read<FPOData>((int)Position);
				Position += itemSize;
			}
		}

		public FPOReader(byte[] data) : base(data) {
			lazyFrames = LazyFactory.CreateLazy(ReadFrames);
		}
	}

}
