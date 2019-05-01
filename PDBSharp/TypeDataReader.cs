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

		private static readonly Dictionary<LeafType, ConstructorInfo> parsers;
		static TypeDataReader() {
			parsers = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<LeafReaderAttribute>() != null)
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<LeafReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(Stream) }
				));
		}

		public TypeDataReader(Stream stream) : base(stream) {
		}

		public LeafType LeafType { get; private set; }

		/// <summary>
		/// Read a varying leaf which can be either a leaf type or a raw data marker
		/// </summary>
		/// <param name="dataSize"></param>
		/// <param name="leafType"></param>
		/// <returns>true if we found a leaf, false if we found raw data marker</returns>
		protected ILeaf ReadVaryingType(out uint dataSize) {
			UInt16 leafValue = Reader.ReadUInt16();
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
			ILeaf leaf = new TypeDataReader(Stream).ReadType(hasSize: false);
			return leaf;

		}

		private void ConsumePadding() {
			long remaining = Stream.Length - Stream.Position;
			long savedPos = Stream.Position;
			while(remaining-- > 0) {
				byte b = Reader.ReadByte();
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
				size = Reader.ReadUInt16();
			}
			UInt16 type = Reader.ReadUInt16();
			if(!Enum.IsDefined(typeof(LeafType), type)) {
				throw new InvalidDataException();
			}

			this.LeafType = (LeafType)type;

			ILeaf typeSym = null;
			if (parsers.ContainsKey(LeafType)) {
				typeSym = (ILeaf)parsers[LeafType].Invoke(new object[] { Stream });
			} else {
				throw new NotImplementedException($"{LeafType} not supported yet");
			}

			ConsumePadding();
			return typeSym;

		}
	}
}