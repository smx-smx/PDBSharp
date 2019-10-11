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
	public class FrameCookie
	{
		public UInt32 Offset;
		public UInt16 RegisterIndex;
		public CookieType Type;
		public byte Flags;
	}

	public class S_FRAMECOOKIE : ISymbol
	{
		public readonly UInt32 Offset;
		public readonly UInt16 RegisterIndex;
		public readonly CookieType Type;
		public readonly byte Flags;

		public S_FRAMECOOKIE(IServiceContainer ctx, IModule mod, SpanReader stream) {
			var r = new SymbolDataReader(ctx, stream);

			Offset = r.ReadUInt32();
			RegisterIndex = r.ReadUInt16();
			Type = r.ReadEnum<CookieType>();
			Flags = r.ReadByte();
		}

		public S_FRAMECOOKIE(FrameCookie data) {
			Offset = data.Offset;
			RegisterIndex = data.RegisterIndex;
			Type = data.Type;
			Flags = data.Flags;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_FRAMECOOKIE);
			w.WriteUInt32(Offset);
			w.WriteUInt16(RegisterIndex);
			w.WriteEnum<CookieType>(Type);
			w.WriteByte(Flags);

			w.WriteSymbolHeader();
		}
	}
}
