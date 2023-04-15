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
using System.Text;

namespace Smx.PDBSharp.DebugSections.Types
{
	public enum FileChecksumType : byte
	{
		NONE = 0,
		MD5,
		SHA1,
		SHA_256
	}

	public class FileChecksumBlock
	{
		public readonly uint FilenameOffset;
		public readonly byte ChecksumSize;
		public readonly FileChecksumType ChecksumType;

		public FileChecksumBlock(SpanStream r) {
			FilenameOffset = r.ReadUInt32();
			ChecksumSize = r.ReadByte();
			ChecksumType = r.ReadEnum<FileChecksumType>();
		}
	}
}
