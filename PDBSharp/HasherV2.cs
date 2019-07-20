#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using DamienG.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public class HasherV2
	{
		private readonly Context ctx;
		public HasherV2(Context ctx) {
			this.ctx = ctx;
		}

		private static UInt32 HashUlong(uint number) {
			return (uint)(number * 1664525L + 1013904223L);
		}

		public static UInt32 HashBufferV8(byte[] buffer, uint modulo) {
			return Crc32.Compute(buffer) % modulo;
		}

		public UInt32 HashTypeIndex(uint typeIndex) {
			byte[] data = BitConverter.GetBytes(typeIndex);
			return HashData(data, ctx.TpiReader.Header.Hash.NumHashBuckets);
		}

		public UInt32 HashString(string str) {
			byte[] data = Encoding.ASCII.GetBytes(str);
			return HashData(data, ctx.TpiReader.Header.Hash.NumHashBuckets);
		}

		public static UInt32 HashData(byte[] data, uint modulo) {
			uint hash = 0xb170a1bf;

			int remaining = data.Length;
			using (BinaryReader br = new BinaryReader(new MemoryStream(data))) {
				while (remaining >= 4) {
					remaining -= 4;
					hash += br.ReadUInt32();
					hash += (hash << 10);
					hash ^= (hash >> 6);
				}

				while(remaining > 0) {
					remaining--;
					hash += br.ReadByte();
					hash += (hash << 10);
					hash ^= (hash >> 6);
				}
			}

			return HashUlong(hash) % modulo;
		}
	}
}
