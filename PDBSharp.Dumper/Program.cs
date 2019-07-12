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
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;

namespace Smx.PDBSharp.Dumper
{
	public class Program
	{
		public static bool OptDumpModules = false;
		public static bool OptDumpLeaves = false;
		public static bool OptDumpSymbols = false;
		public static bool OptDumpStreams = false;
		public static bool OptVerbose = false;
		public static string PdbFilePath = null;

		public static bool OptDisableReflection = false;

		private static void ParseArguments(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				switch (arg) {
					case "-debug":
						OptDisableReflection = true;
						break;
					case "-dump":
						OptDumpStreams = true;
						break;
					case "-dump-modules":
						OptDumpModules = true;
						break;
					case "-dump-leaves":
						OptDumpLeaves = true;
						break;
					case "-dump-syms":
						OptDumpSymbols = true;
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

			//var file = new FileStream(PdbFilePath, FileMode.Open, FileAccess.Read);
			MemoryMappedFile mmap = MemoryMappedFile.CreateFromFile(PdbFilePath, FileMode.Open);
			Stream mmapStream = mmap.CreateViewStream();
			PDBFile pdb = new PDBFile(mmapStream);

			if (OptDumpLeaves) {
				pdb.OnTpiInit += Pdb_OnTpiInit;
			}
			if (OptDumpModules || OptDumpSymbols) {
				pdb.OnDbiInit += Pdb_OnDbiInit;
			}

			if (OptDumpStreams) {
				DirectoryInfo dumpDir = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(PdbFilePath));
				for(int i=1; i<pdb.StreamTable.NumStreams; i++) {
					string dumpPath = Path.Combine(dumpDir.ToString(), $"stream{i}.bin");

					byte[] stream = pdb.StreamTable.GetStream(i);
					File.WriteAllBytes(dumpPath, stream);
				}
			}

			foreach(var type in pdb.Types) {
				if (!OptDisableReflection) {
					ObjectDumper.Dump(type);
				}

				/*switch (type.Data) {
					case LF_FIELDLIST flst:
						flst.Fields.ForEach(leaf => {
							if (OptVerbose) {
								ObjectDumper.Dump(leaf);
							}
						});
						break;
				}*/
			}

			foreach(var container in pdb.Modules) { 
				Console.WriteLine($"[MODULE => {container.Info.ModuleName}]");
				Console.WriteLine($"[OBJECT => {container.Info.ObjectFileName}]");
				if (container.Module != null) {
					Console.WriteLine($"[TYPE   => {container.Module.GetType().Name}");
				}
				Console.WriteLine();

				IModule mod = container.Module;
				if (mod != null) {
					foreach (var sym in mod.Symbols) {
						if (OptVerbose && !OptDisableReflection) {
							ObjectDumper.Dump(sym);
						}
					}
				}
			}

			mmapStream.Close();
			mmap.Dispose();

			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();
		}

		private static void Pdb_OnDbiInit(DBIReader DBI) {
			if (OptDumpModules) {
				DBI.OnModuleData += DBI_OnModuleData;
			}
			DBI.OnModuleReaderInit += DBI_OnModuleReaderInit;
		}

		private static void DBI_OnModuleReaderInit(IModule module) {
			module.OnSymbolData += Module_OnSymbolData;
		}

		private static void Module_OnSymbolData(byte[] data) {
			Console.WriteLine("=> SYM");
			data.HexDump();
		}

		private static void Pdb_OnTpiInit(TPIReader TPI) {
			TPI.OnLeafData += TPI_OnLeafData;
		}

		private static void TPI_OnLeafData(byte[] data) {
			Console.WriteLine("=> LEAF");
			data.HexDump();
		}

		private static void DBI_OnModuleData(ModuleInfo modInfo, byte[] data) {
			Console.WriteLine($"===== {modInfo.ModuleName}");
			data.HexDump();
		}

	}
}
