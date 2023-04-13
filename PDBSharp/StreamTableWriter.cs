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
using System.IO;
using System.Linq;

namespace Smx.PDBSharp
{
	public class StreamTableWriter
	{
		private readonly MSFWriter msf;

		private readonly List<Memory<byte>> Streams = new List<Memory<byte>>();

		private SpanStream st;

		public StreamTableWriter(MSFWriter msf) {
			this.msf = msf;
		}

		public long GetDataSize() {
			long dataSize = 0;
			dataSize += sizeof(uint); //stream count
			dataSize += sizeof(uint) * Streams.Count; //stream sizes
			dataSize += Streams.Sum(st => st.Length); //stream data size
			return dataSize;
		}

		public void AddStream(byte[] streamData) {
			Streams.Add(streamData);
		}

		private void WriteStreamSizes() {
			Streams.ForEach(data => st.WriteUInt32((uint)data.Length));
		}

		private void WriteStream(Memory<byte> data) {
			var numPages = msf.GetNumPages((uint)data.Length);

			uint streamSize = numPages * msf.PageSize;

			uint directorySize = sizeof(UInt32) * numPages;

			for (int i = 0; i < numPages; i++) {
				st.WriteUInt32(msf.AllocPageNumber());
			}

			// write stream first
			st.WriteMemory(data);

			// pad to page size
			uint paddingSize = streamSize - (uint)data.Length;
			byte[] padding = new byte[paddingSize];
			st.WriteBytes(padding);
		}

		public uint GetCurrentSize() {
			return (uint)(
				sizeof(UInt32) +
				sizeof(UInt32) * Streams.Count +
				Streams.Sum(data => data.Length)
			);
		}

		public void Commit() {
			st = new SpanStream(new byte[(int)GetDataSize()]);

			st.WriteUInt32((uint)Streams.Count);
			WriteStreamSizes();
			Streams.ForEach(data => WriteStream(data));
		}
	}
}