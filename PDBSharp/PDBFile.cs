#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Smx.PDBSharp
{
	public enum PDBInternalVersion : UInt32
	{
		V41 = 920924,
		V50 = 19960502,
		V60 = V50,
		V50A = 19970116,
		V61 = 19980914,
		V69 = 19990511,
		V70Deprecated = 20000406,
		V70 = 20001102,
		V80 = 20030901,
		V110 = 20091201
	}

	public enum DefaultStreams : Int16
	{
		PDB = 1,
		TPI = 2,
		DBI = 3,
		IPI = 4
	}

	public delegate void OnTpiInitDelegate(TPI.Serializer TPI);
	public delegate void OnDbiInitDelegate(DBI.Serializer DBI);

	public class PDBFile : IDisposable, IPDBService
	{
		public event OnTpiInitDelegate? OnTpiInit;
		public event OnDbiInitDelegate? OnDbiInit;

		public const int JG_OFFSET = 0x28;
		public const int DS_OFFSET = 0x1B;

		public const string OLD_MAGIC = "Microsoft C/C++ program database 1.00\r\n\x1a" + "JG";
		public const string SMALL_MAGIC = "Microsoft C/C++ program database 2.00\r\n\x1a" + "JG";
		public const string BIG_MAGIC   = "Microsoft C/C++ MSF 7.00\r\n\x1a" + "DS";

		private readonly StreamTable.Serializer StreamTable;

		public readonly PDBType Type;

		private IList<IDisposable> disposables;

		public IServiceContainer Services { get; } = new ServiceContainer();

		public IEnumerable<byte[]> Streams {
			get {
				for (int i = 0; i < StreamTable.Data.NumStreams; i++) {
					yield return StreamTable.GetStream(i);
				}
			}
		}

		public static PDBFile Open(string pdbFilePath) {
			FileStream stream = new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return Open(stream);
		}

		public static PDBFile Open(FileStream fs) {
			var mf = MemoryMappedFile.CreateFromFile(fs, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.Inheritable, true);
			var memSpan = new MemoryMappedSpan<byte>(mf, (int)fs.Length, MemoryMappedFileAccess.Read);
			return new PDBFile(memSpan.Memory, new List<IDisposable>() {
				memSpan, mf
			});
		}

		public static PDBFile Open(MemoryStream mem) {
			return new PDBFile(mem.GetBuffer());
		}

		public static PDBFile Open(Memory<byte> mem) {
			return new PDBFile(mem);
		}

		public void Dispose() {
			foreach(var res in disposables) {
				res.Dispose();
			}
		}

		private PDBFile(Memory<byte> mem, IList<IDisposable>? disposables = null) {
			this.disposables = disposables ?? new List<IDisposable>();

			this.StreamTable = Services.GetService<StreamTable.Serializer>();
			Services.AddService<PDBFile>(this);

			var span = mem.Span;

			this.Type = MSFReader.DetectPdbType(span);

			MSFReader msf;
			StreamTable.Serializer? streamTable = null;

			if(Type != PDBType.Old) {
				switch (Type) {
					case PDBType.Big:
						msf = new MSFReaderDS(mem);
						break;
					case PDBType.Small:
						msf = new MSFReaderJG(mem);
						break;
					default:
						throw new InvalidOperationException();
				}
				Services.AddService<MSFReader>(msf);

				// init stream table
				{
					byte[] streamTableData = msf.StreamTable();
					streamTable = new StreamTable.Serializer(Services, new PDBSpanStream(streamTableData, msf.FileType));
					streamTable.Read();
				}
				Services.AddService<StreamTable.Serializer>(streamTable);

				DBI.Serializer? dbi = null;
				// init DBI
				{
					byte[] dbiData = streamTable.GetStream(DefaultStreams.DBI);
					if (dbiData.Length > 0) {
						dbi = new DBI.Serializer(Services, new SpanStream(dbiData));
						dbi.Read();
						OnDbiInit?.Invoke(dbi);
						Services.AddService<DBI.Data>(dbi.Data);
					}
				}
			}

			TPI.Serializer tpi;

			// init TPI
			{
				SpanStream tpiStream;
				if(streamTable != null) {
					byte[] tpiData  = streamTable.GetStream(DefaultStreams.TPI);
					tpiStream = new SpanStream(tpiData);
				} else {
					JGHeaderOld header = span.Read<JGHeaderOld>(0);
					// $TODO: the MSFReader interface should be abstracted into a more generic PDBHeader
					Services.AddService<JGHeaderOld>(header);

					tpiStream = new SpanStream(span.Slice(header.SIZE).ToArray());					
				}
				tpi = new TPI.Serializer(Services, tpiStream);
				tpi.Read();
				OnTpiInit?.Invoke(tpi);
			}
			Services.AddService<TPI.Serializer>(tpi);

			TPIHash.Serializer? tpiHash = null;
			// init TPIHash
			if(streamTable != null) {
				var tpiHeader = tpi.Data.Header;
				if(tpiHeader.Hash.StreamNumber != -1) {
					byte[] tpiHashData = streamTable.GetStream(tpi.Data.Header.Hash.StreamNumber);
					tpiHash = new TPIHash.Serializer(tpi, new SpanStream(tpiHashData));
					tpiHash.Read();
					Services.AddService<TPIHash.Data>(tpiHash.Data);
				}
			}

			// init resolver
			TypeResolver resolver = new TypeResolver(Services);
			Services.AddService<TypeResolver>(resolver);


			// init Hasher
			HasherV2 hasher = new HasherV2(Services);
			Services.AddService<HasherV2>(hasher);

			if (streamTable != null) {
				{ // init NameMap
					byte[] nameMapData = streamTable.GetStream(DefaultStreams.PDB);

					PDBStream.Serializer nameMap = new PDBStream.Serializer(new SpanStream(nameMapData));
					nameMap.Read();
					Services.AddService<PDBStream.Data>(nameMap.Data);
				}

				NamedStreamTableReader namedStreamTable = new NamedStreamTableReader(Services);
				Services.AddService<NamedStreamTableReader>(namedStreamTable);

				{ // init UdtNameMap
					byte[]? namesData = namedStreamTable.GetStreamByName("/names");
					if (namesData != null) {
						UdtNameTable.Serializer reader = new UdtNameTable.Serializer(new SpanStream(namesData));
						reader.Read();
						var udtNameTable = new UdtNameTable.Accessor(Services, reader);
						Services.AddService<UdtNameTable.Accessor>(udtNameTable);
					}
				}
			}

			var facade = new PDBFacade(Services);
			Services.AddService<PDBFacade>(facade);
		}
	}
}
