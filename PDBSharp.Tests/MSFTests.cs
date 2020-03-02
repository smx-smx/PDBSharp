#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using NUnit.Framework;
using Smx.PDBSharp;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tests
{
	public class MSFTests {
		[SetUp]
		public void Setup() {
		}

#if false
		[Ignore("Work in Progress")]
		[Test]
		public void TestSingleStream() {
			string filePath = Path.GetTempFileName();
			using (Stream strm = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				MSFWriter wr = new MSFWriter(strm);
				wr.PageSize = 4096;

				byte[] sourceData = File.ReadAllBytes(Assembly
					.GetExecutingAssembly()
					.Location);

				wr.StreamTable.AddStream(sourceData);
				wr.Commit();

				strm.Position = 0;
				PDBFile rdr = new PDBFile(strm);
				Assert.AreEqual(rdr.Streams.Count(), 1);

				byte[] streamData = rdr.Streams.First();
				Assert.AreEqual(sourceData, streamData);
			}

			File.Delete(filePath);
		}
#endif

		[Test]
		public void TestHeader() {
			MSFWriter wr = new MSFWriter();
			wr.PageSize = 4096;
			wr.Commit();

			MSFReader rdr = new MSFReaderDS(wr.Memory);
			string magic = rdr.Header.Magic;
			Assert.AreEqual(magic, PDBFile.BIG_MAGIC);
			Assert.AreEqual(rdr.Header.PageSize, 4096);
		}
	}
}