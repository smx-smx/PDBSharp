#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.PE;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Tools
{
	public class PdbDownloader
	{
		public string GetPdbUrlForExecutable(string pePath) {
			using var peFile = PEFile.Open(pePath);
			var data = peFile.DebugDirectory;
			if (data == null || data.x_Data is not RSDSI.Data rsdsi) return string.Empty;

			var guidString = rsdsi.GuidSignature.ToString("N").ToUpperInvariant();
			var ageString = rsdsi.Age.ToString("X");

			var sb = new StringBuilder("https://msdl.microsoft.com/download/symbols");
			sb.AppendFormat("/{0}/{1}{2}/{0}", rsdsi.PdbName, guidString, ageString);
			return sb.ToString();
		}

		public async Task<HttpContent?> DownloadForExecutable(string pePath) {
			var url = GetPdbUrlForExecutable(pePath);
			if (string.IsNullOrEmpty(url)) return null;

			using var client = new HttpClient();
			var res = await client.GetAsync(url);
			return res.Content;
		}
	}
}
