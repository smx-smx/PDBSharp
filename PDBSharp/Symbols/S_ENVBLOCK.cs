#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion

using System;
using Smx.SharpIO;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_ENVBLOCK
{
	public class Data : ISymbolData {
		public string[] EnvData { get; set; }
		public byte Flags { get; set; }
		
		public Data(string[] envData, byte flags) {
			EnvData = envData;
			Flags = flags;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }

		//Reserved according to PDB docs, first bit is fEC tho

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var Flags = r.ReadByte(); //fEC -> reserved (1 bit)

			List<string> strLst = new List<string>(); ;
			while (r.HasMoreData) {
				string str = r.ReadSymbolString();
				if (str.Length == 0)
					break;
				strLst.Add(str);
			}

			Data = new Data(
				flags: Flags,
				envData: strLst.ToArray()
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_ENVBLOCK);
			w.WriteByte(data.Flags);
			foreach (string str in data.EnvData) {
				w.WriteSymbolString(str);
			}

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
