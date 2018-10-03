#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public static class ByteArrayExtensions
	{
		const int STRING_OFFSET = 62;

		public static void HexDump(this byte[] bytes, int? optBytesLength = null) {
			IEnumerable<byte> seq = bytes;
			StringBuilder sb = new StringBuilder();

			int index = 0;
			int lineLength = 0;

			char[] lineBuf = new char[16];
			int lineBufIdx = 0;
			while(index < bytes.Length) {
				if ((index % 16) == 0) {
					sb.AppendFormat("{0:X8}   ", index);
					lineLength = 8 + 3;
					lineBufIdx = 0;
				}

				int takeLength = Math.Min(seq.Count(), 8);
				var chunk = bytes.Skip(index).Take(takeLength);

				chunk.All(b => {
					char c = Convert.ToChar(b);
					if (char.IsControl(c)) {
						c = '.';
					}
					lineBuf[lineBufIdx++] = c;
					lineBufIdx %= 16;

					sb.AppendFormat("{0:X2} ", b);
					lineLength += 3;

					return true;
				});
				
				seq = seq.Skip(takeLength);
				index += takeLength;

				if (takeLength == 8) {
					sb.Append(" ");
					lineLength++;
				}

				if ((index % 16) == 0 || index >= bytes.Length) {
					sb.Append(new string(' ', STRING_OFFSET - lineLength));
					int sz = index % 16;
					if (sz == 0)
						sz = 16;
					sb.Append(lineBuf.Take(sz).ToArray());
					sb.AppendLine();
				}
			}

			Console.Write(sb.ToString());
		}
	}
}
