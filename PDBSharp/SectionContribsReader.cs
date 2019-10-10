#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public enum SCVersion : UInt32
	{
		V60 = 0xeffe0000 + 19970605,
		New = 0xeffe0000 + 20140516
	}


	public class SectionContribsReader : ReaderSpan
	{

		public readonly SCVersion Version;

		private readonly ILazy<IEnumerable<SectionContrib40>> sectionContribsLazy;
		public IEnumerable<SectionContrib40> SectionContribs => sectionContribsLazy.Value;

		private readonly long StreamOffset;
		private readonly uint SectionContribsSize;
		private uint ReadBytes = 0;

		private IEnumerable<SectionContrib40> ReadSectionContribsOld() {
			while(ReadBytes < SectionContribsSize) {
				yield return PerformAt(StreamOffset + ReadBytes, () => new SectionContrib40(this));
				ReadBytes += SectionContrib40.SIZE;
			}
		}

		private IEnumerable<SectionContrib40> ReadSectionContribsV1() {
			while (ReadBytes < SectionContribsSize) {
				yield return PerformAt(StreamOffset + ReadBytes, () => new SectionContrib(this));
				ReadBytes += SectionContrib.SIZE;
			}
		}

		private IEnumerable<SectionContrib40> ReadSectionContribsV2() {
			while(ReadBytes < SectionContribsSize) {
				yield return PerformAt(StreamOffset + ReadBytes, () => new SectionContrib2(this));
				ReadBytes += SectionContrib2.SIZE;
			}
		}


		public SectionContribsReader(uint sectionContribsSize, ReaderSpan stream) : base(stream) {
			this.SectionContribsSize = sectionContribsSize;

			Version = ReadEnum<SCVersion>();
			switch (Version) {
				case SCVersion.V60:
					sectionContribsLazy = LazyFactory.CreateLazy(ReadSectionContribsV1);
					break;
				case SCVersion.New:
					sectionContribsLazy = LazyFactory.CreateLazy(ReadSectionContribsV2);
					break;
				default:
					sectionContribsLazy = LazyFactory.CreateLazy(ReadSectionContribsOld);
					break;
			}

			// after version
			StreamOffset = Position;
		}
	}
}
