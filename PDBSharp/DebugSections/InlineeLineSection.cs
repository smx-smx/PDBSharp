#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.DebugSections.Types;
using Smx.SharpIO;

namespace Smx.PDBSharp.DebugSections
{
	public class InlineeLineSection : IDebugSection
	{
		public readonly InlineeSectionType Signature;

		public InlineeLineSection(SpanStream r) {
			Signature = r.ReadEnum<InlineeSectionType>();

			var minSize = GetMinRecordSize();
			while (r.Remaining > minSize) {
				ReadRecord(r);
			}
		}

		private int GetMinRecordSize() {
			if (Signature == InlineeSectionType.ExtendedSignature) {
				return InlineeSourceLineExtendedBlock.SIZEOF;
			} else {
				return InlineeSourceLineBlock.SIZEOF;
			}
		}

		private InlineeSourceLineBlock ReadRecord(SpanStream r) {
			switch (Signature) {
				case InlineeSectionType.ExtendedSignature:
					return new InlineeSourceLineExtendedBlock(r);
				default:
					return new InlineeSourceLineBlock(r);
			}
		}
	}
}