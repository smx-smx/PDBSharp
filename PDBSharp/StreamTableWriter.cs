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
using System.IO;
using System.Linq;

namespace Smx.PDBSharp
{
	public class StreamTableWriter : WriterBase
	{
		private readonly MSFWriter msf;

		public uint NumStreams => (uint)Stream.Length;

		private readonly List<byte[]> Streams = new List<byte[]>();

		public StreamTableWriter(MSFWriter msf, Stream stream) : base(stream) {
			this.msf = msf;
		}

		public void AddStream(byte[] streamData) {
			Streams.Add(streamData);
		}

		private void WriteStreamSizes() {
			Streams.ForEach(data => WriteUInt32((uint)data.Length));
		}

		private void WriteStream(byte[] data) {
			var numPages = msf.GetNumPages((uint)data.Length);

			uint streamSize = numPages * msf.PageSize;

			uint directorySize = sizeof(UInt32) * numPages;
			long directoryOffset = Stream.Position;

			// write stream first
			Stream.Position += directorySize;
			WriteBytes(data);

			// pad to page size
			uint paddingSize = streamSize - (uint)data.Length;
			byte[] padding = new byte[paddingSize];
			WriteBytes(padding);

			// go back and write page numbers
			Stream.Position -= streamSize;

			for (int i = 0; i < numPages; i++) {
				WriteUInt32(msf.AllocPageNumber());
			}

			Stream.Position += streamSize;
		}

		public uint GetCurrentSize() {
			return (uint)(
				sizeof(UInt32) +
				sizeof(UInt32) * Streams.Count +
				Streams.Sum(data => data.Length)
			);
		}

		public void Commit() {
			WriteUInt32((uint)Streams.Count);
			WriteStreamSizes();
			Streams.ForEach(data => WriteStream(data));
		}
	}
}