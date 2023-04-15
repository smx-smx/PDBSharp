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
using System.Linq;

namespace Smx.PDBSharp
{
	public class LineSection : IDebugSection
	{
		private readonly UInt32 contentsOffset;
		private readonly UInt16 contentsSegment;
		private readonly UInt16 flags;
		private readonly UInt32 contentsSize;

		private const uint CV_LINES_HAVE_COLUMNS = 0x0001;

		public bool HaveColumns => (flags & CV_LINES_HAVE_COLUMNS) == CV_LINES_HAVE_COLUMNS;

		public IList<C13FileBlock> FileBlocks;

		public LineSection(SpanStream r) {
			contentsOffset = r.ReadUInt32();
			contentsSegment = r.ReadUInt16();
			flags = r.ReadUInt16();
			contentsSize = r.ReadUInt32();
			FileBlocks = ReadTable(r).ToList();
		}

		private IEnumerable<C13FileBlock> ReadTable(SpanStream r) {
			uint lastOffset = contentsOffset + contentsSize;
			long tableLength = r.Remaining;
			while(tableLength > 0) {
				C13FileBlock fileBlock = new C13FileBlock(this, r);
				tableLength -= fileBlock.fileBlockLength;			
				yield return fileBlock;
			}
		}
	}
}