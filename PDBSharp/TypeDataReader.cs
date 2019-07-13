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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;

namespace Smx.PDBSharp
{
	public class TypeDataReader : ReaderBase
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

		protected readonly Context ctx;
		public TypeDataReader(Context ctx, Stream stream) : base(stream) {
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

			if(!Enum.IsDefined(typeof(LeafType), leafValue)) {
				throw new InvalidDataException($"Unknown ILeaf type {leafValue} while computing ILeaf data size");
			}

			LeafType leafType = (LeafType)leafValue;
			dataSize = PrimitiveDataSizes[leafType];

			Stream.Seek(-2, SeekOrigin.Current);
			ILeafContainer leaf = new TypeDataReader(ctx, Stream).ReadTypeLazy(hasSize: false);
			return leaf;

		}

		private void ConsumePadding() {
			long remaining = Stream.Length - Stream.Position;
			long savedPos = Stream.Position;
			while(remaining-- > 0) {
				byte b = ReadByte();
				if (b >= (byte)LeafType.LF_PAD0 && b <= (byte)LeafType.LF_PAD15) {
					continue;
				}
				Stream.Seek(-1, SeekOrigin.Current);
				break;
			}
		}

		private ILeaf ReadLeaf(LeafType leafType) {
			switch (leafType) {
				case LeafType.LF_ARGLIST:
					return new LF_ARGLIST(ctx, Stream);
				case LeafType.LF_ARRAY:
					return new LF_ARRAY(ctx, Stream);
				case LeafType.LF_BCLASS:
					return new LF_BCLASS(ctx, Stream);
				case LeafType.LF_BITFIELD:
					return new LF_BITFIELD(ctx, Stream);
				case LeafType.LF_CHAR:
					return new LF_CHAR(ctx, Stream);
				case LeafType.LF_CLASS:
				case LeafType.LF_STRUCTURE:
					return new LF_CLASS(ctx, Stream);
				case LeafType.LF_ENUM:
					return new LF_ENUM(ctx, Stream);
				case LeafType.LF_ENUMERATE:
				case LeafType.LF_ENUMERATE_ST:
					return new LF_ENUMERATE(ctx, Stream);
				case LeafType.LF_FIELDLIST:
					return new LF_FIELDLIST(ctx, Stream);
				case LeafType.LF_INDEX:
					return new LF_INDEX(ctx, Stream);
				case LeafType.LF_LONG:
					return new LF_LONG(ctx, Stream);
				case LeafType.LF_MEMBER:
					return new LF_MEMBER(ctx, Stream);
				case LeafType.LF_METHOD:
					return new LF_METHOD(ctx, Stream);
				case LeafType.LF_METHODLIST:
					return new LF_METHODLIST(ctx, Stream);
				case LeafType.LF_MODIFIER:
					return new LF_MODIFIER(ctx, Stream);
				case LeafType.LF_MFUNCTION:
					return new LF_MFUNCTION(ctx, Stream);
				case LeafType.LF_NESTTYPE:
					return new LF_NESTTYPE(ctx, Stream);
				case LeafType.LF_ONEMETHOD:
					return new LF_ONEMETHOD(ctx, Stream);
				case LeafType.LF_POINTER:
					return new LF_POINTER(ctx, Stream);
				case LeafType.LF_PROCEDURE:
					return new LF_PROCEDURE(ctx, Stream);
				case LeafType.LF_QUADWORD:
					return new LF_QUADWORD(ctx, Stream);
				case LeafType.LF_REAL32:
					return new LF_REAL32(ctx, Stream);
				case LeafType.LF_REAL64:
					return new LF_REAL64(ctx, Stream);
				case LeafType.LF_SHORT:
					return new LF_SHORT(ctx, Stream);
				case LeafType.LF_STMEMBER:
					return new LF_STMEMBER(ctx, Stream);
				case LeafType.LF_ULONG:
					return new LF_ULONG(ctx, Stream);
				case LeafType.LF_UNION:
					return new LF_UNION(ctx, Stream);
				case LeafType.LF_UQUADWORD:
					return new LF_UQUADWORD(ctx, Stream);
				case LeafType.LF_USHORT:
					return new LF_USHORT(ctx, Stream);
				case LeafType.LF_VARSTRING:
					return new LF_VARSTRING(ctx, Stream);
				case LeafType.LF_VBCLASS:
				case LeafType.LF_IVBCLASS:
					return new LF_VBCLASS(ctx, Stream);
				case LeafType.LF_VFTABLE:
					return new LF_VFTABLE(ctx, Stream);
				case LeafType.LF_VFTPATH_16t:
					return new LF_VFTPATH_16t(ctx, Stream);
				case LeafType.LF_VFUNCTAB:
					return new LF_VFUNCTAB(ctx, Stream);
				case LeafType.LF_VTSHAPE:
					return new LF_VTSHAPE(ctx, Stream);
				case LeafType.SPECIAL_BUILTIN:
					throw new InvalidDataException("SPECIAL_BUILTIN is a custom ILeaf marker, it can't be present in a valid ctx file");
				default:
					throw new NotImplementedException($"Leaf {leafType} not supported yet");
			}
		}

		public ILeafContainer ReadTypeLazy(bool hasSize = true) {
			Lazy<ILeafContainer> delayedLeaf = new Lazy<ILeafContainer>(() => {
				return ReadTypeDirect(hasSize);
			});
			return new LazyLeafProvider(delayedLeaf);
		}

		public LeafBase ReadTypeDirect(bool hasSize = true) {
			UInt16 size = 0;
			if (hasSize) {
				size = ReadUInt16();
			}
			LeafType leafType = ReadEnum<LeafType>();
			ILeaf typeSym = ReadLeaf(leafType);

			ConsumePadding();
			return new DirectLeafProvider(0, leafType, typeSym);
		}
	}
}