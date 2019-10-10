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
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	public class Deserializers
	{
		public static NameIndexTableReader ReadNameIndexTable(ReaderSpan r) {
			return new NameIndexTableReader(r);
		}

		public static NameTableReader ReadNameTable(ReaderSpan r) {
			return new NameTableReader(r);
		}


		public static byte[] ReadBuffer(ReaderSpan r) {
			int numBytes = r.ReadInt32();
			return r.ReadBytes(numBytes);
		}

		public static T[] ReadBuffer<T>(ReaderSpan r) {
			byte[] buffer = ReadBuffer(r);

			List<T> elements = new List<T>();

			uint pos = 0;
			while (pos < buffer.Length) {
				//$TODO: interface
				T elem = (T)Activator.CreateInstance(typeof(T), r);
				elements.Add(elem);
			}

			return elements.ToArray();
		}

		public static T[] ReadArray<T>(ReaderSpan r) where T : unmanaged {
			uint numElements = r.ReadUInt32();
			T[] arr = new T[numElements];

			int tSize = Marshal.SizeOf<T>();
			for (int i = 0; i < numElements; i++) {
				unsafe {
					fixed (byte* data = r.ReadBytes(tSize)) {
						T element = *(T*)data;
						arr.SetValue(element, i);
					}
				}
			}

			return arr;
		}

		public static Dictionary<Tkey, Tval> ReadMap<Tkey, Tval>(ReaderSpan r)
			where Tkey : unmanaged
			where Tval : unmanaged {
			// sum of bitsizes of each member
			uint cardinality = r.ReadUInt32();
			int numElements = r.ReadInt32();

			BitSet available = new BitSet(ReadArray<UInt32>(r));
			BitSet deleted = new BitSet(ReadArray<UInt32>(r));

			int keySize = Marshal.SizeOf<Tkey>();
			int valueSize = Marshal.SizeOf<Tval>();

			Dictionary<Tkey, Tval> map = new Dictionary<Tkey, Tval>(numElements);

			for (int i = 0; i < numElements; i++) {
				if (!available.Contains(i)) {
					continue;
				}
				unsafe {
					fixed (byte* pKey = r.ReadBytes(keySize))
					fixed (byte* pVal = r.ReadBytes(valueSize)) {
						Tkey key = *(Tkey*)pKey;
						Tval val = *(Tval*)pVal;
						map.Add(key, val);
					}
				}
			}

			return map;
		}
	}
}
