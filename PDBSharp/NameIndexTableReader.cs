#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Thunks;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Smx.PDBSharp
{
	/// <summary>
	/// A name table is a two-way mapping from string to name index and back.
	/// Name indices(NIs) are intended to be small positive integers.
	/// </summary>
	namespace NameIndexTable {
		public class Data {
			public byte[] StringTableData = new byte[0];
			public uint MaxIndices;
			public Dictionary<uint, uint> OffsetToIndex = new Dictionary<uint, uint>();
		}

		public class Lookup {
			private readonly SpanStream rdr;
			private readonly Dictionary<uint, uint> Offset_Index;
			private readonly Dictionary<uint, uint> Index_Offset;
			
			private readonly Dictionary<string, uint> String_Index = new Dictionary<string, uint>();
			private readonly Dictionary<uint, string> Index_String = new Dictionary<uint, string>();

			public Lookup(Data data) {
				rdr = new SpanStream(data.StringTableData);
				Offset_Index = data.OffsetToIndex;
				Index_Offset = Offset_Index.ToDictionary(x => x.Value, x => x.Key);
			}

			public string? GetString(uint index) {
				if (!Index_Offset.ContainsKey(index)) {
					return null;
				}

				if (Index_String.TryGetValue(index, out string cachedString)) {
					return cachedString;
				}

				uint offset = Index_Offset[index];
				rdr.Position = offset;
				string str = rdr.ReadCString();

				Index_String.Add(index, str);
				return str;
			}

			public bool GetIndex(string str, out uint index) {
				if (String_Index.TryGetValue(str, out uint cachedIndex)) {
					index = cachedIndex;
					return true;
				}

				uint? _index = Offset_Index
					.Where(p => GetString(p.Value) == str)
					.Select(p => p.Value)
					.Cast<uint?>()
					.FirstOrDefault();

				if (_index == null) {
					index = 0;
					return false;
				}

				index = _index.Value;
				String_Index.Add(str, index);

				return true;
			}
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();

			public Data Read() {
				var stringTableData = Deserializers.ReadBuffer(stream);
				

				var Offset_Index = Deserializers.ReadMap<uint, uint>(stream);
				var MaxIndices = stream.ReadUInt32();

				// reverse of Offset_Index
				var Index_Offset = Offset_Index.ToDictionary(x => x.Value, x => x.Key);

				Data = new Data {
					StringTableData = stringTableData,
					MaxIndices = MaxIndices,
					OffsetToIndex = Offset_Index,
				};
				return Data;
			}
		}
	}
}
