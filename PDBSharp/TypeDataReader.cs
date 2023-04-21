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
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp
{
	public class TypeDataReader : SpanStreamEx
	{

		public delegate string ReadStringDelegate();
		public new ReadStringDelegate ReadString16 = () => throw new InvalidOperationException();

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
		
		private readonly TypeResolver resolver;

		public TypeDataReader(IServiceContainer ctx, SpanStream data) : base(data) {
			this.ctx = ctx;
			resolver = ctx.GetService<TypeResolver>();
			InitVariants();
		}

		public TypeDataReader(IServiceContainer ctx, Memory<byte> data) : base(data) {
			this.ctx = ctx;
			resolver = ctx.GetService<TypeResolver>();
			InitVariants();
		}

		public ILeafResolver? ReadIndexedType32Lazy() {
			UInt32 TI = ReadUInt32();
			return resolver.GetTypeByIndex(TI);
		}

		public ILeafResolver? ReadIndexedType16Lazy() {
			UInt16 TI = ReadUInt16();
			return resolver.GetTypeByIndex(TI);
		}

		public unsafe ILeafResolver? ReadIndexedTypeLazy<T>() where T : unmanaged {
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
		public ILeafResolver? ReadVaryingType(out uint dataSize) {
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

			// "fork" the leaf stream
			var subStream = SliceHere(2 + (int)dataSize);
			TypeDataReader rdr = new TypeDataReader(ctx, subStream);
			ILeafResolver? leaf = rdr.ReadTypeDirect(hasSize: false);
			//ILeafContainer leaf = new TypeDataReader(ctx, this).ReadTypeDirect(hasSize: false);

			// add leaf size
			this.Position += 2 + dataSize;
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

		private ILeafSerializer CreateLeafSerializer(LeafType leafType) {
			switch (leafType) {
				case LeafType.LF_ALIAS:
					return new Leaves.LF_ALIAS.Serializer(ctx, this);
				case LeafType.LF_ARGLIST:
					return new Leaves.LF_ARGLIST.Serializer(ctx, this);
				case LeafType.LF_ARRAY_16t:
					return new Leaves.LF_ARRAY.Serializer<ushort>(ctx, this);
				case LeafType.LF_ARRAY:
					return new Leaves.LF_ARRAY.Serializer<uint>(ctx, this);
				case LeafType.LF_BCLASS:
					return new Leaves.LF_BCLASS.Serializer(ctx, this);
				case LeafType.LF_BITFIELD:
					return new Leaves.LF_BITFIELD.Serializer(ctx, this);
				case LeafType.LF_CHAR:
					return new Leaves.LF_CHAR.Serializer(ctx, this);
				case LeafType.LF_CLASS:
				case LeafType.LF_STRUCTURE:
				case LeafType.LF_INTERFACE:
					return new Leaves.LF_CLASS_STRUCTURE_INTERFACE.Serializer(ctx, this);
				case LeafType.LF_CLASS_16t:
				case LeafType.LF_STRUCTURE_16t:
					return new Leaves.LF_CLASS_STRUCTURE_INTERFACE16.Serializer(ctx, this);
				case LeafType.LF_ENUM:
					return new Leaves.LF_ENUM.Serializer(ctx, this);
				case LeafType.LF_ENUMERATE:
				case LeafType.LF_ENUMERATE_ST:
					return new Leaves.LF_ENUMERATE.Serializer(ctx, this);
				case LeafType.LF_FIELDLIST:
					return new Leaves.LF_FIELDLIST.Serializer(ctx, this);
				case LeafType.LF_INDEX:
					return new Leaves.LF_INDEX.Serializer(ctx, this);
				case LeafType.LF_LONG:
					return new Leaves.LF_LONG.Serializer(ctx, this);
				case LeafType.LF_MEMBER:
					return new Leaves.LF_MEMBER.Serializer(ctx, this);
				case LeafType.LF_METHOD:
					return new Leaves.LF_METHOD.Serializer(ctx, this);
				case LeafType.LF_METHODLIST:
					return new Leaves.LF_METHODLIST.Serializer(ctx, this);
				case LeafType.LF_MODIFIER:
					return new Leaves.LF_MODIFIER.Serializer(ctx, this);
				case LeafType.LF_MFUNCTION:
					return new Leaves.LF_MFUNCTION.Serializer(ctx, this);
				case LeafType.LF_NESTTYPE:
					return new Leaves.LF_NESTTYPE.Serializer(ctx, this);
				case LeafType.LF_ONEMETHOD:
					return new Leaves.LF_ONEMETHOD.Serializer(ctx, this);
				case LeafType.LF_POINTER:
					return new Leaves.LF_POINTER.Serializer(ctx, this);
				case LeafType.LF_POINTER_16t:
					return new Leaves.LF_POINTER16t.Serializer(ctx, this);
				case LeafType.LF_PROCEDURE:
					return new Leaves.LF_PROCEDURE.Serializer(ctx, this);
				case LeafType.LF_QUADWORD:
					return new Leaves.LF_QUADWORD.Serializer(ctx, this);
				case LeafType.LF_REAL32:
					return new Leaves.LF_REAL32.Serializer(ctx, this);
				case LeafType.LF_REAL64:
					return new Leaves.LF_REAL64.Serializer(ctx, this);
				case LeafType.LF_SHORT:
					return new Leaves.LF_SHORT.Serializer(ctx, this);
				case LeafType.LF_STMEMBER:
					return new Leaves.LF_STMEMBER.Serializer(ctx, this);
				case LeafType.LF_ULONG:
					return new Leaves.LF_ULONG.Serializer(ctx, this);
				case LeafType.LF_UNION:
					return new Leaves.LF_UNION.Serializer(ctx, this);
				case LeafType.LF_UQUADWORD:
					return new Leaves.LF_UQUADWORD.Serializer(ctx, this);
				case LeafType.LF_USHORT:
					return new Leaves.LF_USHORT.Serializer(ctx, this);
				case LeafType.LF_VARSTRING:
					return new Leaves.LF_VARSTRING.Serializer(ctx, this);
				case LeafType.LF_VBCLASS:
				case LeafType.LF_IVBCLASS:
					return new Leaves.LF_VBCLASS.Serializer(ctx, this);
				case LeafType.LF_VFTABLE:
					return new Leaves.LF_VFTABLE.Serializer(ctx, this);
				case LeafType.LF_VFTPATH_16t:
					return new Leaves.LF_VFTPATH_16t.Serializer(ctx, this);
				case LeafType.LF_VFUNCTAB:
					return new Leaves.LF_VFUNCTAB.Serializer(ctx, this);
				case LeafType.LF_VTSHAPE:
					return new Leaves.LF_VTSHAPE.Serializer(ctx, this);
				case LeafType.SPECIAL_BUILTIN:
					throw new InvalidDataException("SPECIAL_BUILTIN is a custom ILeaf marker, it can't be present in a valid ctx file");
				default:
					throw new NotImplementedException($"Leaf {leafType} not supported yet");
			}
		}

		public ILeafResolver? ReadTypeLazy(bool hasSize = true) {
			long typePos = Position;

			ILazy<ILeafResolver?> delayedLeaf = LazyFactory.CreateLazy<ILeafResolver?>(() => {
				Position = typePos;
				return ReadTypeDirect(hasSize);
			});

			return new LazyLeafData(delayedLeaf);
		}

		public ILeafResolver? ReadTypeDirect(bool hasSize = true) {
			long typeStartOffset = Position;

			UInt16 size = 0;
			if (hasSize) {
				size = ReadUInt16();
				if(size == 0) {
					throw new InvalidDataException("Leaf size field cannot be 0");
				}
			}
			LeafType leafType = ReadEnum<LeafType>();
			
			ILeafSerializer typeSym = CreateLeafSerializer(leafType);
			typeSym.Read();

			var leafBase = typeSym as Leaves.LeafBase;
			if (leafBase == null) {
				throw new InvalidDataException("Failed to read leaf");
			}
			
			Position += leafBase.Length;
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

			return new DirectLeafData(new LeafContext(
				typeIndex: 0,
				type: leafType,
				data: typeSym.GetData()
			));
		}
	}
}