#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using C5;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
		public UInt32 NumTypes;

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

	public class TPIReader : ReaderBase
	{
		public readonly TPIHeader Header;

		private readonly Context ctx;

		public event OnLeafDataDelegate OnLeafData;


		private (uint, uint) GetClosestTIOFF(UInt32 typeIndex) {
			bool hasPrec = ctx.TpiHashReader.TypeIndexToOffset.TryPredecessor(typeIndex, out var prec);
			bool hasSucc = ctx.TpiHashReader.TypeIndexToOffset.TrySuccessor(typeIndex, out var succ);

			if (hasPrec && hasSucc) {
				//[prev] <this> [next]
				//$TODO: maybe succ is closer?
				return (prec.Key, prec.Value);
			} else if (hasPrec) {
				//[prev] <this> EOF
				return (prec.Key, prec.Value);
			} else if (hasSucc) {
				//BEGIN <this> [next]
				return (Header.MinTypeIndex, 0);
			} else {
				throw new InvalidDataException();
			}
		}

		private uint GetTypeSize(UInt32 offset) {
			return PerformAt(Header.HeaderSize + offset, () => {
				return (uint)ReadUInt16() + sizeof(UInt16);
			});
		}

		private bool HasTi(UInt32 TypeIndex) {
			return TypeIndex >= Header.MinTypeIndex && TypeIndex < Header.NumTypes;
		}

		private bool IsBuiltinTi(UInt32 TypeIndex) {
			return TypeIndex <= ((uint)SpecialTypeMode.NearPointer128 | 0xFF);
		}

		public ILeafContainer GetTypeByIndex(UInt32 TypeIndex) {
			if (!HasTi(TypeIndex)) {
				if (IsBuiltinTi(TypeIndex)) {
					ILeaf builtin = new BuiltinTypeLeaf(TypeIndex);
					return new DirectLeafProvider(TypeIndex, LeafType.SPECIAL_BUILTIN, builtin);
				}
				return null;
			}

			UInt32 typeOffset;
			if (ctx.TpiHashReader.TypeIndexToOffset.Contains(TypeIndex)) {
				typeOffset = Header.HeaderSize + ctx.TpiHashReader.TypeIndexToOffset[TypeIndex];
				return PerformAt(typeOffset, () => ReadType());
			} else {
				(var closestTi, var closestOff) = GetClosestTIOFF(TypeIndex);

				uint curOffset = closestOff;
				for(uint ti=closestTi; ti<=TypeIndex; ti++) {
					uint offset;
					if (ctx.TpiHashReader.TypeIndexToOffset.Contains(ti)) {
						// use existing TIOff
						offset = ctx.TpiHashReader.TypeIndexToOffset[ti];
						curOffset += GetTypeSize(offset);
					} else {
						ctx.TpiHashReader.TypeIndexToOffset[ti] = curOffset;
						curOffset += GetTypeSize(curOffset);
					}
				}

				//safety
				if (!ctx.TpiHashReader.TypeIndexToOffset.Contains(TypeIndex)) {
					throw new InvalidDataException($"Type Index {TypeIndex} not found");
				}

				return GetTypeByIndex(TypeIndex);
			}
		}

		public TPIReader(Context ctx, Stream stream) : base(stream) {
			this.ctx = ctx;

			Header = ReadStruct<TPIHeader>();
			if(Header.HeaderSize != Marshal.SizeOf<TPIHeader>()) {
				throw new InvalidDataException();
			}	

			if(!Enum.IsDefined(typeof(TPIVersion), Header.Version)) {
				throw new InvalidDataException();
			}

			if(((int)Header.Hash.StreamNumber) != -1){
				ctx.TpiHashReader = new HashDataReader(this, new MemoryStream(ctx.StreamTableReader.GetStream(Header.Hash.StreamNumber)));
			}


#if false
			if(Header.Version != TPIVersion.V80) {
				throw new NotImplementedException($"TPI Version {Header.Version} not supported yet");
			}
#endif
		}

		private ILeafContainer ReadType() {
			UInt16 length = ReadUInt16();
			if (length == 0) {
				return null;
			}

			int dataSize = length + sizeof(UInt16);
			byte[] leafDataBuf = new byte[dataSize];

			MemoryStream stream = new MemoryStream(leafDataBuf);
			BinaryWriter wr = new BinaryWriter(stream);
			wr.Write(length);
			wr.Write(ReadBytes(length));

			OnLeafData?.Invoke(leafDataBuf);

			stream.Position = 0;
			TypeDataReader rdr = new TypeDataReader(ctx, stream);
			return rdr.ReadTypeLazy();
		}

		public IEnumerable<ILeafContainer> ReadTypes() {
			long savedPos = Stream.Position;
			long processed = 0;
			while (processed < Header.GpRecSize) {
				yield return ReadType();
				processed += Stream.Position - savedPos;
				savedPos = Stream.Position;
			}
		}
	}
}
