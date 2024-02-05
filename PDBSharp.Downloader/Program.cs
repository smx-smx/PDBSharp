#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Downloader
{
	class Program
	{
		static async Task Main(string[] args) {
			if(args.Length < 2) {
				Console.Error.WriteLine("Usage: [exePath] [outPath]");
			}
			var dl = new PdbDownloader();
			var content = await dl.DownloadForExecutable(args[0]);
			if(content == null) {
				Console.Error.WriteLine("Cannot find PDB");
				Environment.Exit(1);
			}
			using var fs = new FileStream(args[1], FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			fs.SetLength(0);
			await content.CopyToAsync(fs);
		}
	}
}
