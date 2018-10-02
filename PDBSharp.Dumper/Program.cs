using Smx.PDBSharp;
using System;
using System.IO;

namespace PDBSharp.Dumper
{
	class Program
	{
		static void Main(string[] args) {
			if (args.Length < 1) {
				Console.Error.WriteLine("Usage: [file.pdb]");
				Environment.Exit(1);
			}

			var file = new FileStream(args[0], FileMode.Open, FileAccess.Read);
			PDBFile pdb = new PDBFile(file);

			foreach (var mod in pdb.Modules) {
				foreach (var sym in mod.Symbols) {
					Console.WriteLine($"[MAIN] => {sym.ToString()}");
				}
			}

			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();
		}
	}
}
