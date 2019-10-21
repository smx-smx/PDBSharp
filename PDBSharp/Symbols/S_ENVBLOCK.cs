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
	public class S_ENVBLOCK : SymbolBase
	{
		public string[] Data { get; set; }
		//Reserved according to PDB docs, first bit is fEC tho
		public byte Flags { get; set; }

		public S_ENVBLOCK(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) { 
		}

		public override void Read() {
			var r = CreateReader();

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

		public override void Write() {
			var w = CreateWriter(SymbolType.S_ENVBLOCK);
			w.WriteByte(Flags);
			foreach (string str in Data) {
				w.WriteSymbolString(str);
			}

			w.WriteHeader();
		}
	}
}
