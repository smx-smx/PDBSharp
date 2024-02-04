#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp
{
	public enum UdtNameTableVersion : UInt32
	{
		LongHash = 1,
		LongHashV2 = 2
	}

	namespace UdtNameTable {
		public class Data {
			public uint Magic;
			public UdtNameTableVersion Version;
			public byte[] NameTable = new byte[0];
			public uint[] NameIndices = new uint[0];
			public uint NumberOfNameIndices;
		}
		public class Serializer {
			const UInt32 MAGIC = 0xEFFEEFFE;

			public Data Data = new Data();

			private SpanStream stream;
			public Serializer(SpanStream stream) {
				this.stream = stream;
			}

			public string ReadName(long nameIndex) {
				string str = stream.PerformAt(nameIndex, () => {
					return stream.ReadCString();
				});
				return str;
			}

			public Data Read() {
				var Magic = stream.ReadUInt32();
				if (Magic != MAGIC) {
					throw new InvalidDataException();
				}

				var Version = stream.ReadEnum<UdtNameTableVersion>();

				var data = Deserializers.ReadBuffer(stream);

				var NameIndices = Deserializers.ReadArray<uint>(stream);
				var NumberOfNameIndices = stream.ReadUInt32();

				Data = new Data {
					Magic = Magic,
					Version = Version,
					NameTable = data,
					NameIndices = NameIndices,
					NumberOfNameIndices = NumberOfNameIndices
				};
				return Data;

				//BuildTypeMap();
			}
		}

		public class Accessor : IPDBService {
			private readonly Dictionary<string?, uint> String_NameIndex = new Dictionary<string?, uint>();
			private readonly Dictionary<uint, string> NameIndex_String = new Dictionary<uint, string>();
			public readonly Dictionary<uint, uint> NameIndex_TypeIndex = new Dictionary<uint, uint>();

			private readonly TPI.Serializer Tpi;
			private readonly TypeResolver resolver;
			private readonly Serializer serializer;
			private Data data;

			public Accessor(IServiceContainer sc, Serializer serializer) {
				this.Tpi = sc.GetService<TPI.Serializer>();
				this.resolver = sc.GetService<TypeResolver>();
				this.serializer = serializer;
				this.data = serializer.Data;
			}

			//$TODO(work in progress): fix the NI -> TI mapping
			private void BuildTypeMap() {
				uint minTi = Tpi.Data.Header.MinTypeIndex;
				uint maxTi = minTi + Tpi.Data.Header.MaxTypeIndex - 1;

				for (uint ti = minTi; ti <= maxTi; ti++) {
					var leafC = resolver.GetTypeByIndex(ti);
					if (leafC == null || leafC.Ctx is not ILeafType leaf) {
						continue;
					}

					if (!leaf.IsDefnUdt) {
						continue;
					}

					string? typeName = leaf.UdtName;

					if (leaf.IsLocalDefnUdtWithUniqueName) {
						throw new NotImplementedException();
					}

					if (!GetIndex(typeName, out uint nameIndex)) {
						//$TODO: how do i handle this?
						continue;
					}

					NameIndex_TypeIndex.Add(nameIndex, ti);
				}
			}

			public string? GetString(uint nameIndex) {
				if (nameIndex == 0) {
					return null;
				}

				if (NameIndex_String.TryGetValue(nameIndex, out string cachedString)) {
					return cachedString;
				}

				string str = serializer.ReadName(nameIndex);

				NameIndex_String.Add(nameIndex, str);
				return str;
			}

			public ILeafResolver? GetType(string? str) {
				if (!GetIndex(str, out uint nameIndex))
					return null;

				if (!NameIndex_TypeIndex.TryGetValue(nameIndex, out uint typeIndex))
					return null;

				return resolver.GetTypeByIndex(typeIndex);
			}

			public bool GetIndex(string? str, out uint index) {
				if (String_NameIndex.TryGetValue(str, out uint cachedIndex)) {
					index = cachedIndex;
					return true;
				}

				uint? _index = data.NameIndices
					.Where(ni => {
						if (ni == 0)
							return false;

						string? _str = GetString(ni);
						if (_str != null && !String_NameIndex.ContainsKey(_str)) {
							String_NameIndex.Add(_str, ni);
						}
						return _str == str;
					})
					.Cast<uint?>()
					.FirstOrDefault();

				if (_index == null) {
					index = 0;
					return false;
				}

				index = _index.Value;
				return true;
			}
		}
	}
}
