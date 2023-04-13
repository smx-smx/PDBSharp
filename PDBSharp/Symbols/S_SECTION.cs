#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_SECTION : SymbolBase
	{
		public UInt16 SectionNumber { get; set; }
		/// <summary>
		/// Alignment of this section (power of 2)
		/// </summary>
		public byte Alignment { get; set; }
		public UInt32 Rva { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 Characteristics { get; set; }
		public string Name { get; set; }

		public S_SECTION(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			SectionNumber = r.ReadUInt16();
			Alignment = r.ReadByte();
			r.ReadByte(); //reserved
			Rva = r.ReadUInt32();
			Length = r.ReadUInt32();
			Characteristics = r.ReadUInt32();
			Name = r.ReadSymbolString(); 
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_SECTION);
			w.WriteUInt16(SectionNumber);
			w.WriteByte(Alignment);
			w.WriteByte(0x00);
			w.WriteUInt32(Rva);
			w.WriteUInt32(Length);
			w.WriteUInt32(Characteristics);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
