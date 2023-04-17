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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Smx.PDBSharp.DebugSections.Types
{
	public class C13FileBlock
	{
		public uint fileId;
		public uint numLines;
		/// <summary>
		/// length of this block (including the 12 bytes header)
		/// </summary>
		public uint fileBlockLength;

		public C13Line[] Lines;
		public C13Column[]? Columns;

		public C13FileBlock(LineSection header, SpanStream r) {
			fileId = r.ReadUInt32();
			numLines = r.ReadUInt32();
			fileBlockLength = r.ReadUInt32();
			
			Lines = Enumerable.Range(0, (int)numLines)
				.Select(_ => new C13Line(r))
				.ToArray();

			if (header.HaveColumns) {
				Columns = Enumerable.Range(0, (int)numLines)
					.Select(_ => new C13Column(r))
					.ToArray();
			}
		}
	}

	public class C13Column
	{
		public ushort columnStartOffset;
		public ushort columnEndOffset;

		public C13Column(SpanStream r) {
			columnStartOffset = r.ReadUInt16();
			columnEndOffset = r.ReadUInt16();
		}
	}

	public class C13Line
	{
		/// <summary>
		/// Offset to start of code bytes for line number
		/// </summary>
		public uint offset;

		private uint data;

		/// <summary>
		/// line where statement/expression starts
		/// </summary>
		public uint LineNumStart => data & 0xFFFFFF;
		/// <summary>
		/// delta to line where statement ends (optional)
		/// </summary>
		public uint DeltaLineEnd => data >> 24 & 0x7F;

		/// <summary>
		/// true if a statement linenumber, else an expression line num
		/// </summary>
		public bool IsStatement => (data >> 31 & 1) == 1;

		public C13Line(SpanStream r) {
			offset = r.ReadUInt32();
			data = r.ReadUInt32();
		}
	}
}
