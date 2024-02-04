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
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;

namespace Smx.PDBSharp
{
	public enum DebugType : Int16
	{
		FPO,
		Exception,
		Fixup,
		OMapToSrc,
		OMapFromSrc,
		SectionHdr,
		TokenRidMap,
		XData,
		PData,
		NewFPO,
		SectionHdrOrig,

		DebugTypeMax
	}

	namespace DebugData
	{
		public class Data {
			public short[] DebugStreams = new short[(int)DebugType.DebugTypeMax];
		}

		public class Accessor {
			private readonly StreamTable.Serializer streamTable;
			private readonly Data data;
			
			private ILazy<FPO.Stream.Data?> FPOLazy;
			public FPO.Stream.Data? FPO => FPOLazy.Value;

			public Accessor(IServiceContainer sc, Data data) {
				streamTable = sc.GetService<StreamTable.Serializer>();
				this.data = data;
				FPOLazy = LazyFactory.CreateLazy(GetFPOStream);
			}

			public bool HasStream(DebugType type) {
				return data.DebugStreams[(int)type] != -1;
			}

			public byte[]? GetStream(DebugType type) {
				if (!HasStream(type))
					return null;

				int streamNumber = data.DebugStreams[(int)type];
				return streamTable.GetStream(streamNumber);
			}

			private FPO.Stream.Data? GetFPOStream() {
				byte[]? fpo = GetStream(DebugType.FPO);
				if (fpo == null)
					return null;

				return new FPO.Stream.Serializer(new SpanStream(fpo)).Read();
			}
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var DebugStreams = new short[(int)DebugType.DebugTypeMax];

				for (int i = 0; i < (int)DebugType.DebugTypeMax; i++) {
					DebugStreams[i] = stream.ReadInt16();
				}

				Data = new Data {
					DebugStreams = DebugStreams
				};
				return Data;
			}
		}
	}
}
