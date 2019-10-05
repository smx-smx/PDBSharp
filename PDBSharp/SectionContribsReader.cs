#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.IO;

namespace Smx.PDBSharp
{
	public enum SCVersion : UInt32
	{
		V60 = 0xeffe0000 + 19970605,
		New = 0xeffe0000 + 20140516
	}


	public class SectionContribsReader : ReaderBase
	{
		private readonly SCVersion version;

		public SectionContribsReader(Stream stream) : base(stream) {
			version = ReadEnum<SCVersion>();
		}
	}
}
