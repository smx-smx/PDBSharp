#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Text;

namespace Smx.PDBSharp
{
	public static class ByteArrayExtensions
	{
		private static readonly int ROW_PRESIZE = 0x3E;
		private static readonly int ROW_POSTSIZE = 16 + Environment.NewLine.Length;
		private static readonly int ROW_SIZE = ROW_PRESIZE + ROW_POSTSIZE;

		private static int ROUND_UP_DIV(int val, int div) {
			return (val + div - 1) / div;
		}

		public static void HexDump(this byte[] bytes, int? optBytesLength = null) {
			int max = ROUND_UP_DIV(bytes.Length, ROW_SIZE) * ROW_SIZE;
			StringBuilder sb = new StringBuilder(max);

			int i = 0, j = 0, octets = 0;
			while (i < bytes.Length) {
				int offset = i & 15;
				if (offset == 0) {
					sb.AppendFormat("{0:X8}   ", i);
				}

				sb.AppendFormat("{0:X2} ", bytes[i++]);

				if (i > 0 && (i & 7) == 0) {
					if (++octets == 2)
						sb.Append("  ");
					else
						sb.Append(' ');
				}

				if (octets == 2 || i == bytes.Length) {
					int ws = sb.Length % ROW_SIZE;
					if (ws < ROW_PRESIZE) {
						sb.Append(new string(' ', ROW_PRESIZE - ws));
					}

					// go back to saved pos, and print the bytes content
					for (int k = j; k < i; k++) {
						if (bytes[k] < 0x20)
							sb.Append('.');
						else
							sb.Append((char)(bytes[k]));
					}
					sb.AppendLine();

					j = i;
					octets = 0;
				}
			}

			Console.Write(sb.ToString());
		}
	}
}
