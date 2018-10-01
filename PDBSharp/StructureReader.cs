#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace Smx.PDBSharp
{
	public class StructureMemberAttribute : Attribute
	{
		/// <summary>
		/// </summary>
		/// <param name="memberType">Type of a class to invoke to process the member</param>
		public StructureMemberAttribute(Type memberType) {
			this.MemberType = memberType;
		}

		public Type MemberType { get; private set; }

	}

	public static class StructureReader
	{
		public static byte[] StructToBytes<T>(T data) {
			byte[] rawData = new byte[Marshal.SizeOf(data)];
			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				Marshal.StructureToPtr(data, rawDataPtr, false);
			} finally {
				handle.Free();
			}

			return rawData;
		}

		public static T BytesToStruct<T>(Func<int, byte[]> readBytes) {
			byte[] bytes = readBytes(Marshal.SizeOf(typeof(T)));
			return BytesToStruct<T>(bytes);
		}

		private static T BytesToStruct<T>(byte[] rawData) {
			T result = default(T);

			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
			} finally {
				handle.Free();
			}

			return result;
		}
	}

	/// <summary>
	/// Reads in a structure field by field from an image reader.
	/// </summary>
	public class StructureReader<T> where T : struct
	{
		private Func<int, byte[]> readBytes;
		private Endianness defaultEndianess = Endianness.LittleEndian;

		public StructureReader(BinaryReader reader) : this(reader.ReadBytes) {
		}

		public StructureReader(Func<int, byte[]> readBytes) {
			this.readBytes = readBytes;
			if(typeof(T).IsDefined(typeof(EndianAttribute), false)) {
				EndianAttribute attr = (EndianAttribute)(typeof(T).GetCustomAttribute(typeof(EndianAttribute), false));
				this.defaultEndianess = attr.Endianness;
			}
		}

		public T Read() {
			return this.BytesToStruct(this.readBytes);
		}

		private int FieldSize(FieldInfo field) {
			if(field.FieldType.IsArray) {
				MarshalAsAttribute attr = (MarshalAsAttribute)field.GetCustomAttribute(typeof(MarshalAsAttribute), false);
				return Marshal.SizeOf(field.FieldType.GetElementType()) * attr.SizeConst;
			} else {
				if(field.FieldType.IsEnum) {
					return Marshal.SizeOf(Enum.GetUnderlyingType(field.FieldType));
				}
				return Marshal.SizeOf(field.FieldType);
			}
		}

		private void SwapEndian(byte[] data, Type type, FieldInfo field) {
			int offset = Marshal.OffsetOf(type, field.Name).ToInt32();
			if(field.FieldType.IsArray) {
				MarshalAsAttribute attr = (MarshalAsAttribute)field.GetCustomAttribute(typeof(MarshalAsAttribute), false);
				int subSize = Marshal.SizeOf(field.FieldType.GetElementType());
				for(int i = 0; i < attr.SizeConst; i++) {
					Array.Reverse(data, offset + (i * subSize), subSize);
				}
			} else {
				Array.Reverse(data, offset, FieldSize(field));
			}
		}

		/* Adapted from http://stackoverflow.com/a/2624377 */
		private void RespectEndianness(Type type, byte[] data) {
			foreach(var field in type.GetFields()) {
				if(field.IsDefined(typeof(EndianAttribute), false)) {
					Endianness fieldEndianess = ((EndianAttribute)field.GetCustomAttributes(typeof(EndianAttribute), false)[0]).Endianness;
					if(
						(fieldEndianess == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
						(fieldEndianess == Endianness.LittleEndian && !BitConverter.IsLittleEndian)
					) {
						SwapEndian(data, type, field);
					}
				} else if(
					(this.defaultEndianess == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
					(this.defaultEndianess == Endianness.LittleEndian && !BitConverter.IsLittleEndian)
				) {
					SwapEndian(data, type, field);
				}
			}
		}

		private byte[] StructToBytes(T data) {
			byte[] rawData = new byte[Marshal.SizeOf(data)];
			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				Marshal.StructureToPtr(data, rawDataPtr, false);
			} finally {
				handle.Free();
			}

			RespectEndianness(typeof(T), rawData);

			return rawData;
		}

		private T BytesToStruct(Func<int, byte[]> readBytes) {
			byte[] bytes = readBytes(Marshal.SizeOf(typeof(T)));
			return this.BytesToStruct(bytes);
		}

		private T BytesToStruct(byte[] rawData) {
			T result = default(T);

			RespectEndianness(typeof(T), rawData);

			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
			} finally {
				handle.Free();
			}

			return result;
		}
	}
}
