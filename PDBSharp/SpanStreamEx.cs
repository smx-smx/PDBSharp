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
	public class SpanStreamEx : SpanStream
	{
		public SpanStreamEx(SpanStream other) : base(other) {
		}

		public SpanStreamEx(Memory<byte> data, SharpIO.Endianness endianness = SharpIO.Endianness.LittleEndian) : base(data, endianness) {
		}

		public SpanStreamEx(MFile mf) : base(mf) {
		}

		public string ReadString16NoTerm() {
			int length = ReadByte();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			return str;
		}

		public string ReadString16() {
			int length = ReadByte();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			ReadByte(); //null terminator
			return str;
		}

		public string ReadString32() {
			int length = ReadInt32();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			ReadByte(); //null terminator
			return str;
		}
	}
}
