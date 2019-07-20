#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public class Context : IDisposable {
		public PDBFile Pdb { get; internal set; }
		public MSFReader MsfReader { get; internal set; }
		public ModuleListReader ModuleListReader { get; internal set; }
		public StreamTableReader StreamTableReader { get; internal set; }

		public DBIReader DbiReader { get; internal set; }
		public TPIReader TpiReader { get; internal set; }
		public HashDataReader TpiHashReader { get; internal set; }

		public PdbStreamReader PdbStreamReader { get; internal set; }
		public UdtNameTableReader UdtNameTableReader { get; internal set; }

		private readonly Stream stream;

		public Context(string pdbFilePath) {
			stream = new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read);
			Pdb = new PDBFile(this, stream);
		}

		public void Dispose() {
			stream.Close();
		}
	}
}
