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
using System.Runtime.InteropServices;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp
{
	public struct TPISlice
	{
		public UInt32 Offset;
		public UInt32 Size;
	}

	public struct TPIHash
	{
		public Int16 StreamNumber;
		public UInt16 AuxHashStreamNumber;

		public UInt32 HashKeySize;
		public UInt32 NumHashBuckets;

		/// <summary>
		/// offset,size pairs of hash values
		/// </summary>
		public TPISlice HashValues;
		/// <summary>
		/// offset,size pairs of type indices
		/// </summary>
		public TPISlice TypeOffsets;
		/// <summary>
		/// offset,size pairs of hash head list
		/// </summary>
		public TPISlice HashHeadList;
	}

	public struct TPIHeader
	{
		public TPIVersion Version;
		public UInt32 HeaderSize;

		public UInt32 MinTypeIndex;
		public UInt32 MaxTypeIndex;

		public UInt32 GpRecSize;
		public TPIHash Hash;
	}

	public enum TPIVersion : UInt32
	{
		V40 = 19950410,
		V41 = 19951122,
		V50Beta = 19960307,
		V50 = 19961031,
		V70 = 19990903,
		V80 = 20040203
	}

	public delegate void OnLeafDataDelegate(byte[] data);

	namespace TPI {
		delegate ILeafResolver? ReadTypeDelegate(out long dataSize);

		public class Data {
			public TPIHeader Header = new TPIHeader();
		}

		public class Serializer : ISerializer<Data>, IPDBService {
			private readonly IServiceContainer sc;
			private readonly PDBFile pdb;
			private readonly SpanStream stream;
			private readonly ReadTypeDelegate TypeReader;


			public event OnLeafDataDelegate? OnLeafData;
			public ILazy<IEnumerable<ILeafResolver?>> lazyLeafContainers;
			public IEnumerable<ILeafResolver?>? Types => lazyLeafContainers.Value;

			public Serializer(IServiceContainer sc, SpanStream stream) {
				this.sc = sc;
				this.pdb = sc.GetService<PDBFile>();
				this.stream = stream;
				switch (pdb.Type) {
					case PDBType.Old:
						TypeReader = ReadTypeOld;
						break;
					default:
						TypeReader = ReadType;
						break;
				}

				lazyLeafContainers = LazyFactory.CreateLazy(ReadTypes);
			}

			public Data Data = new Data();

			private static TPIHeader ImportJGOld(JGHeaderOld oldHeader) {
				TPIHeader hdr = new TPIHeader() {
					MinTypeIndex = oldHeader.MinTi,
					MaxTypeIndex = oldHeader.MaxTi,
					GpRecSize = oldHeader.GpRecSize
				};
				return hdr;
			}

			private ILeafResolver? ReadTypeOld(out long dataSize) {
				uint hash = stream.ReadUInt32();

				// we have no length, so we just pass all memory (after the hash)
				SpanStream memStream = new SpanStream(stream.Memory.Slice((int)stream.Position));
				TypeDataReader rdr = new TypeDataReader(sc, memStream);

				// without a length, and with a small amount of data, we can just read this directly
				ILeafResolver? leaf = rdr.ReadTypeDirect(false);

				// hash + type data
				dataSize = stream.Position + rdr.Position;

				stream.Position += rdr.Position;
				return leaf;
			}


			public ILeafResolver? ReadType(uint typeOffset) {
				return stream.PerformAt(typeOffset, () => ReadType(out long datSize));
			}


			private IEnumerable<ILeafResolver?> ReadTypes() {
				long processed = 0;
				while (processed < Data.Header.GpRecSize) {
					yield return TypeReader(out long dataSize);
					processed += dataSize;
				}
			}

			private ILeafResolver? ReadType(out long dataSize) {
				ushort length = stream.ReadUInt16();
				if (length == 0) {
					dataSize = sizeof(ushort);
					return null;
				}
				dataSize = sizeof(ushort) + length;
				stream.Position -= sizeof(ushort);

#if false //$TODO
			{
				var leafDataBuf = ReadBytes((int)length + sizeof(ushort));

				UInt32 leafHash = HasherV2.HashBufferV8(leafDataBuf, 0xFFFFFFFF);
				UInt32 hash = HasherV2.HashData(leafDataBuf, Header.Hash.NumHashBuckets);

				Position -= sizeof(ushort) + length;
			}
#endif

				//OnLeafData?.Invoke(leafDataBuf);

				var typeSpan = stream.Memory.Slice((int)stream.Position, (int)dataSize);
				TypeDataReader rdr = new TypeDataReader(sc, new SpanStream(typeSpan));
				stream.Position += dataSize;

				return rdr.ReadTypeLazy();
			}

			public Data Read() {
				if (pdb.Type == PDBType.Old) {
					
					JGHeaderOld oldHdr = sc.GetService<JGHeaderOld>();
					Data.Header = ImportJGOld(oldHdr);
				} else {
					Data.Header = stream.Read<TPIHeader>();
					if (Data.Header.HeaderSize != Marshal.SizeOf<TPIHeader>()) {
						throw new InvalidDataException();
					}

					if (!Enum.IsDefined(typeof(TPIVersion), Data.Header.Version)) {
						throw new InvalidDataException();
					}
				}

				#if false
				if(Data.Header.Version!= TPIVersion.V80) {
					throw new NotImplementedException($"TPI Version {Header.Version} not supported yet");
				}
				#endif

				return Data;
			}

			public void Write(Data Data) {
				throw new NotImplementedException();
			}

			public uint GetLeafSize(uint offset) {
				return stream.PerformAt(Data.Header.HeaderSize + offset, () => {
					return (uint)stream.ReadUInt16() + sizeof(UInt16);
				});
			}

			public bool HasTi(UInt32 TypeIndex) {
				return TypeIndex >= Data.Header.MinTypeIndex && TypeIndex < Data.Header.MaxTypeIndex;
			}

			public bool IsBuiltinTi(UInt32 TypeIndex) {
				return TypeIndex <= ((uint)SpecialTypeMode.NearPointer128 | 0xFF);
			}
		}
	}
}
