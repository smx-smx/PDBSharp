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

	public class PDBFile
	{
		public event OnTpiInitDelegate OnTpiInit;
		public event OnDbiInitDelegate OnDbiInit;

		public const string SMALL_MAGIC = "Microsoft C/C++ program database 2.00\r\n\x1a" + "JG";
		public const string BIG_MAGIC = "Microsoft C/C++ MSF 7.00\r\n\x1a" + "DS";

		private readonly Stream stream;

		private readonly StreamTableReader StreamTable;

		public IServiceContainer Services { get; } = new ServiceContainer();

		public IEnumerable<byte[]> Streams {
			get {
				for (int i = 0; i < StreamTable.NumStreams; i++) {
					yield return StreamTable.GetStream(i);
				}
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

		public static PDBFile Open(string pdbFilePath) {
			Stream stream = new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read);
			return new PDBFile(stream);
		}

		public PDBFile(Stream stream) {
			this.StreamTable = Services.GetService<StreamTableReader>();

			this.stream = stream;
			this.FileType = DetectPdbType();

			Services.AddService<PDBFile>(this);

			//$TODO
			if (this.FileType == PDBType.Small) {
				throw new NotImplementedException($"Small/Old/JG PDBs not supported/tested yet");
			}


			MSFReader msf = new MSFReader(this.stream, FileType);
			Services.AddService<MSFReader>(msf);

			StreamTableReader streamTable;
			// init stream table
			{
				byte[] streamTableData = msf.StreamTable();
				streamTable = new StreamTableReader(Services, new MemoryStream(streamTableData));
			}
			Services.AddService<StreamTableReader>(streamTable);

			DBIReader dbi;
			// init DBI
			{
				byte[] dbiData = streamTable.GetStream(DefaultStreams.DBI);
				dbi = new DBIReader(Services, new MemoryStream(dbiData));
				OnDbiInit?.Invoke(dbi);
			}
			Services.AddService<DBIReader>(dbi);

			TPIReader tpi;
			// init TPI
			{
				byte[] tpiData = streamTable.GetStream(DefaultStreams.TPI);
				tpi = new TPIReader(Services, new MemoryStream(tpiData));
				OnTpiInit?.Invoke(tpi);
			}
			Services.AddService<TPIReader>(tpi);

			HashDataReader tpiHash = null;
			// init TPIHash
			if (tpi.Header.Hash.StreamNumber != -1) {
				byte[] tpiHashData = streamTable.GetStream(tpi.Header.Hash.StreamNumber);
				tpiHash = new HashDataReader(Services, new MemoryStream(tpiHashData));
				Services.AddService<HashDataReader>(tpiHash);
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
				nameMap = new PdbStreamReader(new MemoryStream(nameMapData));
			}
			Services.AddService<PdbStreamReader>(nameMap);

			NamedStreamTableReader namedStreamTable = new NamedStreamTableReader(Services);
			Services.AddService<NamedStreamTableReader>(namedStreamTable);

			UdtNameTableReader udtNameTable = null;
			// init UdtNameMap
			{
				byte[] namesData = namedStreamTable.GetStreamByName("/names");
				if (namesData != null) {
					udtNameTable = new UdtNameTableReader(Services, new MemoryStream(namesData));
					Services.AddService<UdtNameTableReader>(udtNameTable);
				}
			}

#if DEBUG
			if (tpiHash != null && udtNameTable != null) {
				foreach (var pair in tpiHash.NameIndexToTypeIndex) {
					string name = udtNameTable.GetString(pair.Key);
					ILeafContainer leaf = resolver.GetTypeByIndex(pair.Value);
					Console.WriteLine($"=> {name} [NI={pair.Key}] [TI={pair.Value}]");
					Console.WriteLine(leaf.Data.GetType().Name);
				}
			}
#endif
		}
	}
}
