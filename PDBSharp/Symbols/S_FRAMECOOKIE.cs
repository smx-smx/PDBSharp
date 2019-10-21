#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_FRAMECOOKIE : SymbolBase
	{
		public UInt32 Offset { get; set; }
		public UInt16 RegisterIndex { get; set; }
		public CookieType Type { get; set; }
		public byte Flags { get; set; }

		public S_FRAMECOOKIE(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			Offset = r.ReadUInt32();
			RegisterIndex = r.ReadUInt16();
			Type = r.ReadEnum<CookieType>();
			Flags = r.ReadByte();
		}
		public override void Write() {
			var w = CreateWriter(SymbolType.S_FRAMECOOKIE);
			w.WriteUInt32(Offset);
			w.WriteUInt16(RegisterIndex);
			w.Write<CookieType>(Type);
			w.WriteByte(Flags);

			w.WriteHeader();
		}
	}
}
