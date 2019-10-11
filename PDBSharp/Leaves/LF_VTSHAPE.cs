#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_VTSHAPE : ILeaf
	{
		/// <summary>
		/// number of entries in vfunctable
		/// </summary>
		public readonly UInt16 NumberOfEntries;

		public readonly VTableShapeDescriptor[] Descriptors;

		public LF_VTSHAPE(IServiceContainer pdb, SpanStream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			NumberOfEntries = r.ReadUInt16();

			//round up 4 bits (descriptor size)
			int numberOfBytes = (int)Math.Ceiling((double)(4 * NumberOfEntries) / 8);
			byte[] descriptorsData = r.ReadBytes(numberOfBytes);

			Descriptors = new VTableShapeDescriptor[NumberOfEntries];

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
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_VTSHAPE);
			w.WriteUInt16(NumberOfEntries);

			byte data = 0x00;

			for (int i = 0; i < NumberOfEntries; i++) {
				byte descr = (byte)Descriptors[i];

				switch (i % 2) {
					case 0:
						data = (byte)(descr & 0xF);
						break;
					case 1:
						data = (byte)(((descr << 4) & 0xF) | data);
						w.WriteByte(data);
						break;
				}
			}

			w.WriteLeafHeader();
		}
	}
}
