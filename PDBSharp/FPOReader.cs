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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

	namespace FPO
	{
		namespace Frame
		{
			public class Data
			{
				public UInt32 StartOffset;
				public UInt32 FunctionSize;
				public UInt32 SizeLocalsDwords;
				public UInt16 SizeParamsDwords;
				public byte PrologSize;
				public byte Flags;

				// frame type determined by size
				public FPOFrameType FrameType => (FPOFrameType)FrameSize;

				public byte NumberSavedRegisters => (byte)(Flags & 3);
				public bool HasSEH => ((Flags >> 3) & 1) == 1;
				public bool UsesBasePointer => ((Flags >> 4) & 1) == 1;
				// bit 5 is reserved
				public byte FrameSize => (byte)((Flags >> 6) & 2);


				public const int SIZE = 16;
			}

			public class Serializer(SpanStream stream)
			{
				public Data Data = new Data();
				public Data Read() {
					var StartOffset = stream.ReadUInt32();
					var FunctionSize = stream.ReadUInt32();
					var SizeLocalsDwords = stream.ReadUInt32();
					var SizeParamsDwords = stream.ReadUInt16();
					var PrologSize = stream.ReadByte();
					var flags = stream.ReadByte();

					Data = new Data {
						StartOffset = StartOffset,
						FunctionSize = FunctionSize,
						SizeLocalsDwords = SizeLocalsDwords,
						SizeParamsDwords = SizeParamsDwords,
						PrologSize = PrologSize,
						Flags = flags,
					};
					return Data;
				}
			}
		}

		namespace Stream
		{
			public class Data
			{
				public IEnumerable<Frame.Data> Frames = Enumerable.Empty<Frame.Data>();
			}

			public class Serializer(SpanStream stream)
			{
				public Data Data = new Data();

				private IEnumerable<Frame.Data> ReadFrames() {
					while (stream.Position < stream.Length) {
						var frame = new Frame.Serializer(stream.SliceHere(Frame.Data.SIZE)).Read();
						yield return frame;
						stream.Position += Frame.Data.SIZE;
					}
				}

				public Data Read() {
					var Frames = new CachedEnumerable<Frame.Data>(ReadFrames());
					Data = new Data {
						Frames = Frames
					};
					return Data;
				}
			}
		}
	}
}
