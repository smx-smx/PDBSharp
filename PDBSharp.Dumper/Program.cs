#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using MoreLinq;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
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
			Console.WriteLine(new string('=', 80));
			Console.WriteLine();

			SymbolHeader symHdr;
			if(symbol == null) {
				symHdr = new SymbolHeaderReader(new MemoryStream(rawData)).Data;
			} else {
				symHdr = symbol.Header;
			}

			Console.WriteLine($"[{symHdr.Type}]");
			rawData.HexDump();
			Console.WriteLine();

			if (symbol == null)
				return;

			FieldInfo data = symbol.GetType().GetField("Data");

			object obj;
			if (data != null) {
				obj = data.GetValue(symbol);
				ObjectDumper.Dump(obj);
			}
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

#if FALSE
			int i = 0;
			foreach (byte[] stream in pdb.Streams){
				File.WriteAllBytes($"stream{i}.bin", stream);
				i++;
			}
#endif

			foreach(var type in pdb.Types) {
				switch (type) {
					case LF_FIELDLIST flst:
						flst.Fields.ForEach(leaf => ObjectDumper.Dump(leaf));
						break;
				}
				ObjectDumper.Dump(type);
			}
			return;

			// trigger enumeration
			foreach (var mod in pdb.Modules) {
				Console.WriteLine($"[MODULE => {mod.Module.ModuleName}]");
				Console.WriteLine($"[OBJECT => {mod.Module.ObjectFileName}]");
				Console.WriteLine();
				foreach (var sym in mod.Symbols) {
				}
			}

			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();
		}
	}
}
