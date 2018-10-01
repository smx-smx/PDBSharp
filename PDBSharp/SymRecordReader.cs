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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class SymRecordReader : ReaderBase
	{
		public SymRecordReader(Stream stream) : base(stream) {
		}

		public IEnumerable<byte[]> ReadSymbols() {
			var remaining = Stream.Length;
			while(remaining > 0) {
				uint symSize = Reader.ReadUInt16();
				if(symSize == 0 && remaining > 0) {
					throw new InvalidDataException();
				}

				UInt16 symType = Reader.ReadUInt16();
				if(!Enum.IsDefined(typeof(SymbolType), symType)) {
					throw new InvalidDataException();
				}

				Trace.WriteLine($"Size: {symSize} bytes");
				Stream.Position += symSize;
			}

			return null;
		}
	}
}
