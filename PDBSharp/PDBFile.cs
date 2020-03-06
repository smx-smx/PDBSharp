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
using System.ComponentModel.Design;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

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

	public class PDBFile : IDisposable
	{
		public event OnTpiInitDelegate OnTpiInit;
		public event OnDbiInitDelegate OnDbiInit;

		public const int JG_OFFSET = 0x28;
		public const int DS_OFFSET = 0x1B;

		public const string SMALL_MAGIC = "Microsoft C/C++ program database 2.00\r\n\x1a" + "JG";
		public const string BIG_MAGIC   = "Microsoft C/C++ MSF 7.00\r\n\x1a" + "DS";

		private readonly MemoryMappedSpan memSpan;
		private readonly MemoryMappedFile mf;
		private readonly FileStream fs;

		private readonly StreamTableReader StreamTable;

		public IServiceContainer Services { get; } = new ServiceContainer();

		public IEnumerable<byte[]> Streams {
			get {
				for (int i = 0; i < StreamTable.NumStreams; i++) {
					yield return StreamTable.GetStream(i);
				}
			}
		}

		public static PDBFile Open(string pdbFilePath) {
			FileStream stream = new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new PDBFile(stream);
		}

		public void Dispose() {
			memSpan.Dispose();
			mf.Dispose();
			fs.Close();
		}

		public PDBFile(FileStream stream) {
			this.fs = stream;
			this.mf = MemoryMappedFile.CreateFromFile(stream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.Inheritable, true);
			this.memSpan = new MemoryMappedSpan(mf, fs.Length);

			this.StreamTable = Services.GetService<StreamTableReader>();
			Services.AddService<PDBFile>(this);

			PDBType type = MSFReader.DetectPdbType(memSpan.GetSpan());

			MSFReader msf;
			switch (type) {
				case PDBType.Big:
					msf = new MSFReaderDS(this.memSpan);
					break;
				case PDBType.Small:
					msf = new MSFReaderJG(this.memSpan);
					break;
				default:
					throw new InvalidOperationException();					
			}

			Services.AddService<MSFReader>(msf);

			StreamTableReader streamTable;
			// init stream table
			{
				byte[] streamTableData = msf.StreamTable();
				streamTable = new StreamTableReader(Services, streamTableData);
			}
			Services.AddService<StreamTableReader>(streamTable);

			DBIReader dbi;
			// init DBI
			{
				byte[] dbiData = streamTable.GetStream(DefaultStreams.DBI);
				dbi = new DBIReader(Services, dbiData);
				OnDbiInit?.Invoke(dbi);
			}
			Services.AddService<DBIReader>(dbi);

			TPIReader tpi;
			// init TPI
			{
				byte[] tpiData = streamTable.GetStream(DefaultStreams.TPI);
				tpi = new TPIReader(Services, new SpanStream(tpiData));
				OnTpiInit?.Invoke(tpi);
			}
			Services.AddService<TPIReader>(tpi);

			TPIHashReader tpiHash = null;
			// init TPIHash
			if (tpi.Header.Hash.StreamNumber != -1) {
				byte[] tpiHashData = streamTable.GetStream(tpi.Header.Hash.StreamNumber);
				tpiHash = new TPIHashReader(Services, tpiHashData);
				Services.AddService<TPIHashReader>(tpiHash);
			}

			// init resolver
			TypeResolver resolver = new TypeResolver(Services);
			Services.AddService<TypeResolver>(resolver);


			// init Hasher
			HasherV2 hasher = new HasherV2(Services);
			Services.AddService<HasherV2>(hasher);

			PdbStreamReader nameMap;
			// init NameMap
			{
				byte[] nameMapData = streamTable.GetStream(DefaultStreams.PDB);
				nameMap = new PdbStreamReader(nameMapData);
			}
			Services.AddService<PdbStreamReader>(nameMap);

			NamedStreamTableReader namedStreamTable = new NamedStreamTableReader(Services);
			Services.AddService<NamedStreamTableReader>(namedStreamTable);

			UdtNameTableReader udtNameTable = null;
			// init UdtNameMap
			{
				byte[] namesData = namedStreamTable.GetStreamByName("/names");
				if (namesData != null) {
					udtNameTable = new UdtNameTableReader(Services, namesData);
					Services.AddService<UdtNameTableReader>(udtNameTable);
				}
			}
		}
	}
}
