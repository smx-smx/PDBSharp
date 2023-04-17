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

	public class TPIReader : SpanStream
	{
		public readonly TPIHeader Header;

		private readonly IServiceContainer ctx;

		public event OnLeafDataDelegate? OnLeafData;

		public ILazy<IEnumerable<ILeafResolver?>> lazyLeafContainers;

		public IEnumerable<ILeafResolver?>? Types => lazyLeafContainers.Value;

		private delegate ILeafResolver? ReadTypeDelegate(out long dataSize);
		private readonly ReadTypeDelegate TypeReader;

		public uint GetLeafSize(UInt32 offset) {
			return PerformAt(Header.HeaderSize + offset, () => {
				return (uint)ReadUInt16() + sizeof(UInt16);
			});
		}

		public bool HasTi(UInt32 TypeIndex) {
			return TypeIndex >= Header.MinTypeIndex && TypeIndex < Header.MaxTypeIndex;
		}

		public bool IsBuiltinTi(UInt32 TypeIndex) {
			return TypeIndex <= ((uint)SpecialTypeMode.NearPointer128 | 0xFF);
		}

		private static TPIHeader ImportJGOld(JGHeaderOld oldHeader) {
			TPIHeader hdr = new TPIHeader() {
				MinTypeIndex = oldHeader.MinTi,
				MaxTypeIndex = oldHeader.MaxTi,
				GpRecSize = oldHeader.GpRecSize
			};
			return hdr;
		}

		public TPIReader(IServiceContainer ctx, SpanStream stream) : base(stream) {
			this.ctx = ctx;

			PDBFile pdb = ctx.GetService<PDBFile>();
			if(pdb.Type == PDBType.Old) {
				TypeReader = ReadTypeOld;
				JGHeaderOld oldHdr = ctx.GetService<JGHeaderOld>();
				Header = ImportJGOld(oldHdr);
			} else {
				Header = Read<TPIHeader>();
				if (Header.HeaderSize != Marshal.SizeOf<TPIHeader>()) {
					throw new InvalidDataException();
				}

				if (!Enum.IsDefined(typeof(TPIVersion), Header.Version)) {
					throw new InvalidDataException();
				}
				TypeReader = ReadType;
			}


#if false
			if(Header.Version != TPIVersion.V80) {
				throw new NotImplementedException($"TPI Version {Header.Version} not supported yet");
			}
#endif

			lazyLeafContainers = LazyFactory.CreateLazy(ReadTypes);
		}

		public ILeafResolver? ReadType(uint typeOffset) {
			return PerformAt(typeOffset, () => ReadType(out long datSize));
		}

		private ILeafResolver? ReadTypeOld(out long dataSize) {
			uint hash = ReadUInt32();

			// we have no length, so we just pass all memory (after the hash)
			SpanStream memStream = new SpanStream(Memory.Slice((int)Position));
			TypeDataReader rdr = new TypeDataReader(ctx, memStream);

			// without a length, and with a small amount of data, we can just read this directly
			ILeafResolver? leaf = rdr.ReadTypeDirect(false);

			// hash + type data
			dataSize = Position + rdr.Position;
			
			Position += rdr.Position;
			return leaf;
		}

		private ILeafResolver? ReadType(out long dataSize) {
			ushort length = ReadUInt16();
			if (length == 0) {
				dataSize = sizeof(ushort);
				return null;
			}
			dataSize = sizeof(ushort) + length;
			Position -= sizeof(ushort);

#if false //$TODO
			{
				var leafDataBuf = ReadBytes((int)length + sizeof(ushort));

				UInt32 leafHash = HasherV2.HashBufferV8(leafDataBuf, 0xFFFFFFFF);
				UInt32 hash = HasherV2.HashData(leafDataBuf, Header.Hash.NumHashBuckets);

				Position -= sizeof(ushort) + length;
			}
#endif

			//OnLeafData?.Invoke(leafDataBuf);

			var typeSpan = Memory.Slice((int)Position, (int)dataSize);
			TypeDataReader rdr = new TypeDataReader(ctx, new SpanStream(typeSpan));
			Position += dataSize;

			return rdr.ReadTypeLazy();
		}

		private IEnumerable<ILeafResolver?> ReadTypes() {
			long processed = 0;
			while (processed < Header.GpRecSize) {
				yield return TypeReader(out long dataSize);
				processed += dataSize;
			}
		}
	}
}
