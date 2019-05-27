#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
	public class TypeDataReader : ReaderBase, ILeafData
	{
		private static readonly ReadOnlyDictionary<LeafType, uint> PrimitiveDataSizes = new ReadOnlyDictionary<LeafType, uint>(new Dictionary<LeafType, uint>() {
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

		protected readonly PDBFile PDB;
		public TypeDataReader(PDBFile pdb, Stream stream) : base(stream) {
			this.PDB = pdb;
		}

		public ILeaf ReadIndexedTypeLazy() {
			UInt32 TI = ReadUInt32();
			return new LazyLeafProvider(PDB, TI);
		}

		public ILeaf ReadIndexedType16Lazy() {
			UInt16 TI = ReadUInt16();
			return new LazyLeafProvider(PDB, TI);
		}

		/// <summary>
		/// Read a varying ILeaf which can be either a ILeaf type or a raw data marker
		/// </summary>
		/// <param name="dataSize"></param>
		/// <param name="leafType"></param>
		/// <returns>true if we found a ILeaf, false if we found raw data marker</returns>
		protected ILeaf ReadVaryingType(out uint dataSize) {
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
			ILeaf leaf = new TypeDataReader(this.PDB, Stream).ReadType(hasSize: false);
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

		private ILeafData ReadLeaf(LeafType leafType) {
			switch (leafType) {
				case LeafType.LF_ARGLIST:
					return new LF_ARGLIST(PDB, Stream);
				case LeafType.LF_ARRAY:
					return new LF_ARRAY(PDB, Stream);
				case LeafType.LF_BCLASS:
					return new LF_BCLASS(PDB, Stream);
				case LeafType.LF_BITFIELD:
					return new LF_BITFIELD(PDB, Stream);
				case LeafType.LF_CHAR:
					return new LF_CHAR(PDB, Stream);
				case LeafType.LF_CLASS:
				case LeafType.LF_STRUCTURE:
					return new LF_CLASS(PDB, Stream);
				case LeafType.LF_ENUM:
					return new LF_ENUM(PDB, Stream);
				case LeafType.LF_ENUMERATE:
				case LeafType.LF_ENUMERATE_ST:
					return new LF_ENUMERATE(PDB, Stream);
				case LeafType.LF_FIELDLIST:
					return new LF_FIELDLIST(PDB, Stream);
				case LeafType.LF_INDEX:
					return new LF_INDEX(PDB, Stream);
				case LeafType.LF_LONG:
					return new LF_LONG(PDB, Stream);
				case LeafType.LF_MEMBER:
					return new LF_MEMBER(PDB, Stream);
				case LeafType.LF_METHOD:
					return new LF_METHOD(PDB, Stream);
				case LeafType.LF_METHODLIST:
					return new LF_METHODLIST(PDB, Stream);
				case LeafType.LF_MODIFIER:
					return new LF_MODIFIER(PDB, Stream);
				case LeafType.LF_MFUNCTION:
					return new LF_MFUNCTION(PDB, Stream);
				case LeafType.LF_NESTTYPE:
					return new LF_NESTTYPE(PDB, Stream);
				case LeafType.LF_ONEMETHOD:
					return new LF_ONEMETHOD(PDB, Stream);
				case LeafType.LF_POINTER:
					return new LF_POINTER(PDB, Stream);
				case LeafType.LF_PROCEDURE:
					return new LF_PROCEDURE(PDB, Stream);
				case LeafType.LF_QUADWORD:
					return new LF_QUADWORD(PDB, Stream);
				case LeafType.LF_REAL32:
					return new LF_REAL32(PDB, Stream);
				case LeafType.LF_REAL64:
					return new LF_REAL64(PDB, Stream);
				case LeafType.LF_SHORT:
					return new LF_SHORT(PDB, Stream);
				case LeafType.LF_STMEMBER:
					return new LF_STMEMBER(PDB, Stream);
				case LeafType.LF_ULONG:
					return new LF_ULONG(PDB, Stream);
				case LeafType.LF_UNION:
					return new LF_UNION(PDB, Stream);
				case LeafType.LF_UQUADWORD:
					return new LF_UQUADWORD(PDB, Stream);
				case LeafType.LF_USHORT:
					return new LF_USHORT(PDB, Stream);
				case LeafType.LF_VARSTRING:
					return new LF_VARSTRING(PDB, Stream);
				case LeafType.LF_VBCLASS:
				case LeafType.LF_IVBCLASS:
					return new LF_VBCLASS(PDB, Stream);
				case LeafType.LF_VFTABLE:
					return new LF_VFTABLE(PDB, Stream);
				case LeafType.LF_VFTPATH_16t:
					return new LF_VFTPATH_16t(PDB, Stream);
				case LeafType.LF_VFUNCTAB:
					return new LF_VFUNCTAB(PDB, Stream);
				case LeafType.LF_VTSHAPE:
					return new LF_VTSHAPE(PDB, Stream);
				case LeafType.SPECIAL_BUILTIN:
					throw new InvalidDataException("SPECIAL_BUILTIN is a custom ILeaf marker, it can't be present in a valid PDB file");
				default:
					throw new NotImplementedException($"Leaf {leafType} not supported yet");
			}
		}

		public ILeaf ReadType(bool hasSize = true) {
			UInt16 size = 0;
			if (hasSize) {
				size = ReadUInt16();
			}
			LeafType leafType = ReadEnum<LeafType>();
			ILeafData typeSym = ReadLeaf(leafType);

			ConsumePadding();
			return new DirectLeafProvider(leafType, typeSym);

		}

		private string GetUdtName() {
			switch (this) {
				case LF_CLASS lfClass:
					return lfClass.Name;
				default:
					throw new NotImplementedException();
			}
		}

		private bool IsUdtAnon() {
			string[] utag = new string[] {
				"::<unnamed-tag>",
				"::__unnamed"
			};

			string UdtName = GetUdtName();
			foreach(string tag in utag) {
				if (UdtName.Contains(tag))
					return true;
			}
			return false;
		}

		public bool IsUdtSourceLine() {
			switch (this) {
				//$TODO
				/*case LF_UDT_SRC_LINE:
				case LF_UDT_MOD_SRC_LINE:
					return true;*/
				default:
					return false;
			}
		}

		public bool IsGlobalDefnUdtWithUniqueName() {
			switch (this) {
				case LF_CLASS _:
				case LF_UNION _:
				case LF_ENUM _:
				//$TODO
				//case LF_INTERFACE _:
					break;
				default:
					return false;
			}

			LF_CLASS leaf = (LF_CLASS)this;
			return (
				!leaf.FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
				!leaf.FieldProperties.HasFlag(TypeProperties.IsScoped) &&
				leaf.FieldProperties.HasFlag(TypeProperties.HasUniqueName) &&
				!IsUdtAnon()
			);

		}

		public bool IsGlobalDefnUdt() {
			switch (this) {
				//$TODO
				/*case LF_ALIAS _:
					return true;*/
				case LF_CLASS _:
				case LF_UNION _:
				case LF_ENUM _:
				//case LF_INTERFACE _:
					break;
				default:
					return false;
			}

			//$TODO: not tested
			LF_CLASS leaf = (LF_CLASS)this;
			return (
				!leaf.FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
				!leaf.FieldProperties.HasFlag(TypeProperties.IsScoped) &&
				!IsUdtAnon()
			);
		}
	}
}