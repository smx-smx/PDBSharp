#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.S_SEPCODE;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	namespace S_PUB32
	{
		[Flags]
		public enum PubSymFlags : uint {
			/// <summary>
			/// set if public symbol refers to a code address
			/// </summary>
			Code = 1 << 0,
			/// <summary>
			/// set if public symbol is a function
			/// </summary>
			Function = 1 << 1,
			/// <summary>
			/// set if managed code (native or IL)
			/// </summary>
			Managed = 1 << 2,
			/// <summary>
			/// set if managed IL code
			/// </summary>
			MSIL = 1 << 3
		}

		public class Data : ISymbolData
		{
			public PubSymFlags Flags;
			public uint Offset;
			public ushort Segment;
			public string Name = string.Empty;

			public override string ToString() {
				return $"S_PUB32(Name='{Name}', Segment='{Segment}', Offset='{Offset}')";
			}
		}

		public class Serializer : SymbolSerializerBase, ISymbolSerializer
		{
			public Data Data = new Data();

			public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			}

			public ISymbolData? GetData() {
				return Data;
			}

			public void Read() {
				var r = CreateReader();
				var flags = r.ReadFlagsEnum<PubSymFlags>();
				var offset = r.ReadUInt32();
				var segment = r.ReadUInt16();
				var name = r.ReadSymbolString();
				Data = new Data {
					Flags = flags,
					Offset = offset,
					Segment = segment,
					Name = name
				};
			}

			public void Write() {
			}
		}
	}
}
