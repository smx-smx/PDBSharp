#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.DebugSections;
using Smx.PDBSharp.DebugSections.Types;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Smx.PDBSharp
{

	public struct C13SubSectionHeader
	{
		public C13DebugSubSectionType Type;
		public UInt32 Length;
	}

	public enum C13DebugSubSectionType : uint
	{
		IGNORE = 0x80000000,    // if this bit is set in a subsection type then ignore the subsection contents

		SYMBOLS = 0xf1,
		LINES,
		STRINGTABLE,
		FILECHKSMS,
		FRAMEDATA,
		INLINEELINES,
		CROSSSCOPEIMPORTS,
		CROSSSCOPEEXPORTS,

		IL_LINES,
		FUNC_MDTOKEN_MAP,
		TYPE_MDTOKEN_MAP,
		MERGED_ASSEMBLYINPUT,

		COFF_SYMBOL_RVA,
	}

	public class C13SubSectionReader
	{
		public readonly C13SubSectionHeader Header;
		private readonly SpanStream sectionStream;

		public C13SubSectionReader(C13SubSectionHeader header, SpanStream r) {
			Header = header;
			sectionStream = r;
		}

		public IDebugSection ReadDebugSections() {
			switch (Header.Type) {
				case C13DebugSubSectionType.LINES:
					return new LineSection(sectionStream);
				case C13DebugSubSectionType.FILECHKSMS:
					return new FileChecksumsSection(sectionStream);
				case C13DebugSubSectionType.INLINEELINES:
					return new InlineeLineSection(sectionStream);
				default:
					throw new NotImplementedException(Enum.GetName(typeof(C13DebugSubSectionType), Header.Type));
			}
		}
	}

	public class C13Lines
	{
		public readonly IDebugSection[] DebugSections;

		private IDebugSection ReadSubSection(SpanStream r) {
			C13SubSectionHeader hdr = r.ReadStruct<C13SubSectionHeader>();

			SpanStream subStream = r.SliceHere((int)hdr.Length);
			C13SubSectionReader rdr = new C13SubSectionReader(hdr, subStream);
			var subSection = rdr.ReadDebugSections();

			r.Position += hdr.Length;
			return subSection;
		}

		public C13Lines(SpanStream r) {
			DebugSections = r.ReadAll(ReadSubSection).ToArray();
		}
	}
}