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
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_FRAMECOOKIE
{
	public class Data : ISymbolData {
		public UInt32 Offset { get; set; }
		public UInt16 RegisterIndex { get; set; }
		public CookieType Type { get; set; }
		public byte Flags { get; set; }

		public Data(uint offset, ushort registerIndex, CookieType type, byte flags) {
			Offset = offset;
			RegisterIndex = registerIndex;
			Type = type;
			Flags = flags;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var Offset = r.ReadUInt32();
			var RegisterIndex = r.ReadUInt16();
			var Type = r.ReadEnum<CookieType>();
			var Flags = r.ReadByte();

			Data = new Data(
				offset: Offset,
				registerIndex: RegisterIndex,
				type: Type,
				flags: Flags
			);
		}
		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_FRAMECOOKIE);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.RegisterIndex);
			w.Write<CookieType>(data.Type);
			w.WriteByte(data.Flags);

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
