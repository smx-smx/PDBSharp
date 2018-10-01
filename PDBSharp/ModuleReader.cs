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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class ModuleReader : ReaderBase
	{
		private const int SIGNATURE = 4;
		private readonly ModuleInfoInstance modInfo;

		public ModuleReader(ModuleInfoInstance modInfo, Stream stream) : base(stream) {
			this.Load();
			this.modInfo = modInfo;

			GetSymbols();
		}

		private void Load() {
			uint signature = Reader.ReadUInt32();
			if(signature != SIGNATURE) {
				throw new InvalidDataException();
			}
		}

		public IEnumerable<int> GetSymbols() {
			Stream.Position = sizeof(int); //after signature
			int symbolsSize = (int)modInfo.Header.SymbolsSize - sizeof(int); //exclude signature

			byte[] symbolsData = Reader.ReadBytes(symbolsSize);

			var rdr = new SymbolsReader(new MemoryStream(symbolsData));
			foreach(var symbol in rdr.Symbols) {
				Console.WriteLine(symbol.ToString());
			}

			return null;
		}
	}
}
