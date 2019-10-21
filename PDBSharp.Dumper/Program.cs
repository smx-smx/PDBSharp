#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Codegen;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
		public static bool OptPrintDecls = false;
		public static string PdbFilePath = null;

		private static void ParseArguments(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				switch (arg) {
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
					case "-print":
						OptPrintDecls = true;
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
				Console.Error.WriteLine(@"PDBSharp.Dumper
  Usage: PDBSharp.Dumper <options> <file.pdb>
    [-dump]            Save individual PDB Streams to files
    [-dump-modules]    Verbose output for DBI Modules
    [-dump-leaves]     Verbose output for TPI Leaves
    [-dump-syms]       Verbose output for DBI Symbols
    [-print]           Extract and print type definitions");
				Environment.Exit(1);
			}

#if PERF
			Console.WriteLine("Starting...");
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif

			PDBFile pdb = PDBFile.Open(PdbFilePath);
			IServiceContainer sc = pdb.Services;

			DBIReader dbi = sc.GetService<DBIReader>();
			TPIReader tpi = sc.GetService<TPIReader>();
			StreamTableReader streamTable = sc.GetService<StreamTableReader>();

			if (OptPrintDecls) {
				var tree = new GraphBuilder(sc).Build();
				CodeWriter cw = new CodeWriter(tree);
#if !PERF
				cw.Write(Console.Out);
#endif
			}

			if (OptDumpLeaves) {
				pdb.OnTpiInit += Pdb_OnTpiInit;
			}
			if (OptDumpModules || OptDumpSymbols) {
				pdb.OnDbiInit += Pdb_OnDbiInit;
			}

			if (OptDumpStreams) {
				DirectoryInfo dumpDir = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(PdbFilePath));
				for(int i=1; i< streamTable.NumStreams; i++) {
					string dumpPath = Path.Combine(dumpDir.ToString(), $"stream{i}.bin");

					byte[] stream = streamTable.GetStream(i);
					File.WriteAllBytes(dumpPath, stream);
				}
			}

			/*
			foreach(var type in ctx.TpiReader.Types) {
				//Console.WriteLine(type);
			}*/

			TPIHashReader tpiHash = sc.GetService<TPIHashReader>();
			UdtNameTableReader udtNameTable = sc.GetService<UdtNameTableReader>();
			TypeResolver resolver = sc.GetService<TypeResolver>();
			if (tpiHash != null && udtNameTable != null) {
				foreach (var pair in tpiHash.NameIndexToTypeIndex) {
					string name = udtNameTable.GetString(pair.Key);
					ILeafContainer leaf = resolver.GetTypeByIndex(pair.Value);
#if !PERF
					Console.WriteLine($"=> {name} [NI={pair.Key}] [TI={pair.Value}]");
					Console.WriteLine(leaf.Data.GetType().Name);
#endif
				}
			}

			foreach(var contrib in dbi.SectionContribs.SectionContribs) {
#if !PERF
				ObjectDumper.Dump(contrib);
#endif
			}

			DebugReader debug = dbi.DebugInfo;
			if(debug != null && debug.FPO != null) {
				foreach(var frame in debug.FPO.Frames) {
#if !PERF
					ObjectDumper.Dump(frame);
#endif
				}
			}

			foreach (var container in dbi.Modules) {
#if !PERF
				Console.WriteLine($"[MODULE => {container.Info.ModuleName}]");
				Console.WriteLine($"[OBJECT => {container.Info.ObjectFileName}]");
				Console.WriteLine($"[SRC    => {container.Info.SourceFileName}]");
				if (container.Module != null) {
					Console.WriteLine($"[TYPE   => {container.Module.GetType().Name}");
				}
				Console.WriteLine();
#endif

				IModule mod = container.Module;
				if (mod != null) {
					foreach (var sym in mod.Symbols) {
#if !PERF
						Console.WriteLine(sym);
#endif
					}
				}
			}

			foreach(var type in tpi.Types) {
#if !PERF
				Console.WriteLine(type);
#endif
			}

#if !PERF
			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();
#else
			sw.Stop();
			Console.WriteLine($"Finished in {sw.Elapsed.TotalSeconds} seconds");
#endif
		}

		private static void Pdb_OnDbiInit(DBIReader DBI) {
#if !PERF
			if (OptDumpModules) {
				DBI.OnModuleData += DBI_OnModuleData;
			}
			DBI.OnModuleReaderInit += DBI_OnModuleReaderInit;
#endif
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
