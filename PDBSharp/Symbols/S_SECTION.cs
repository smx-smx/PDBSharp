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
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_SECTION
{
	public class Data : ISymbolData {
		public UInt16 SectionNumber { get; set; }

		/// <summary>
		/// Alignment of this section (power of 2)
		/// </summary>
		public byte Alignment { get; set; }

		public UInt32 Rva { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 Characteristics { get; set; }
		public string Name { get; set; }

		public Data(ushort sectionNumber, byte alignment, uint rva, uint length, uint characteristics, string name) {
			SectionNumber = sectionNumber;
			Alignment = alignment;
			Rva = rva;
			Length = length;
			Characteristics = characteristics;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var SectionNumber = r.ReadUInt16();
			var Alignment = r.ReadByte();
			r.ReadByte(); //reserved
			var Rva = r.ReadUInt32();
			var Length = r.ReadUInt32();
			var Characteristics = r.ReadUInt32();
			var Name = r.ReadSymbolString();

			Data = new Data(
				sectionNumber: SectionNumber,
				alignment: Alignment,
				rva: Rva,
				length: Length,
				characteristics: Characteristics,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_SECTION);
			w.WriteUInt16(data.SectionNumber);
			w.WriteByte(data.Alignment);
			w.WriteByte(0x00);
			w.WriteUInt32(data.Rva);
			w.WriteUInt32(data.Length);
			w.WriteUInt32(data.Characteristics);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
