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
using System.Linq;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	namespace RSDSI {
		public class Data {
			public string Signature = "RSDS";
			public Guid GuidSignature;
			public uint Age;
			public string PdbName = string.Empty;
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var Signature = stream.ReadString(4, System.Text.Encoding.ASCII);
				if(Signature != "RSDS") {
					throw new InvalidDataException();
				}

				var GuidSignature = stream.ReadStruct<Guid>();
				var Age = stream.ReadUInt32();
				var PdbName = stream.ReadCString(System.Text.Encoding.UTF8);

				Data = new Data {
					Signature = Signature,
					GuidSignature = GuidSignature,
					Age = Age,
					PdbName	= PdbName
				};
				return Data;
			}
		}
	}

	public enum DBIVersion : UInt32
	{
		V41 = 930803,
		V50 = 19960307,
		V60 = 19970606,
		V70 = 19990903,
		V110 = 20091201
	}

	[Flags]
	public enum DBIFlags : UInt16
	{
		IsIncrementalLink = 1 << 0,
		IsStripped = 1 << 1,
		HasCTypes = 1 << 2
	}

	public interface IDBIHeader
	{
		UInt16 GsSymbolsStreamNumber { get; set; }
		UInt16 PsSymbolsStreamNumber { get; set; }
		UInt16 SymbolRecordsStreamNumber { get; set; }
		UInt32 ModuleListSize { get; set; }
		UInt32 SectionContributionSize { get; set; }
		UInt32 SectionMapSize { get; set; }
	}

	/// <summary>
	/// Not sure who uses this, VC++ 1.0 already uses the new one
	/// </summary>
	public struct DBIHeaderOld : IDBIHeader
	{
		public UInt16 GsSymbolsStreamNumber;
		public UInt16 PsSymbolsStreamNumber;
		public UInt16 SymbolRecordsStreamNumber;
		public UInt32 ModuleListSize;
		public UInt32 SectionContributionSize;
		public UInt32 SectionMapSize;

		ushort IDBIHeader.GsSymbolsStreamNumber {
			get => GsSymbolsStreamNumber;
			set => GsSymbolsStreamNumber = value;
		}

		ushort IDBIHeader.PsSymbolsStreamNumber {
			get => PsSymbolsStreamNumber;
			set => PsSymbolsStreamNumber = value;
		}

		ushort IDBIHeader.SymbolRecordsStreamNumber {
			get => SymbolRecordsStreamNumber;
			set => SymbolRecordsStreamNumber = value;
		}

		uint IDBIHeader.ModuleListSize {
			get => ModuleListSize;
			set => ModuleListSize = value;
		}

		uint IDBIHeader.SectionContributionSize {
			get => SectionContributionSize;
			set => SectionContributionSize = value;
		}

		uint IDBIHeader.SectionMapSize {
			get => SectionMapSize;
			set => SectionMapSize = value;
		}
	}

	public struct DBIHeaderNew : IDBIHeader
	{
		public UInt32 Signature;
		public DBIVersion Version;
		public UInt32 Age;

		public UInt16 GsSymbolsStreamNumber;
		public UInt16 InternalVersion;
		public UInt16 PsSymbolsStreamNumber;
		public UInt16 PdbDllVersion;
		public UInt16 SymbolRecordsStreamNumber;
		public UInt16 RbldVersion;

		public UInt32 ModuleListSize;
		public UInt32 SectionContributionSize;
		public UInt32 SectionMapSize;
		public UInt32 FileInfoSize;
		public UInt32 TypeServerMapSize;
		public UInt32 MFCTypeServerIndex;

		public UInt32 DebugHeaderSize;
		public UInt32 EcSubstreamSize;

		public DBIFlags Flags;
		public UInt16 MachineType;
		public UInt32 Reserved;

		ushort IDBIHeader.GsSymbolsStreamNumber {
			get => GsSymbolsStreamNumber;
			set => GsSymbolsStreamNumber = value;
		}

		ushort IDBIHeader.PsSymbolsStreamNumber {
			get => PsSymbolsStreamNumber;
			set => PsSymbolsStreamNumber = value;
		}

		ushort IDBIHeader.SymbolRecordsStreamNumber {
			get => SymbolRecordsStreamNumber;
			set => SymbolRecordsStreamNumber = value;
		}

		uint IDBIHeader.ModuleListSize {
			get => ModuleListSize;
			set => ModuleListSize = value;
		}

		uint IDBIHeader.SectionContributionSize {
			get => SectionContributionSize;
			set => SectionContributionSize = value;
		}

		uint IDBIHeader.SectionMapSize {
			get => SectionMapSize;
			set => SectionMapSize = value;
		}
	}

	namespace DBI
	{
		public class Data
		{
			public IDBIHeader Header { get; internal set; } = new DBIHeaderNew();

			public DebugReader? DebugInfo { get; internal set; }
			public SectionContribsReader? SectionContribs { get; internal set; }

			public ECReader? EC { get; internal set; }
			public TypeServerMapReader? TypeServerMap { get; internal set; }
			public CachedEnumerable<IModuleContainer> Modules { get; internal set; } = new CachedEnumerable<IModuleContainer>(Enumerable.Empty<IModuleContainer>());
		}


		public class Stream : SpanStream
		{
			public Data Data = new DBI.Data();

			private readonly StreamTableReader StreamTable;

			public event OnModuleDataDelegate? OnModuleData;
			public event OnModuleReaderInitDelegate? OnModuleReaderInit;

			private readonly IServiceContainer ctx;

			private bool IsValidStreamNumber(ushort sn) {
				// snNil, allowed
				if ((short)sn == -1) return true;
				return sn < StreamTable.NumStreams;
			}

			/**
			 * Layout of the DBI stream
			 * -> Header 
			 * -> ModuleList
			 * -> SectionContributions
			 * -> SectionMap
			 * -> FileInfo
			 * -> TypeServerMap
			 * -> EcSubstream
			 * -> DebugHeader
			 **/
			public Stream(IServiceContainer ctx, byte[] data) : base(data) {
				this.ctx = ctx;

				if (Length < Math.Min(Marshal.SizeOf<DBIHeaderOld>(), Marshal.SizeOf<DBIHeaderNew>())) {
					throw new InvalidDataException();
				}

				{
					int signature = PerformAt(0, ReadInt32); // invalid GSSyms, used to detect new header
					if (signature == -1) {
						DBIHeaderNew header = Read<DBIHeaderNew>();
						if (!Enum.IsDefined(typeof(DBIVersion), (uint)header.Version)) {
							throw new NotSupportedException();
						}
						Data.Header = header;
					} else {
						Data.Header = Read<DBIHeaderOld>();
					}
				}

				this.StreamTable = ctx.GetService<StreamTableReader>();

				uint nStreams = StreamTable.NumStreams;
				if (!IsValidStreamNumber(Data.Header.GsSymbolsStreamNumber)
					|| !IsValidStreamNumber(Data.Header.PsSymbolsStreamNumber)
					|| !IsValidStreamNumber(Data.Header.SymbolRecordsStreamNumber)
				) {
					throw new InvalidDataException();
				}

				Data.Modules = new CachedEnumerable<IModuleContainer>(ReadModules());
				Position += Data.Header.ModuleListSize;

				if (Data.Header.SectionContributionSize > 0) {
					Data.SectionContribs = new SectionContribsReader(ctx, Data.Header.SectionContributionSize, this);
				}
				Position += Data.Header.SectionContributionSize;

				Position += Data.Header.SectionMapSize;

				if (Data.Header is DBIHeaderNew DSHeader) {
					Position += DSHeader.FileInfoSize;

					if (DSHeader.TypeServerMapSize > 0) {
						Data.TypeServerMap = new TypeServerMapReader(this);
					}
					Position += DSHeader.TypeServerMapSize;

					if (DSHeader.EcSubstreamSize > 0) {
						Data.EC = new ECReader(this);
					}
					Position += DSHeader.EcSubstreamSize;

					if (DSHeader.DebugHeaderSize > 0) {
						Data.DebugInfo = new DebugReader(ctx, this);
					}
				}
			}

			public IModuleContainer? GetModuleByFileOffset(int sectionIndex, long fileOffset) {
				SectionContrib40? sc = Data.SectionContribs?.SectionContribs
					.Where(sec => sec.Contains(sectionIndex, fileOffset))
					//there should be only 1 match
					.OrderBy(sec => fileOffset - sec.Offset)
					.FirstOrDefault();

				return sc?.Module;
			}

			private IEnumerable<IModuleContainer> ReadModules() {
				Position = Marshal.SizeOf<DBIHeaderNew>();

				ModuleListReader rdr = ctx.GetService<ModuleListReader>();
				if (rdr == null) {
					rdr = new ModuleListReader(ctx, this, Data.Header.ModuleListSize);
					ctx.AddService<ModuleListReader>(rdr);
				}

				IEnumerable<ModuleInfo> moduleInfoList = rdr.Modules;
				foreach (ModuleInfo mod in moduleInfoList) {
					LazyModuleProvider provider = new LazyModuleProvider(ctx, mod);

					if (OnModuleReaderInit != null) {
						provider.OnModuleReaderInit += OnModuleReaderInit;
					}
					if (OnModuleData != null) {
						provider.OnModuleData += OnModuleData;
					}
					yield return provider;
				}
			}
		}
	}
}
