#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp
{
	public enum SCVersion : UInt32
	{
		/// <summary>
		/// VC++ 4.0, has no version code so we use this as fallback with an arbitrary free number
		/// </summary>
		Old,
		V60 = 0xeffe0000 + 19970605,
		New = 0xeffe0000 + 20140516
	}


	public class SectionContribsReader : SpanStream
	{

		public readonly SCVersion Version;

        public IEnumerable<SectionContrib40> SectionContribs { get; }

        private readonly long StreamOffset;
		private readonly uint SectionContribsSize;
		private new uint ReadBytes = 0;

		public IEnumerable<SectionContrib40> GetByModule(ModuleInfo modi) {
			return GetByModule(modi.ModuleIndex);
		}

		public IEnumerable<SectionContrib40> GetByModule(int modIndex) {
			return SectionContribs.Where(sc => sc.ModuleIndex == modIndex);
		}

		private IEnumerable<SectionContrib40> ReadSectionContribsOld(IServiceContainer ctx) {
			while(ReadBytes < SectionContribsSize) {
				yield return PerformAt(StreamOffset + ReadBytes, () => new SectionContrib40(ctx, this));
				ReadBytes += SectionContrib40.SIZE;
			}
		}

		private IEnumerable<SectionContrib40> ReadSectionContribsV1(IServiceContainer ctx) {
			while (ReadBytes < SectionContribsSize) {
				yield return PerformAt(StreamOffset + ReadBytes, () => new SectionContrib(ctx, this));
				ReadBytes += SectionContrib.SIZE;
			}
		}

		private IEnumerable<SectionContrib40> ReadSectionContribsV2(IServiceContainer ctx) {
			while(ReadBytes < SectionContribsSize) {
				yield return PerformAt(StreamOffset + ReadBytes, () => new SectionContrib2(ctx, this));
				ReadBytes += SectionContrib2.SIZE;
			}
		}


		public SectionContribsReader(IServiceContainer ctx, uint sectionContribsSize, SpanStream stream) : base(stream) {
			this.SectionContribsSize = sectionContribsSize;

			UInt32 version = ReadUInt32();
			switch (version) {
				case (uint)SCVersion.V60:
					Version = SCVersion.V60;
					SectionContribs = new CachedEnumerable<SectionContrib40>(ReadSectionContribsV1(ctx));
					break;
				case (uint)SCVersion.New:
					Version = SCVersion.New;
					SectionContribs = new CachedEnumerable<SectionContrib40>(ReadSectionContribsV2(ctx));
					break;
				default:
					Version = SCVersion.Old;
					SectionContribs = new CachedEnumerable<SectionContrib40>(ReadSectionContribsOld(ctx));
					break;
			}

			// VC++ 4.0 has no version code
			StreamOffset = (Version == SCVersion.Old) ? 0 : Position;
		}
	}
}
