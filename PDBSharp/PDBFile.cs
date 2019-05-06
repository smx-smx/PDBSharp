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
	public enum DefaultStreams : Int16
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
		public readonly StreamTableReader StreamTable;

		private IEnumerable<IModule> modules;
		private IEnumerable<ILeaf> types;

		public IEnumerable<byte[]> Streams {
			get {
				for(int i=0; i<StreamTable.NumStreams; i++) {
					yield return StreamTable.GetStream(i);
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

		public DBIReader DBI { get; private set; }
		public TPIReader TPI { get; private set; }

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
			StreamTable = new StreamTableReader(rdr, new MemoryStream(streamTable));
		}

		public IEnumerable<IModule> ReadModules() {
			if (DBI == null) {
				byte[] dbi = StreamTable.GetStream((int)DefaultStreams.DBI);
				if (dbi.Length == 0) {
					return Enumerable.Empty<IModule>();
				}
				DBI = new DBIReader(this, StreamTable, new MemoryStream(dbi));
			}
			return DBI.ReadModules();
		}

		public IEnumerable<ILeaf> ReadTypes() {
			if (TPI == null) {
				byte[] tpi = StreamTable.GetStream((int)DefaultStreams.TPI);
				if(tpi.Length == 0) {
					return Enumerable.Empty<ILeaf>();
				}
				TPI = new TPIReader(this, StreamTable, new MemoryStream(tpi));
			}
			return TPI.ReadTypes();
		}
	}
}
