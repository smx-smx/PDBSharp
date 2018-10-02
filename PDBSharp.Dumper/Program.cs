#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Smx.PDBSharp.Dumper
{
	class Program
	{
		static void OnSymbolData(ISymbol symbol, byte[] rawData) {
			Console.WriteLine($"[{symbol.Header.Type}]");
			rawData.HexDump();

			FieldInfo data = symbol.GetType().GetField("Data");

			object obj;
			if (data == null) {
				obj = new object();
			} else {
				obj = data.GetValue(symbol);
			}
			string strRep = ObjectDumper.Dump(obj);
			Console.WriteLine(strRep);
		}

		static void Main(string[] args) {
			if (args.Length < 1) {
				Console.Error.WriteLine("Usage: [file.pdb]");
				Environment.Exit(1);
			}

			var file = new FileStream(args[0], FileMode.Open, FileAccess.Read);
			PDBFile pdb = new PDBFile(file);

#if DEBUG
			SymbolDataReader.OnDataRead += OnSymbolData;
#endif

			// trigger enumeration
			foreach (var mod in pdb.Modules) {
				foreach (var sym in mod.Symbols) {
				}
			}

			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();
		}
	}
}
