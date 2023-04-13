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
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public class TypeDataReader : SpanStreamEx
	{

		public delegate string ReadStringDelegate();
		public new ReadStringDelegate ReadString16;

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
		private int maxAlignment = 16; //LF_PAD0 .. LF_PAD15

		private void InitVariants() {
			switch (ctx.GetService<PDBFile>().Type) {
				case PDBType.Small:
					ReadString16 = base.ReadString16;
					break;
				case PDBType.Old:
					ReadString16 = base.ReadString16NoTerm;
					maxAlignment = 4;
					break;
			}
		}

		public TypeDataReader(IServiceContainer ctx, SpanStream data) : base(data) {
			this.ctx = ctx;
			InitVariants();
		}

		public TypeDataReader(IServiceContainer ctx, Memory<byte> data) : base(data) {
			this.ctx = ctx;
			InitVariants();
		}

		public ILeafContainer ReadIndexedType32Lazy() {
			UInt32 TI = ReadUInt32();
			return new LazyLeafProvider(ctx, TI);
		}

		public ILeafContainer ReadIndexedType16Lazy() {
			UInt16 TI = ReadUInt16();
			return new LazyLeafProvider(ctx, TI);
		}

		public unsafe ILeafContainer ReadIndexedTypeLazy<T>() where T : unmanaged {
			switch (sizeof(T)) {
				case 2:
					return ReadIndexedType16Lazy();
				case 4:
					return ReadIndexedType32Lazy();
				default:
					throw new InvalidOperationException("Invalid type, can only read 16 and 32bit indexed types");
			}
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

			TypeDataReader rdr = new TypeDataReader(ctx, this);
			ILeafContainer leaf = rdr.ReadTypeDirect(hasSize: false);
			//ILeafContainer leaf = new TypeDataReader(ctx, this).ReadTypeDirect(hasSize: false);

			// add leaf size
			this.Position += rdr.Position;
			
			return leaf;

		}

		private void ConsumePadding() {
			long remaining = Length - Position;
			if(remaining < 1) {
				return;
			}

			long savedPosition = Position;

			// example: LF_PAD3
			byte b = ReadByte();
			if(b < (byte)LeafType.LF_PAD0) {
				goto rollback;
			}
			if(--remaining < 1) {
				return;
			}

			// example: 3
			int alignment = b & 0x0F;

			for(int i=1; remaining > 0 && i<alignment; i++, remaining--) {
				// we now expect 2..1..
				b = ReadByte();
				if((b & 0x0F) != (alignment - i)) {
					goto rollback;
				}
			}
			return;

			rollback:
			Position = savedPosition;
			return;
		}

		private ILeaf CreateLeafStream(LeafType leafType) {
			switch (leafType) {
				case LeafType.LF_ALIAS:
					return new LF_ALIAS(ctx, this);
				case LeafType.LF_ARGLIST:
					return new LF_ARGLIST(ctx, this);
				case LeafType.LF_ARRAY_16t:
					return new LF_ARRAY<ushort>(ctx, this);
				case LeafType.LF_ARRAY:
					return new LF_ARRAY<uint>(ctx, this);
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
				case LeafType.LF_CLASS_16t:
				case LeafType.LF_STRUCTURE_16t:
					return new LF_CLASS_STRUCTURE_INTERFACE16(ctx, this);
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
				case LeafType.LF_POINTER_16t:
					return new LF_POINTER16t(ctx, this);
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
				if(size == 0) {
					throw new InvalidDataException("Leaf size field cannot be 0");
				}
			}
			LeafType leafType = ReadEnum<LeafType>();
			
			ILeaf typeSym = CreateLeafStream(leafType);
			typeSym.Read();

			Position += (typeSym as LeafBase).Length;
			ConsumePadding();
			
			// for PDB 1.0: hash collides with padding, and is not properly encoded sometimes
			AlignStream(2);

#if !PEFF
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