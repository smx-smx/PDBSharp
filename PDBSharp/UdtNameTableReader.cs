#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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

namespace Smx.PDBSharp
{
	public enum UdtNameTableVersion : UInt32
	{
		LongHash = 1,
		LongHashV2 = 2
	}

	public class UdtNameTableReader : SpanStream
	{
		const UInt32 MAGIC = 0xEFFEEFFE;

		private readonly UInt32 Magic;
		private readonly UdtNameTableVersion Version;

		private readonly byte[] data;
		public readonly uint[] NameIndices;
		private readonly uint NumberOfNameIndices;

		private readonly SpanStream rdr;

		private readonly Dictionary<string, uint> String_NameIndex = new Dictionary<string, uint>();
		private readonly Dictionary<uint, string> NameIndex_String = new Dictionary<uint, string>();

		private readonly TPIReader Tpi;

		private readonly TypeResolver resolver;

		public readonly Dictionary<uint, uint> NameIndex_TypeIndex = new Dictionary<uint, uint>();

		//$TODO(work in progress): fix the NI -> TI mapping
		private void BuildTypeMap() {
			uint minTi = Tpi.Header.MinTypeIndex;
			uint maxTi = minTi + Tpi.Header.MaxTypeIndex - 1;

			for (uint ti = minTi; ti <= maxTi; ti++) {
				ILeafContainer leafC = resolver.GetTypeByIndex(ti);
				if (leafC == null || !(leafC.Data is LeafBase leaf)) {
					continue;
				}

				if (!leaf.IsDefnUdt) {
					continue;
				}

				string typeName = leaf.UdtName;

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

		public string GetString(uint nameIndex) {
			if (nameIndex == 0) {
				return null;
			}

			if (NameIndex_String.TryGetValue(nameIndex, out string cachedString)) {
				return cachedString;
			}

			string str = rdr.PerformAt(nameIndex, () => {
				return rdr.ReadCString();
			});

			NameIndex_String.Add(nameIndex, str);
			return str;
		}

		public ILeafContainer GetType(string str) {
			if (!GetIndex(str, out uint nameIndex))
				return null;

			if (!NameIndex_TypeIndex.TryGetValue(nameIndex, out uint typeIndex))
				return null;

			return resolver.GetTypeByIndex(typeIndex);
		}

		public bool GetIndex(string str, out uint index) {
			if (String_NameIndex.TryGetValue(str, out uint cachedIndex)) {
				index = cachedIndex;
				return true;
			}

			uint? _index = NameIndices
				.Where(ni => {
					if (ni == 0)
						return false;

					string _str = GetString(ni);
					if (!String_NameIndex.ContainsKey(_str)) {
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

		public UdtNameTableReader(IServiceContainer ctx, byte[] namesData) : base(namesData) {
			this.Tpi = ctx.GetService<TPIReader>();
			this.resolver = ctx.GetService<TypeResolver>();

			Magic = ReadUInt32();
			if (Magic != MAGIC) {
				throw new InvalidDataException();
			}

			Version = ReadEnum<UdtNameTableVersion>();

			data = Deserializers.ReadBuffer(this);
			rdr = new SpanStream(data);

			NameIndices = Deserializers.ReadArray<uint>(this);
			NumberOfNameIndices = ReadUInt32();

			//BuildTypeMap();
		}
	}
}
