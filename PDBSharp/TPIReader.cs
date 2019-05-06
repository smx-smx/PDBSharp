#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using C5;
using MoreLinq;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
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

	public class TPIReader : ReaderBase
	{
		private readonly StreamTableReader stRdr;


		public readonly TPIHeader Header;

		private readonly HashDataReader TPIHash;

		public IEnumerable<ILeaf> Types;


		private (uint, uint) GetClosestTIOFF(UInt32 typeIndex) {
			bool hasPrec = TPIHash.TypeIndexToOffset.TryPredecessor(typeIndex, out var prec);
			bool hasSucc = TPIHash.TypeIndexToOffset.TrySuccessor(typeIndex, out var succ);

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

		public ILeaf GetTypeByIndex(UInt32 TypeIndex) {
			if (!HasTi(TypeIndex))
				return null;

			UInt32 typeOffset;
			if (TPIHash.TypeIndexToOffset.Contains(TypeIndex)) {
				typeOffset = Header.HeaderSize + TPIHash.TypeIndexToOffset[TypeIndex];
				return PerformAt(typeOffset, () => ReadType());
			} else {
				(var closestTi, var closestOff) = GetClosestTIOFF(TypeIndex);

				uint curOffset = closestOff;
				for(uint ti=closestTi; ti<=TypeIndex; ti++) {
					uint offset;
					if (TPIHash.TypeIndexToOffset.Contains(ti)) {
						// use existing TIOff
						offset = TPIHash.TypeIndexToOffset[ti];
						curOffset += GetTypeSize(offset);
					} else {
						TPIHash.TypeIndexToOffset[ti] = curOffset;
						curOffset += GetTypeSize(curOffset);
					}
				}

				//safety
				if (!TPIHash.TypeIndexToOffset.Contains(TypeIndex)) {
					throw new InvalidDataException($"Type Index {TypeIndex} not found");
				}

				return GetTypeByIndex(TypeIndex);
			}
		}

		private readonly PDBFile pdb;

		public TPIReader(PDBFile pdb, StreamTableReader stRdr, Stream stream) : base(stream) {
			this.pdb = pdb;
			this.stRdr = stRdr;

			Header = ReadStruct<TPIHeader>();
			if(Header.HeaderSize != Marshal.SizeOf<TPIHeader>()) {
				throw new InvalidDataException();
			}	

			if(!Enum.IsDefined(typeof(TPIVersion), Header.Version)) {
				throw new InvalidDataException();
			}

			TPIHash = new HashDataReader(this, new MemoryStream(stRdr.GetStream(Header.Hash.StreamNumber)));


#if false
			if(Header.Version != TPIVersion.V80) {
				throw new NotImplementedException($"TPI Version {Header.Version} not supported yet");
			}
#endif
		}

		private ILeaf ReadType() {
			UInt16 length = ReadUInt16();
			if (length == 0) {
				return null;
			}

			int dataSize = length + sizeof(UInt16);
			byte[] symDataBuf = new byte[dataSize];

			BinaryWriter wr = new BinaryWriter(new MemoryStream(symDataBuf));
			wr.Write(length);
			wr.Write(ReadBytes(length));

			TypeDataReader rdr = new TypeDataReader(pdb, new MemoryStream(symDataBuf));
			return rdr.ReadType();
		}

		public IEnumerable<ILeaf> ReadTypes() {
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
