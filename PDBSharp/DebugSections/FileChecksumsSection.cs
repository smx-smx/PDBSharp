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
	public class FileChecksumsSection : IDebugSections
	{
		public FileChecksumsSection(SpanStream r) {
			ReadTable(r);
		}

		private void ReadTable(SpanStream r) {
			while (r.Remaining > 0) {
				// read packed entry (6 bytes)
				var block = new FileChecksumBlock(r);
				// read checksum data
				var checksumData = r.ReadBytes(block.ChecksumSize);
				// pad to 4 bytes
				r.AlignStream(4);
			}
		}
	}
}