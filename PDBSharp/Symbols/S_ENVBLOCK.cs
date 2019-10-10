#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_ENVBLOCK : ISymbol
	{
		public readonly string[] Data;
		public readonly byte Flags;

		public S_ENVBLOCK(IServiceContainer ctx, IModule mod, ReaderSpan stream) {
			var r = new SymbolDataReader(ctx, stream);

			Flags = r.ReadByte(); //fEC -> reserved (1 bit)

			List<string> strLst = new List<string>(); ;
			while (r.HasMoreData) {
				string str = r.ReadSymbolString();
				if (str.Length == 0)
					break;
				strLst.Add(str);
			}

			Data = strLst.ToArray();
		}

		public S_ENVBLOCK(string[] data) {
			Flags = 0x00; //Reserved according to PDB docs, first bit is fEC tho
			Data = data;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_ENVBLOCK);
			w.WriteByte(Flags);
			foreach (string str in Data) {
				w.WriteSymbolString(str);
			}

			w.WriteSymbolHeader();
		}
	}
}
