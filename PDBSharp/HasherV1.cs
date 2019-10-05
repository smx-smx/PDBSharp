#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public class HasherV1
	{
		public readonly Context ctx;
		public HasherV1(Context ctx) {
			this.ctx = ctx;
		}

		private const uint LOWER_MASK = 0x20202020;

		public static UInt32 HashData(byte[] data, uint modulo) {
			uint hash = 0;

			int leadingWords = data.Length / 4;

			using (BinaryReader br = new BinaryReader(new MemoryStream(data))) {
				for (int i = 0; i < leadingWords; i++) {
					hash ^= br.ReadUInt32();
				}

				if((data.Length & 2) != 0) {
					hash ^= br.ReadUInt16();
				}

				if((data.Length & 1) != 0) {
					hash ^= br.ReadByte();
				}

				hash |= LOWER_MASK;
				hash ^= (hash >> 11);

				hash ^= (hash >> 16);
				return hash % modulo;
			}
		}

	}
}
