#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Smx.PDBSharp.Symbols;

namespace Smx.PDBSharp
{

	public struct SectionContribution
	{
		public UInt32 DataCrc;
		public UInt32 RelocCrc;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SectionContribution20
	{
		public UInt16 SectionIndex;
		public UInt32 Offset;
		public UInt32 Size;
		public UInt16 ModuleIndex;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SectionContribution40
	{
		//<-- SC40
		public UInt16 SectionIndex;
		//- padding u16
		public UInt32 Offset;
		public UInt32 Size;
		public UInt32 Characteristics;
		public UInt16 ModuleIndex;

		public SectionContribution Footer;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModuleInfo
	{
		public UInt32 Opened;
		public SectionContribution40 Section;
		public UInt16 Flags;
		public Int16 StreamNumber;
		public UInt32 SymbolsSize;
		public UInt32 LinesSize;
		public UInt32 C13LinesSizes;
		public UInt16 NumFiles;
		//- padding u16
		public UInt32 FilenameOffsets;

		public UInt32 SourceFileNameIdx;
		public UInt32 PDBNameIdx;

		//szModule
		//szObjFile
	}

	public class ModuleInfoInstance 
	{
		public ModuleInfo Header;
		public string ModuleName;
		public string ObjectFileName;
	}

	/// <summary>
	/// Represents a module without symbols
	/// </summary>
	public class ModuleWrapper : IModule
	{
		public readonly ModuleInfoInstance Header;

		public ModuleWrapper(ModuleInfoInstance moduleInfo) {
			Header = moduleInfo;
		}

		public ModuleInfoInstance Module => Header;
		public IEnumerable<ISymbol> Symbols => Enumerable.Empty<ISymbol>();
	}

	public class ModuleListReader : ReaderBase
	{
		private readonly DBIReader dbi;
		public ModuleListReader(DBIReader dbi, Stream stream) : base(stream) {
			this.dbi = dbi;
		}

		private IEnumerable<ModuleInfoInstance> modules;
		public IEnumerable<ModuleInfoInstance> Modules {
			get {
				if (modules == null)
					modules = GetModules();
				return modules;
			}
		}

		private IEnumerable<ModuleInfoInstance> GetModules() {
			var remaining = Stream.Length;
			while (remaining > 0) {
				ModuleInfo mod = ReadStruct<ModuleInfo>();
				string moduleName = ReadCString();
				string objectFileName = ReadCString();

				Trace.WriteLine($"[{moduleName}:{objectFileName}]");

				yield return new ModuleInfoInstance() {
					Header = mod,
					ModuleName = moduleName,
					ObjectFileName = objectFileName
				};

				int moduleSize = Marshal.SizeOf<ModuleInfo>()
					+ moduleName.Length + 1
					+ objectFileName.Length + 1
					+ AlignTo(sizeof(int));

				remaining -= moduleSize;
			}
		}
	}
}
