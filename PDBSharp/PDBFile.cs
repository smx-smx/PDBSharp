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
using System;
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

		private const string SMALL_MAGIC = "Microsoft C/C++ program database 2.00\r\n\x1a" + "JG";
		private const string BIG_MAGIC = "Microsoft C/C++ MSF 7.00\r\n\x1a" + "DS";

		private readonly Stream stream;

		private readonly MSFReader rdr;
		private readonly StreamTableReader stRdr;

		private IEnumerable<IModule> modules;
		private IEnumerable<ILeaf> types;

		public IEnumerable<byte[]> Streams {
			get {
				for(uint i=0; i<stRdr.NumStreams; i++) {
					yield return stRdr.GetStream(i);
				}
			}
		}

		public IEnumerable<IModule> Modules {
			get {
				if (modules == null)
					modules = ReadModules().Cached();
				return modules;
			}
		}

		public IEnumerable<ILeaf> Types {
			get {
				if (types == null)
					types = ReadTypes().Cached();
				return types;
			}
		}

		public readonly PDBType FileType;

		private PDBType DetectPdbType() {
			int maxSize = Math.Max(SMALL_MAGIC.Length, BIG_MAGIC.Length);

			byte[] buffer = new byte[maxSize];
			stream.Read(buffer, 0, maxSize);
			stream.Position = 0;

			string msfMagic = Encoding.ASCII.GetString(buffer);
			if (msfMagic.StartsWith(BIG_MAGIC)) {
				return PDBType.Big;
			} else if (msfMagic.StartsWith(SMALL_MAGIC)) {
				return PDBType.Small;
			} else {
				throw new InvalidDataException("No valid MSF header found");
			}

		}

		public PDBFile(Stream stream) {
			this.stream = stream;

			this.FileType = DetectPdbType();

			//$TODO
			if (this.FileType == PDBType.Small) {
				throw new NotImplementedException($"Small/Old/JG PDBs not supported/tested yet");
			}

			this.rdr = new MSFReader(this.stream, FileType);

			byte[] streamTable = rdr.StreamTable();
			//streamTable.HexDump();
			stRdr = new StreamTableReader(rdr, new MemoryStream(streamTable));
		}

		public IEnumerable<IModule> ReadModules() {
			byte[] dbi = stRdr.GetStream((uint)DefaultStreams.DBI);

			DBIReader dbiRdr = new DBIReader(stRdr, new MemoryStream(dbi));
			return dbiRdr.ReadModules();
		}

		public IEnumerable<ILeaf> ReadTypes() {
			byte[] tpi = stRdr.GetStream((uint)DefaultStreams.TPI);

			TPIReader tpiRdr = new TPIReader(stRdr, new MemoryStream(tpi));
			return tpiRdr.ReadTypes();
		}
	}
}
