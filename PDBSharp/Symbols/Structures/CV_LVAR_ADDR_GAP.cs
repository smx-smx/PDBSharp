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
using System.Linq;

namespace Smx.PDBSharp.Symbols.Structures
{
	namespace CV_LVAR_ADDR_GAP {
		public class Data {
			public const int SIZE = 4;
			public UInt16 GapStartOffset;
			public UInt16 Length;
		}
		public class Serializer(SpanStream stream)
		{
			public Data Data = new Data();

			public static Data[] ReadGaps(SpanStream stream) {
				// interpret remaining data as gaps
				var numGaps = stream.Remaining / Data.SIZE;
				return Enumerable
					.Range(1, (int)numGaps)
					.Select(_ => new Serializer(stream).Read())
					.ToArray();
			}

			public Data Read() {
				var GapStartOffset = stream.ReadUInt16();
				var Length = stream.ReadUInt16();
				Data = new Data {
					GapStartOffset = GapStartOffset,
					Length = Length
				};
				return Data;
			}

			public void Write(SymbolDataWriter w) {
				w.WriteUInt16(Data.GapStartOffset);
				w.WriteUInt16(Data.Length);
			}
		}
	}
}
