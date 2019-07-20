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

		public TPIReader Tpi => ctx.TpiReader;
		public DBIReader Dbi => ctx.DbiReader;

		public IEnumerable<byte[]> Streams {
			get {
				for(int i=0; i<ctx.StreamTableReader.NumStreams; i++) {
					yield return ctx.StreamTableReader.GetStream(i);
				}
			}
		}

		public IEnumerable<IModuleContainer> Modules => ReadModules();

		private readonly Lazy<IEnumerable<ILeafContainer>> lazyLeaves;

		public IEnumerable<ILeafContainer> Types => lazyLeaves.Value;

		private readonly Context ctx;

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

		public PDBFile(Context ctx, Stream stream) {
			this.ctx = ctx;
			this.stream = stream;
			this.FileType = DetectPdbType();

			ctx.Pdb = this;

			//$TODO
			if (this.FileType == PDBType.Small) {
				throw new NotImplementedException($"Small/Old/JG PDBs not supported/tested yet");
			}

			ctx.MsfReader = new MSFReader(this.stream, FileType);

			// read stream table
			{
				byte[] streamTable = ctx.MsfReader.StreamTable();
				ctx.StreamTableReader = new StreamTableReader(ctx, new MemoryStream(streamTable));
			}

			// read NameMap
			{
				byte[] nameMap = ctx.StreamTableReader.GetStream((int)DefaultStreams.PDB);
				ctx.PdbStreamReader = new PdbStreamReader(ctx, new MemoryStream(nameMap));
			}

			// read UdtNameMap
			{
				byte[] names = ctx.StreamTableReader.GetStreamByName("/names");
				ctx.UdtNameTableReader = new UdtNameTableReader(new MemoryStream(names));
			}

			// read TPI
			{
				byte[] tpi = ctx.StreamTableReader.GetStream((int)DefaultStreams.TPI);
				ctx.TpiReader = new TPIReader(ctx, new MemoryStream(tpi));
				OnTpiInit?.Invoke(ctx.TpiReader);
			}

			foreach (var pair in ctx.TpiHashReader.NameIndexToTypeIndex) {
				string name = ctx.UdtNameTableReader.GetString(pair.Key);
				ILeafContainer leaf = ctx.TpiReader.GetTypeByIndex(pair.Value);
				Console.WriteLine($"=> {name} [NI={pair.Key}] [TI={pair.Value}]");
				Console.WriteLine(leaf.Data.GetType().Name);
			}

			lazyLeaves = new Lazy<IEnumerable<ILeafContainer>>(ReadTypes);
		}

		public IEnumerable<IModuleContainer> ReadModules() {
			if (ctx.DbiReader == null) {
				byte[] dbi = ctx.StreamTableReader.GetStream((int)DefaultStreams.DBI);
				if (dbi.Length == 0) {
					return Enumerable.Empty<IModuleContainer>();
				}
				ctx.DbiReader = new DBIReader(ctx, new MemoryStream(dbi));
				OnDbiInit?.Invoke(ctx.DbiReader);
			}
			return ctx.DbiReader.Modules;
		}

		public IEnumerable<ILeafContainer> ReadTypes() {
			return ctx.TpiReader.ReadTypes();
		}
	}
}
