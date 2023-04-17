#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	public class PDBSpanStream : SpanStream
	{
		private readonly PDBType type;

		public const int CB_SIZE = sizeof(UInt32);
		public const int UPN_SIZE = sizeof(UInt32);
		public int PN_SIZE { get; private set; }
		public int SI_PERSIST_SIZE { get; private set; }

		private void SetDataTypeSizes() {
			switch (type) {
				case PDBType.Big:
					PN_SIZE = sizeof(UInt32);
					//only CB is stored
					SI_PERSIST_SIZE = CB_SIZE;
					break;
				case PDBType.Small:
					PN_SIZE = sizeof(UInt16);
					// CB + pageOf(map<SPN,PN>)
					SI_PERSIST_SIZE = CB_SIZE + sizeof(UInt32);
					break;
			}
		}

		public PDBSpanStream(SpanStream other, PDBType type) : base(other) {
			this.type = type;
			SetDataTypeSizes();
		}

		public PDBSpanStream(Memory<byte> data, PDBType type) : base(data) {
			this.type = type;
			SetDataTypeSizes();
		}

		public int ReadInt() {
			switch (this.type) {
				case PDBType.Big:
					return ReadInt32();
				case PDBType.Small:
					return ReadInt16();
				default:
					throw new ArgumentException();
			}
		}

		public uint ReadUInt() {
			switch (this.type) {
				case PDBType.Big:
					return ReadUInt32();
				case PDBType.Small:
					return ReadUInt16();
				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		/// Read a "number of bytes" field  (int32)
		/// </summary>
		/// <returns></returns>
		public int ReadCB() => ReadInt32();


		/// <summary>
		/// Read a Page Number (16/32 bit)
		/// </summary>
		/// <returns></returns>
		public uint ReadPN() => ReadUInt();
	}
}
