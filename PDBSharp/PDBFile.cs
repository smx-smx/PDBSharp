#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public enum DefaultStreams : uint
	{
		PDB = 1,
		TPI = 2,
		DBI = 3,
		IPI = 4
	}

	public class PDBFile
	{
		private readonly Stream stream;

		private readonly MSFReader rdr;
		private readonly StreamTableReader stRdr;

		private IEnumerable<ModuleReader> modules;

		public IEnumerable<ModuleReader> Modules {
			get {
				if (modules == null)
					modules = GetModules();
				return modules;
			}
		}

		public PDBFile(Stream stream) {
			this.stream = stream;

			this.rdr = new MSFReader(this.stream, PDBType.Big);

			byte[] streamTable = rdr.StreamTable();
			//streamTable.HexDump();
			stRdr = new StreamTableReader(rdr, new MemoryStream(streamTable));
		}

		private IEnumerable<ModuleReader> GetModules() {
			byte[] dbi = stRdr.GetStream((uint)DefaultStreams.DBI);

			DBIReader dbiRdr = new DBIReader(stRdr, new MemoryStream(dbi));
			return dbiRdr.Modules;
		}
	}
}
