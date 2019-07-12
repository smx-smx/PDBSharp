#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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

	public delegate void OnTpiInitDelegate(TPIReader TPI);
	public delegate void OnDbiInitDelegate(DBIReader DBI);

	public class PDBFile
	{
		public event OnTpiInitDelegate OnTpiInit;
		public event OnDbiInitDelegate OnDbiInit;

		public const string SMALL_MAGIC = "Microsoft C/C++ program database 2.00\r\n\x1a" + "JG";
		public const string BIG_MAGIC = "Microsoft C/C++ MSF 7.00\r\n\x1a" + "DS";

		private readonly Stream stream;

		private readonly MSFReader rdr;
		public readonly StreamTableReader StreamTable;

		private IEnumerable<IModule> modules;
		private IEnumerable<LeafBase> types;

		public IEnumerable<byte[]> Streams {
			get {
				for(int i=0; i<StreamTable.NumStreams; i++) {
					yield return StreamTable.GetStream(i);
				}
			}
		}

		public IEnumerable<IModuleContainer> Modules => ReadModules();

		private readonly Lazy<IEnumerable<ILeafContainer>> lazyLeaves;

		public IEnumerable<ILeafContainer> Types => lazyLeaves.Value;

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
			StreamTable = new StreamTableReader(rdr, new MemoryStream(streamTable));

			lazyLeaves = new Lazy<IEnumerable<ILeafContainer>>(ReadTypes);
		}

		public IEnumerable<IModuleContainer> ReadModules() {
			if (DBI == null) {
				byte[] dbi = StreamTable.GetStream((int)DefaultStreams.DBI);
				if (dbi.Length == 0) {
					return Enumerable.Empty<IModuleContainer>();
				}
				DBI = new DBIReader(this, StreamTable, new MemoryStream(dbi));
				OnDbiInit?.Invoke(DBI);
			}
			return DBI.Modules;
		}

		public IEnumerable<ILeafContainer> ReadTypes() {
			if (TPI == null) {
				byte[] tpi = StreamTable.GetStream((int)DefaultStreams.TPI);
				if(tpi.Length == 0) {
					return Enumerable.Empty<ILeafContainer>();
				}
				TPI = new TPIReader(this, StreamTable, new MemoryStream(tpi));
				OnTpiInit?.Invoke(TPI);
			}
			return TPI.ReadTypes();
		}
	}
}
