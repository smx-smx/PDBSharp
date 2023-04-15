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
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Smx.PDBSharp
{
	public struct StartEnd
	{
		public UInt32 Start;
		public UInt32 End;
	}

	public class FileSegmentBase
	{
		public readonly UInt16 NumberOfFiles;
		public readonly UInt16 NumberOfSegments;
		public readonly UInt32[] FileOffsets;

		public FileSegmentBase(SpanStream r) {
			NumberOfFiles = r.ReadUInt16();
			NumberOfSegments = r.ReadUInt16();
			FileOffsets = Enumerable.Range(1, NumberOfFiles).Select(_ => r.ReadUInt32()).ToArray();
		}
	}

	public class SegmentPadBase
	{
		public readonly UInt16 NumberOfSegments;
		public readonly UInt16 Padding;
		public readonly UInt32[] BaseSourceLengthsOffsets;

		public SegmentPadBase(SpanStream r) {
			NumberOfSegments = r.ReadUInt16();
			Padding = r.ReadUInt16();

			BaseSourceLengthsOffsets = Enumerable.Range(1, NumberOfSegments).Select(_ => r.ReadUInt32()).ToArray();
		}
	}

	public class SegmentPairOffset
	{
		public UInt16 Segment;
		public UInt16 NumberOfPairs;
		public UInt32[] Offsets;
		public UInt16[] LineNumbers;

		public SegmentPairOffset(SpanStream r) {
			Segment = r.ReadUInt16();
			NumberOfPairs = r.ReadUInt16();

			Offsets = Enumerable.Range(1, NumberOfPairs).Select(_ => r.ReadUInt32()).ToArray();
			LineNumbers = Enumerable.Range(1, NumberOfPairs).Select(_ => r.ReadUInt16()).ToArray();
		}
	}

	public class C11File
	{
		public readonly SegmentPadBase SPB;
		public readonly StartEnd[] SE;
		public readonly string Name;
		public SegmentPairOffset[] SPO;

		public C11File(SpanStream r) {
			SPB = new SegmentPadBase(r);

			SE = Enumerable.Range(1, SPB.NumberOfSegments)
									.Select(_ => r.ReadStruct<StartEnd>())
									.ToArray();

			Name = r.ReadCString();

			SPO = Enumerable.Range(0, SPB.NumberOfSegments - 1).Select(i => {
				return r.PerformAt(SPB.BaseSourceLengthsOffsets[i], () => new SegmentPairOffset(r));
			}).ToArray();
		}
	}

	public class C11Lines
	{
		public readonly FileSegmentBase FSB;
		public readonly StartEnd[] SE;
		public readonly UInt16[] SegmentNumbers;

		public readonly C11File[] Files;

		public C11Lines(SpanStream r) {
			// read FSB
			FSB = new FileSegmentBase(r);

			// Segment Entries
			SE = Enumerable.Range(1, FSB.NumberOfSegments).Select(_ => r.ReadStruct<StartEnd>()).ToArray();
			SegmentNumbers = Enumerable.Range(1, FSB.NumberOfSegments).Select(_ => r.ReadUInt16()).ToArray();

			Files = Enumerable.Range(0, FSB.NumberOfFiles - 1).Select(i => {
				return r.PerformAt(FSB.FileOffsets[i], () => new C11File(r));
			}).ToArray();
		}
	}
}
