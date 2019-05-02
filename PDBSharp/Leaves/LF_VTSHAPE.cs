#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_VTSHAPE)]
	public class LF_VTSHAPE : TypeDataReader
	{
		/// <summary>
		/// number of entries in vfunctable
		/// </summary>
		public readonly UInt16 NumberOfEntries;

		public readonly VTableShapeDescriptor[] Descriptors;

		public LF_VTSHAPE(Stream stream) : base(stream) {
			NumberOfEntries = ReadUInt16();

			//round up 4 bits (desctiptor size)
			int numberOfBytes = (int)Math.Ceiling((double)(4 * NumberOfEntries) / sizeof(byte));
			byte[] descriptorsData = ReadBytes(numberOfBytes);

			Descriptors = new VTableShapeDescriptor[NumberOfEntries];

			for(int i=0; i<NumberOfEntries; i++) {
				int offset = 4 * i;
				int descIndex = offset / 8;

				byte data = descriptorsData[descIndex];
				switch(i % 2) {
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
	}
}
