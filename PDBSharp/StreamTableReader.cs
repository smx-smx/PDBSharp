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
using System.Linq;

namespace Smx.PDBSharp
{
	public class StreamTableReader : SpanReader
	{
		public readonly UInt32[] StreamSizes;
		private readonly MSFReader msf;

		public StreamTableReader(IServiceContainer ctx, byte[] data) : base(data) {

			msf = ctx.GetService<MSFReader>();

			NumStreams = ReadUInt32();
			StreamSizes = ReadStreamSizes();

			streamsData = new byte[NumStreams][];
			offsets = Enumerable.Repeat(-1L, (int)NumStreams).ToArray();
		}

		public readonly uint NumStreams;
		private readonly byte[][] streamsData;
		private readonly long[] offsets;

		private UInt32[] ReadStreamSizes() {
			UInt32[] streamSizes = Enumerable
				.Range(1, (int)NumStreams)
				.Select(_ => ReadUInt32())
				.ToArray();

			return streamSizes;
		}

		/// <summary>
		/// Reads the list of pages for a given stream
		/// </summary>
		/// <param name="streamNumber"></param>
		/// <returns></returns>
		private IEnumerable<UInt32> GetListPages_Stream(int streamNumber) {
			var streamSize = StreamSizes[streamNumber];

			long dataOffset = offsets[streamNumber];
			if(dataOffset < 0) {
				// skip stream sizes
				dataOffset = sizeof(UInt32) * (NumStreams + 1);

				// skip the page list for the streams before us
				for (int i = 0; i < streamNumber; i++) {
					dataOffset += msf.GetNumPages(StreamSizes[i]) * sizeof(UInt32);
				}

				offsets[streamNumber] = dataOffset;
			}

			Seek(dataOffset, SeekOrigin.Begin);

			// how many pages are we going to read
			var numStreamPages = msf.GetNumPages(streamSize);
			UInt32[] pageList = Enumerable
				.Range(1, (int)numStreamPages)
				.Select(_ => ReadUInt32())
				.ToArray();

			return pageList;
		}

		private byte[] ReadStream(int streamNumber) {
			var pages = GetListPages_Stream(streamNumber);

			// for each page in list, read the page data and combine
			return pages
				.Select(page => msf.ReadPage(page))
				.SelectMany(x => x)
				.ToArray();
		}

		public byte[] GetStream(DefaultStreams streamNumber) => GetStream((int)streamNumber);

		public byte[] GetStream(int streamNumber) {
			if (streamsData[streamNumber] != null)
				return streamsData[streamNumber];

			streamsData[streamNumber] = ReadStream(streamNumber);
			return streamsData[streamNumber];
		}
	}
}
