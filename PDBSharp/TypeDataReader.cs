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
	public class TypeDataReader : ReaderBase, ILeaf
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

		private static readonly Dictionary<LeafType, ConstructorInfo> readers;
		static TypeDataReader() {

			var allReaders = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<LeafReaderAttribute>() != null);

			var withStream = allReaders.ToDictionary(
					// key
					t => t.GetCustomAttribute<LeafReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(Stream) }
				)).Where(p => p.Value != null);

			var withContext = allReaders.ToDictionary(
					// key
					t => t.GetCustomAttribute<LeafReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(PDBFile), typeof(Stream) }
				)).Where(p => p.Value != null);

			readers = withStream.Concat(withContext)
				.ToDictionary(i => i.Key, i => i.Value);

		}

		protected readonly PDBFile PDB;
		public TypeDataReader(PDBFile pdb, Stream stream) : base(stream) {
			this.PDB = pdb;
		}

		public Lazy<ILeaf> ReadIndexedTypeLazy() {
			UInt32 TI = ReadUInt32();
			return new Lazy<ILeaf>(() => {
				if (TI == 0)
					return null;
				return PDB.TPI.GetTypeByIndex(TI);
			});
		}

		public Lazy<ILeaf> ReadIndexedType16Lazy() {
			UInt16 TI = ReadUInt16();
			return new Lazy<ILeaf>(() => {
				if (TI == 0)
					return null;
				return PDB.TPI.GetTypeByIndex(TI); //WidenTI
			});
		}

		/// <summary>
		/// Read a varying leaf which can be either a leaf type or a raw data marker
		/// </summary>
		/// <param name="dataSize"></param>
		/// <param name="leafType"></param>
		/// <returns>true if we found a leaf, false if we found raw data marker</returns>
		protected ILeaf ReadVaryingType(out uint dataSize) {
			UInt16 leafValue = ReadUInt16();
			if (leafValue < (ushort)LeafType.LF_NUMERIC) {
				// $TODO: leafValue is not a leaf marker, but it's still a valid ushort
				// do we need to save this?
				dataSize = 0;
				return null;
			}

			if(!Enum.IsDefined(typeof(LeafType), leafValue)) {
				throw new InvalidDataException($"Unknown leaf type {leafValue} while computing leaf data size");
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

		public ILeaf ReadType(bool hasSize = true) {
			UInt16 size = 0;
			if (hasSize) {
				size = ReadUInt16();
			}
			LeafType leafType = ReadEnum<LeafType>();

			ILeaf typeSym = null;
			if (readers.ContainsKey(leafType)) {
				ConstructorInfo ctor = readers[leafType];
				object[] args;

				switch (ctor.GetParameters().Length) {
					case 1:
						args = new object[] { Stream };
						break;
					case 2:
						args = new object[] { PDB, Stream };
						break;
					default:
						throw new NotSupportedException();
				}

				typeSym = (ILeaf)readers[leafType].Invoke(args);
			} else {
				throw new NotImplementedException($"{leafType} not supported yet");
			}

			ConsumePadding();
			return typeSym;

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