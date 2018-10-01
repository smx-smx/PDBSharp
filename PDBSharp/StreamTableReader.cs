#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class StreamTableReader : ReaderBase
	{
		private readonly MSFReader msf;

		public StreamTableReader(MSFReader msf, Stream stream) : base(stream) {
			this.msf = msf;
			numStreams = Reader.ReadUInt32();
			streamsData = new byte[numStreams][];

			StreamSizes();
		}

		private uint numStreams;
		private UInt32[] streamSizes;

		private byte[][] streamsData;

		public UInt32[] StreamSizes() {
			if(streamSizes != null)
				return streamSizes;

			Stream.Position = sizeof(int);

			streamSizes = new uint[numStreams];
			for(int i=0; i<numStreams; i++) {
				streamSizes[i] = Reader.ReadUInt32();
			}

			return streamSizes;
		}

		private IEnumerable<UInt32> GetListPages_Stream(uint streamNumber) {
			var streamSize = streamSizes[streamNumber];
			var numStreamPages = msf.GetNumPages(streamSize);

			uint dataOffset = sizeof(int) * (numStreams + 1);
			for(int i=0; i<streamNumber; i++) {
				dataOffset += msf.GetNumPages(streamSizes[i]) * sizeof(int);
			}

			Stream.Position = dataOffset;

			UInt32[] pageList = new UInt32[numStreamPages];
			for(int i=0; i<numStreamPages; i++) {
				pageList[i] = Reader.ReadUInt32();
			}

			return pageList;
		}

		private byte[] ReadStream(uint streamNumber) {
			var pages = GetListPages_Stream(streamNumber);

			return pages
				.Select(page => msf.ReadPage(page))
				.SelectMany(x => x)
				.ToArray();
		}

		public byte[] GetStream(uint streamNumber) {
			if(streamsData[streamNumber] != null)
				return streamsData[streamNumber];

			streamsData[streamNumber] = ReadStream(streamNumber);
			return streamsData[streamNumber];
		}
		
	}
}
