#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

	public class DBIReader : ReaderBase
	{
		public readonly DBIHeader Header;

		private readonly Context ctx;

		private readonly Lazy<IEnumerable<IModuleContainer>> lazyModuleContainers;

		public readonly DebugReader DebugInfo;
		public readonly SectionContribsReader SectionContribs;

		public readonly ECReader EC;
		public readonly TypeServerMapReader TypeServerMap;

		public IEnumerable<IModuleContainer> Modules => lazyModuleContainers.Value;

		public event OnModuleDataDelegate OnModuleData;
		public event OnModuleReaderInitDelegate OnModuleReaderInit;

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
		public DBIReader(Context ctx, Stream stream) : base(stream) {
			this.ctx = ctx;
			if (stream.Length == 0)
				return;

			if(stream.Length < Marshal.SizeOf<DBIHeader>()) {
				throw new InvalidDataException();
			}


			Header = ReadStruct<DBIHeader>();

			if(Header.Signature != unchecked((uint)-1) || !Enum.IsDefined(typeof(DBIVersion), (uint)Header.Version)) {
				throw new InvalidDataException();
			}

			uint nStreams = ctx.StreamTableReader.NumStreams;
			if (
				Header.GsSymbolsStreamNumber >= nStreams ||
				Header.PsSymbolsStreamNumber >= nStreams ||
				Header.SymbolRecordsStreamNumber >= nStreams
			) {
				throw new InvalidDataException();
			}

			lazyModuleContainers = new Lazy<IEnumerable<IModuleContainer>>(ReadModules);
			stream.Position += Header.ModuleListSize;

			if(Header.SectionContributionSize > 0) {
				SectionContribs = PerformAt(stream.Position, () => new SectionContribsReader(stream));
			}
			stream.Position += Header.SectionContributionSize;

			stream.Position += Header.SectionMapSize;
			stream.Position += Header.FileInfoSize;

			if(Header.TypeServerMapSize > 0) {
				TypeServerMap = PerformAt(stream.Position, () => new TypeServerMapReader(stream));
			}
			stream.Position += Header.TypeServerMapSize;

			if (Header.EcSubstreamSize > 0) {
				EC = PerformAt(stream.Position, () => new ECReader(stream));
			}
			stream.Position += Header.EcSubstreamSize;

			if (Header.DebugHeaderSize > 0) {
				DebugInfo = new DebugReader(ctx, stream);
			}
		}


		private IEnumerable<IModuleContainer> ReadModules() {
			Stream.Position = Marshal.SizeOf<DBIHeader>();
			ctx.ModuleListReader = new ModuleListReader(ctx, Stream, Header.ModuleListSize);

			IEnumerable<ModuleInfo> moduleInfoList = ctx.ModuleListReader.Modules;
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
