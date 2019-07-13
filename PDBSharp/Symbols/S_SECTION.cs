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
	public class SectionSym
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
	}

	public class S_SECTION : ISymbol
	{
		public readonly UInt16 SectionNumber;
		/// <summary>
		/// Alignment of this section (power of 2)
		/// </summary>
		public readonly byte Alignment;
		public readonly UInt32 Rva;
		public readonly UInt32 Length;
		public readonly UInt32 Characteristics;
		public readonly string Name;

		public S_SECTION(Context ctx, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);

			SectionNumber = r.ReadUInt16();
			Alignment = r.ReadByte();
			r.ReadByte(); //reserved
			Rva = r.ReadUInt32();
			Length = r.ReadUInt32();
			Characteristics = r.ReadUInt32();
			Name = r.ReadSymbolString();
		}

		public S_SECTION(SectionSym data) {
			SectionNumber = data.SectionNumber;
			Alignment = data.Alignment;
			Rva = data.Rva;
			Length = data.Length;
			Characteristics = data.Characteristics;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_SECTION);
			w.WriteUInt16(SectionNumber);
			w.WriteByte(Alignment);
			w.WriteByte(0x00);
			w.WriteUInt32(Rva);
			w.WriteUInt32(Length);
			w.WriteUInt32(Characteristics);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
