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

namespace Smx.PDBSharp.Leaves.LF_VTSHAPE
{
	public class Data : ILeafData {
		/// <summary>
		/// number of entries in vfunctable
		/// </summary>
		public UInt16 NumberOfEntries { get; set; }

		public VTableShapeDescriptor[] Descriptors { get; set; }

		public Data(ushort numberOfEntries, VTableShapeDescriptor[] descriptors) {
			NumberOfEntries = numberOfEntries;
			Descriptors = descriptors;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer pdb, SpanStream stream) : base(pdb, stream) {
		}


		public void Read() {
			TypeDataReader r = CreateReader();

			var NumberOfEntries = r.ReadUInt16();

			//round up 4 bits (descriptor size)
			int numberOfBytes = (int)Math.Ceiling((double)(4 * NumberOfEntries) / 8);
			byte[] descriptorsData = r.ReadBytes(numberOfBytes);

			var Descriptors = new VTableShapeDescriptor[NumberOfEntries];

			for (int i = 0; i < NumberOfEntries; i++) {
				byte data = descriptorsData[i / 2];
				switch (i % 2) {
					case 0:
						data &= 0xF;
						break;
					case 1:
						data = (byte)((data >> 4) & 0xF);
						break;
				}

				Descriptors[i] = (VTableShapeDescriptor)data;
			}

			Data = new Data(
				numberOfEntries: NumberOfEntries,
				descriptors: Descriptors
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_VTSHAPE);
			w.WriteUInt16(data.NumberOfEntries);

			byte value = 0x00;

			for (int i = 0; i < data.NumberOfEntries; i++) {
				byte descr = (byte)data.Descriptors[i];

				switch (i % 2) {
					case 0:
						value = (byte)(descr & 0xF);
						break;
					case 1:
						value = (byte)(((descr << 4) & 0xF) | value);
						w.WriteByte(value);
						break;
				}
			}

			w.WriteHeader();
		}
	}
}
