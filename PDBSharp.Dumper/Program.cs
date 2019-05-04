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
			if (symbol == null) {
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

		static bool OptDumpStreams = false;
		static bool OptVerbose = false;
		static string PdbFilePath = null;

		private static void ParseArguments(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				switch (arg) {
					case "-dump":
						OptDumpStreams = true;
						break;
					case "-verbose":
						OptVerbose = true;
						break;
					default:
						PdbFilePath = arg;
						return;
				}
			}
		}

		static void Main(string[] args) {
			ParseArguments(args);

			if (PdbFilePath == null) {
				Console.Error.WriteLine("Usage: [-dump][-verbose] <file.pdb>");
				Environment.Exit(1);
			}

			var file = new FileStream(PdbFilePath, FileMode.Open, FileAccess.Read);
			PDBFile pdb = new PDBFile(file);


			if (OptDumpStreams) {
				DirectoryInfo dumpDir = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(PdbFilePath));
				for(int i=1; i<pdb.StreamTable.NumStreams; i++) {
					string dumpPath = Path.Combine(dumpDir.ToString(), $"stream{i}.bin");

					byte[] stream = pdb.StreamTable.GetStream(i);
					File.WriteAllBytes(dumpPath, stream);
				}
			}

			pdb.Types.ForEach(type => {
				if (OptVerbose) {
					ObjectDumper.Dump(type);
				}

				switch (type) {
					case LF_FIELDLIST flst:
						flst.Fields.ForEach(leaf => {
							if (OptVerbose) {
								ObjectDumper.Dump(leaf);
							}
						});
						break;
				}
			});

			pdb.Modules.ForEach(mod => {
				Console.WriteLine($"[MODULE => {mod.Module.ModuleName}]");
				Console.WriteLine($"[OBJECT => {mod.Module.ObjectFileName}]");
				Console.WriteLine();

				mod.Symbols.ForEach(sym => {
					if (OptVerbose) {
						ObjectDumper.Dump(sym);
					}
				});
			});

			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();
		}
	}
}
