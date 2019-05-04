#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
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
		public UInt32 TypeServerIndex;

		public UInt32 DebugHeaderSize;
		public UInt32 EcSubstreamSize;

		public DBIFlags Flags;
		public UInt16 MachineType;
		public UInt32 Reserved;
	}

	public class DBIReader : ReaderBase
	{
		private DBIHeader hdr;
		private readonly StreamTableReader stRdr;

		private readonly PDBFile pdb;

		public DBIReader(PDBFile pdb, StreamTableReader stRdr, Stream stream) : base(stream) {
			this.pdb = pdb;

			this.stRdr = stRdr;
			hdr = ReadStruct<DBIHeader>();

			if(hdr.Signature != unchecked((uint)-1) || !Enum.IsDefined(typeof(DBIVersion), (uint)hdr.Version)) {
				throw new InvalidDataException();
			}

			uint nStreams = stRdr.NumStreams;
			if (
				hdr.GsSymbolsStreamNumber >= nStreams ||
				hdr.PsSymbolsStreamNumber >= nStreams ||
				hdr.SymbolRecordsStreamNumber >= nStreams
			) {
				throw new InvalidDataException();
			}
		}

		public IEnumerable<IModule> ReadModules() {
			byte[] moduleList = ReadBytes((int)hdr.ModuleListSize);
			var modListRdr = new ModuleListReader(new MemoryStream(moduleList));

			IEnumerable<ModuleInfo> moduleInfoList = modListRdr.Modules;
			foreach (ModuleInfo mod in moduleInfoList) {
				int streamNumber = mod.StreamNumber;
				if (streamNumber < 0) {
					continue;
				}
				byte[] modData = stRdr.GetStream(streamNumber);

				MemoryStream modStream = new MemoryStream(modData);
				UInt32 signature = new BinaryReader(modStream).ReadUInt32();
				modStream.Position = 0;
				if (Enum.IsDefined(typeof(CodeViewSignature), signature)) {
					yield return new CodeViewModuleReader(pdb, mod, modStream);
				} else {
					yield return new SourceFileModuleReader(pdb, mod, modStream);
				}
			}
		}

	}
}
