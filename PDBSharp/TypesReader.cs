#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public class TypesReader : ReaderBase
	{
		public TypesReader(Stream stream) : base(stream) {
		}

		public IEnumerable<ILeaf> ReadTypes() {
			var remaining = Stream.Length;

			while(remaining > 0) {
				UInt16 length = Reader.ReadUInt16();
				if (length == 0)
					break;

				int dataSize = length + sizeof(UInt16);
				byte[] symDataBuf = new byte[dataSize];

				BinaryWriter wr = new BinaryWriter(new MemoryStream(symDataBuf));
				wr.Write(length);
				wr.Write(Reader.ReadBytes((int)length));

				TypeDataReader rdr = new TypeDataReader(new MemoryStream(symDataBuf));
				yield return rdr.ReadType();

				remaining -= dataSize;
			}
		}
	}
}
