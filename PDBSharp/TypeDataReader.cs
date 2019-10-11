#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public class TypeDataReader : SpanStream
	{

		public static readonly ReadOnlyDictionary<LeafType, uint> PrimitiveDataSizes = new ReadOnlyDictionary<LeafType, uint>(new Dictionary<LeafType, uint>() {
			{ LeafType.LF_CHAR, 1 },
			{ LeafType.LF_SHORT, 2 },
			{ LeafType.LF_USHORT, 2 },
			{ LeafType.LF_LONG, 4 },
			{ LeafType.LF_ULONG, 4 },
			{ LeafType.LF_REAL32, 4 },
			{ LeafType.LF_REAL48, 6 },
			{ LeafType.LF_REAL64, 8 },
			{ LeafType.LF_REAL80, 10 },
			{ LeafType.LF_REAL128, 16 }
		});

		protected readonly IServiceContainer ctx;
		public TypeDataReader(IServiceContainer ctx, SpanStream data) : base(data) {
			this.ctx = ctx;
		}

		public ILeafContainer ReadIndexedTypeLazy() {
			UInt32 TI = ReadUInt32();
			return new LazyLeafProvider(ctx, TI);
		}

		public ILeafContainer ReadIndexedType16Lazy() {
			UInt16 TI = ReadUInt16();
			return new LazyLeafProvider(ctx, TI);
		}

		/// <summary>
		/// Read a varying ILeaf which can be either a ILeaf type or a raw data marker
		/// </summary>
		/// <param name="dataSize"></param>
		/// <param name="leafType"></param>
		/// <returns>true if we found a ILeaf, false if we found raw data marker</returns>
		public ILeafContainer ReadVaryingType(out uint dataSize) {
			UInt16 leafValue = ReadUInt16();
			if (leafValue < (ushort)LeafType.LF_NUMERIC) {
				// $TODO: ILeafValue is not a ILeaf marker, but it's still a valid ushort
				// do we need to save this?
				dataSize = 0;
				return null;
			}

			if (!Enum.IsDefined(typeof(LeafType), leafValue)) {
				throw new InvalidDataException($"Unknown ILeaf type {leafValue} while computing ILeaf data size");
			}

			LeafType leafType = (LeafType)leafValue;
			dataSize = PrimitiveDataSizes[leafType];
			Seek(-2, SeekOrigin.Current);

			ILeafContainer leaf = new TypeDataReader(ctx, this).ReadTypeDirect(hasSize: false);
			//ILeafContainer leaf = new TypeDataReader(ctx, this).ReadTypeLazy(hasSize: false);
			return leaf;

		}

		private void ConsumePadding() {
			long remaining = Length - Position;
			long savedPos = Position;
			while (remaining-- > 0) {
				byte b = ReadByte();
				if (b >= (byte)LeafType.LF_PAD0 && b <= (byte)LeafType.LF_PAD15) {
					continue;
				}
				Seek(-1, SeekOrigin.Current);
				break;
			}
		}

		private ILeaf ReadLeaf(LeafType leafType) {
			switch (leafType) {
				case LeafType.LF_ALIAS:
					return new LF_ALIAS(ctx, this);
				case LeafType.LF_ARGLIST:
					return new LF_ARGLIST(ctx, this);
				case LeafType.LF_ARRAY:
					return new LF_ARRAY(ctx, this);
				case LeafType.LF_BCLASS:
					return new LF_BCLASS(ctx, this);
				case LeafType.LF_BITFIELD:
					return new LF_BITFIELD(ctx, this);
				case LeafType.LF_CHAR:
					return new LF_CHAR(ctx, this);
				case LeafType.LF_CLASS:
				case LeafType.LF_STRUCTURE:
				case LeafType.LF_INTERFACE:
					return new LF_CLASS_STRUCTURE_INTERFACE(ctx, this);
				case LeafType.LF_ENUM:
					return new LF_ENUM(ctx, this);
				case LeafType.LF_ENUMERATE:
				case LeafType.LF_ENUMERATE_ST:
					return new LF_ENUMERATE(ctx, this);
				case LeafType.LF_FIELDLIST:
					return new LF_FIELDLIST(ctx, this);
				case LeafType.LF_INDEX:
					return new LF_INDEX(ctx, this);
				case LeafType.LF_LONG:
					return new LF_LONG(ctx, this);
				case LeafType.LF_MEMBER:
					return new LF_MEMBER(ctx, this);
				case LeafType.LF_METHOD:
					return new LF_METHOD(ctx, this);
				case LeafType.LF_METHODLIST:
					return new LF_METHODLIST(ctx, this);
				case LeafType.LF_MODIFIER:
					return new LF_MODIFIER(ctx, this);
				case LeafType.LF_MFUNCTION:
					return new LF_MFUNCTION(ctx, this);
				case LeafType.LF_NESTTYPE:
					return new LF_NESTTYPE(ctx, this);
				case LeafType.LF_ONEMETHOD:
					return new LF_ONEMETHOD(ctx, this);
				case LeafType.LF_POINTER:
					return new LF_POINTER(ctx, this);
				case LeafType.LF_PROCEDURE:
					return new LF_PROCEDURE(ctx, this);
				case LeafType.LF_QUADWORD:
					return new LF_QUADWORD(ctx, this);
				case LeafType.LF_REAL32:
					return new LF_REAL32(ctx, this);
				case LeafType.LF_REAL64:
					return new LF_REAL64(ctx, this);
				case LeafType.LF_SHORT:
					return new LF_SHORT(ctx, this);
				case LeafType.LF_STMEMBER:
					return new LF_STMEMBER(ctx, this);
				case LeafType.LF_ULONG:
					return new LF_ULONG(ctx, this);
				case LeafType.LF_UNION:
					return new LF_UNION(ctx, this);
				case LeafType.LF_UQUADWORD:
					return new LF_UQUADWORD(ctx, this);
				case LeafType.LF_USHORT:
					return new LF_USHORT(ctx, this);
				case LeafType.LF_VARSTRING:
					return new LF_VARSTRING(ctx, this);
				case LeafType.LF_VBCLASS:
				case LeafType.LF_IVBCLASS:
					return new LF_VBCLASS(ctx, this);
				case LeafType.LF_VFTABLE:
					return new LF_VFTABLE(ctx, this);
				case LeafType.LF_VFTPATH_16t:
					return new LF_VFTPATH_16t(ctx, this);
				case LeafType.LF_VFUNCTAB:
					return new LF_VFUNCTAB(ctx, this);
				case LeafType.LF_VTSHAPE:
					return new LF_VTSHAPE(ctx, this);
				case LeafType.SPECIAL_BUILTIN:
					throw new InvalidDataException("SPECIAL_BUILTIN is a custom ILeaf marker, it can't be present in a valid ctx file");
				default:
					throw new NotImplementedException($"Leaf {leafType} not supported yet");
			}
		}

		public ILeafContainer ReadTypeLazy(bool hasSize = true) {
			long typePos = Position;

			ILazy<ILeafContainer> delayedLeaf = LazyFactory.CreateLazy<ILeafContainer>(() => {
				Position = typePos;
				return ReadTypeDirect(hasSize);
			});
			return new LazyLeafProvider(ctx, delayedLeaf);
		}

		public LeafContainerBase ReadTypeDirect(bool hasSize = true) {
			long typeStartOffset = Position;

			UInt16 size = 0;
			if (hasSize) {
				size = ReadUInt16();
			}
			LeafType leafType = ReadEnum<LeafType>();
			ILeaf typeSym = ReadLeaf(leafType);

			ConsumePadding();

#if DEBUG
			long typeDataSize = size + sizeof(UInt16);
			UInt32 typeHash = PerformAt(typeStartOffset, () => {
				byte[] typeData = ReadBytes((int)typeDataSize);
				return HasherV2.HashBufferV8(typeData, 0xFFFFFFFF);
			});
#endif

			return new DirectLeafProvider(0, leafType, typeSym);
		}
	}
}