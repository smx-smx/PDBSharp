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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp
{
	namespace StreamTable {
		public class Data {
			public uint NumStreams;
			public uint[] StreamSizes = new uint[0];
		}

		public class Serializer : IPDBService {
			private MSFReader msf;
			private PDBSpanStream stream;

			public Data Data = new Data();

			private long[] offsets = new long[0];
			private byte[][] streamsData = new byte[0][];

			public Serializer(IServiceContainer sc, PDBSpanStream stream) {
				msf = sc.GetService<MSFReader>();
				this.stream = stream;
			}

			private UInt32[] ReadStreamSizesJG() {
				UInt32[] streamSizes = Enumerable
					.Range(1, (int)Data.NumStreams)
					.Select(_ => {
						uint streamSize = (uint)stream.ReadCB();
						// skip pageof(Map<SPN, PN>)
						stream.ReadUInt32();
						return streamSize;
					}).ToArray();

				return streamSizes;
			}

			private UInt32[] ReadStreamSizesDS() {
				UInt32[] streamSizes = Enumerable
					.Range(1, (int)Data.NumStreams)
					.Select(_ => (uint)stream.ReadCB())
					.ToArray();

				return streamSizes;
			}

			private UInt32[] ReadStreamSizes() {
				switch (msf.FileType) {
					case PDBType.Big:
						return ReadStreamSizesDS();
					case PDBType.Small:
						return ReadStreamSizesJG();
					default:
						throw new ArgumentException();
				}
			}

			public Data Read() {
				Data.NumStreams = stream.ReadUInt32();
				Data.StreamSizes = ReadStreamSizes();

				offsets = Enumerable.Repeat(-1L, (int)Data.NumStreams).ToArray();
				streamsData = new byte[Data.NumStreams][];

				return Data;
			}

			/// <summary>
			/// Reads the list of pages for a given stream
			/// </summary>
			/// <param name="streamNumber"></param>
			/// <returns></returns>
			private IEnumerable<UInt32> GetListPages_Stream(int streamNumber) {
				var streamSize = Data.StreamSizes[streamNumber];

				long dataOffset = offsets[streamNumber];
				if (dataOffset < 0) {
					// skip number of streams and stream sizes
					dataOffset = sizeof(UInt32) + (stream.SI_PERSIST_SIZE * Data.NumStreams);

					// skip the page list for the streams before us
					for (int i = 0; i < streamNumber; i++) {
						dataOffset += msf.GetNumPages(Data.StreamSizes[i]) * stream.PN_SIZE;
					}

					offsets[streamNumber] = dataOffset;
				}

				stream.Seek(dataOffset, SeekOrigin.Begin);

				// how many pages are we going to read
				var numStreamPages = msf.GetNumPages(streamSize);
				UInt32[] pageList = Enumerable
					.Range(1, (int)numStreamPages)
					.Select(_ => stream.ReadPN())
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
}
