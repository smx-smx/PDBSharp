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
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.PE;

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
		public static string ExeFilePath = null;

		public static bool OptPrintFpo = false;
		public static bool OptPrintSc = false;
		public static bool OptPrintSyms = false;
		public static bool OptPrintTypes = false;
		public static bool OptPrintTpiHash = false;

		private static void ParseArguments(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				switch (arg) {
					case "-print-fpo":
						OptPrintFpo = true;
						break;
					case "-print-sc":
						OptPrintSc = true;
						break;
					case "-print-syms":
						OptPrintSyms = true;
						break;
					case "-print-types":
						OptPrintTypes = true;
						break;
					case "-print-tpihash":
						OptPrintTpiHash = true;
						break;
					case "-print-decls":
						OptPrintDecls = true;
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
					default:
						if (PdbFilePath == null) {
							PdbFilePath = arg;
						} else {
							ExeFilePath = arg;
						}
						break;
				}
			}
		}

		static void Main(string[] args) {
			ParseArguments(args);

			if (PdbFilePath == null) {
				Console.Error.WriteLine(@"PDBSharp.Dumper
  Usage: PDBSharp.Dumper <options> <file.pdb> [<file.exe|dll>]
    [-dump]            Save individual PDB Streams to files
    [-dump-modules]    Verbose output for DBI Modules
    [-dump-leaves]     Verbose output for TPI Leaves
    [-dump-syms]       Verbose output for DBI Symbols
    [-print-decls]     Extract and print type definitions
    [-print-fpo]       Print FPO entries
    [-print-sc]        Print Section Contribution entries
    [-print-syms]      Print symbol entries
    [-print-types]     Print type
    [-print-tpihash]   Print TPIHash");
				Environment.Exit(1);
			}

			Console.WriteLine("Starting...");
			Stopwatch sw = new Stopwatch();
			sw.Start();

			PDBFile pdb = PDBFile.Open(PdbFilePath);
			IServiceContainer sc = pdb.Services;

			DBI.Data dbi = sc.GetService<DBI.Data>();
			TPI.Serializer tpi = sc.GetService<TPI.Serializer>();
			StreamTable.Serializer streamTable = sc.GetService<StreamTable.Serializer>();

			PDBFacade facade = sc.GetService<PDBFacade>();

			if (OptPrintDecls) {
				var tree = new GraphBuilder(sc).Build();
				if (tree != null) {
					CodeWriter cw = new CodeWriter(tree, Console.Out);
					cw.Write();
				}
			}

			if (OptDumpLeaves) {
				pdb.OnTpiInit += Pdb_OnTpiInit;
			}
			if (OptDumpModules || OptDumpSymbols) {
				pdb.OnDbiInit += Pdb_OnDbiInit;
			}

			if (OptDumpStreams) {
				DirectoryInfo dumpDir = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(PdbFilePath));
				for(int i=1; i< streamTable.Data.NumStreams; i++) {
					string dumpPath = Path.Combine(dumpDir.ToString(), $"stream{i}.bin");

					byte[] stream = streamTable.GetStream(i);
					File.WriteAllBytes(dumpPath, stream);
				}
			}

			/*
			foreach(var type in ctx.TpiReader.Types) {
				//Console.WriteLine(type);
			}*/

			TPIHash.Data tpiHash = sc.GetService<TPIHash.Data>();
			UdtNameTable.Accessor udtNameTable = sc.GetService<UdtNameTable.Accessor>();
			TypeResolver resolver = sc.GetService<TypeResolver>();
			if (tpiHash != null && tpiHash.NameIndexToTypeIndex != null && udtNameTable != null) {
				foreach (var pair in tpiHash.NameIndexToTypeIndex) {
					string name = udtNameTable.GetString(pair.Key);
					ILeafResolver leafContext = resolver.GetTypeByIndex(pair.Value);
					if (OptPrintTpiHash) {
						Console.WriteLine($"=> {name} [NI={pair.Key}] [TI={pair.Value}]");
					}
				}
			}

			if(dbi != null && dbi.SectionContribs != null) {
				if (dbi.SectionContribs.SectionContribs != null) {
					foreach (var contrib in dbi.SectionContribs.SectionContribs) {
						if (OptPrintSc) {
							ObjectDumper.Dump(contrib);
						}
					}
				}

				DebugData.Accessor debug = dbi.DebugInfo;
				if (debug != null && OptPrintFpo && debug.FPO != null) {
					foreach (var frame in debug.FPO.Frames) {
						ObjectDumper.Dump(frame);
					}
				}

				if (dbi.Modules != null) {
					foreach (var container in dbi.Modules) {
						Console.WriteLine($"[MODULE => {container.Info.ModuleName}]");
						Console.WriteLine($"[OBJECT => {container.Info.ObjectFileName}]");
						Console.WriteLine($"[SRC    => {container.Info.SourceFileName}]");
						if (container.Module != null) {
							Console.WriteLine($"[TYPE   => {container.Module.GetType().Name}");
						}
						Console.WriteLine();

						IModule mod = container.Module;
						if (mod != null) {
							foreach (var sym in mod.Symbols) {
								if (OptPrintSyms) {
									Console.WriteLine(sym);
								}
							}
						}
					}
				}
			}

			foreach(var type in tpi.Types) {
				if (OptPrintTypes) {
					Console.WriteLine(type.Ctx.Data);
				}
			}

			if(ExeFilePath != null) {
				using var pe = PEFile.Open(ExeFilePath);
				// $TODO
			}

			sw.Stop();
			Console.WriteLine($"Finished in {sw.Elapsed.TotalSeconds} seconds");
		}

		private static void Pdb_OnDbiInit(DBI.Serializer DBI) {
			if (OptDumpModules) {
				DBI.OnModuleData += DBI_OnModuleData;
				DBI.OnModuleReaderInit += DBI_OnModuleReaderInit;
			}
		}

		private static void DBI_OnModuleReaderInit(IModule module) {
			module.OnSymbolData += Module_OnSymbolData;
		}

		private static void Module_OnSymbolData(byte[] data) {
			Console.WriteLine("=> SYM");
			data.HexDump();
		}

		private static void Pdb_OnTpiInit(TPI.Serializer TPI) {
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
