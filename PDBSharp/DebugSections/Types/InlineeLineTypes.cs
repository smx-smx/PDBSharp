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
using System.Text;

namespace Smx.PDBSharp.DebugSections.Types
{
	public enum InlineeSectionType : uint
	{
		NormalSignature = 0,
		ExtendedSignature = 1
	}

	public class InlineeSourceLineBlock
	{
		public readonly uint InlineeId;
		public readonly uint FileId;
		public readonly uint SourceLineNumber;

		public const int SIZEOF = 6;

		public InlineeSourceLineBlock(SpanStream r) {
			InlineeId = r.ReadUInt32();
			FileId = r.ReadUInt32();
			SourceLineNumber = r.ReadUInt32();
		}
	}

	public class InlineeSourceLineExtendedBlock : InlineeSourceLineBlock
	{
		// + count of extra files field
		public const int SIZEOF = InlineeSourceLineBlock.SIZEOF + 4;

		public readonly uint[] ExtraFiles;

		public InlineeSourceLineExtendedBlock(SpanStream r)
			: base(r) {
			var numberOfFiles = r.ReadUInt32();
			ExtraFiles = Enumerable.Range(0, (int)numberOfFiles)
				.Select(_ => r.ReadUInt32())
				.ToArray();
		}
	}
}
