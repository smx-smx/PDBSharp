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
using System.Linq;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
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

	public struct DBIHeader
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
	}

	public class DBIReader : ReaderSpan
	{
		public readonly DBIHeader Header;

		private readonly ILazy<IEnumerable<IModuleContainer>> lazyModuleContainers;

		public readonly DebugReader DebugInfo;
		public readonly SectionContribsReader SectionContribs;

		public readonly ECReader EC;
		public readonly TypeServerMapReader TypeServerMap;

		private readonly StreamTableReader StreamTable;

		public IEnumerable<IModuleContainer> Modules => lazyModuleContainers.Value;

		public event OnModuleDataDelegate OnModuleData;
		public event OnModuleReaderInitDelegate OnModuleReaderInit;

		private readonly IServiceContainer ctx;

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
		public DBIReader(IServiceContainer ctx, byte[] data) : base(data) {
			this.ctx = ctx;
			if (Length == 0)
				return;

			if (Length < Marshal.SizeOf<DBIHeader>()) {
				throw new InvalidDataException();
			}

			Header = Read<DBIHeader>();

			if (Header.Signature != unchecked((uint)-1) || !Enum.IsDefined(typeof(DBIVersion), (uint)Header.Version)) {
				throw new InvalidDataException();
			}

			this.StreamTable = ctx.GetService<StreamTableReader>();

			uint nStreams = StreamTable.NumStreams;
			if (
				Header.GsSymbolsStreamNumber >= nStreams ||
				Header.PsSymbolsStreamNumber >= nStreams ||
				Header.SymbolRecordsStreamNumber >= nStreams
			) {
				throw new InvalidDataException();
			}

			lazyModuleContainers = LazyFactory.CreateLazy(ReadModules);
			Position += Header.ModuleListSize;

			if (Header.SectionContributionSize > 0) {
				SectionContribs = new SectionContribsReader(Header.SectionContributionSize, this);
			}
			Position += Header.SectionContributionSize;

			Position += Header.SectionMapSize;
			Position += Header.FileInfoSize;

			if (Header.TypeServerMapSize > 0) {
				TypeServerMap = new TypeServerMapReader(this);
			}
			Position += Header.TypeServerMapSize;

			if (Header.EcSubstreamSize > 0) {
				EC = new ECReader(this);
			}
			Position += Header.EcSubstreamSize;

			if (Header.DebugHeaderSize > 0) {
				DebugInfo = new DebugReader(ctx, this);
			}

		}


		private IEnumerable<IModuleContainer> ReadModules() {
			Position = Marshal.SizeOf<DBIHeader>();

			ModuleListReader rdr = new ModuleListReader(ctx, this, Header.ModuleListSize);
			ctx.AddService<ModuleListReader>(rdr);

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
