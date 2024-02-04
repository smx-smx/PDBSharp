#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using Smx.PDBSharp.BuiltinTypeLeaf;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp
{
	public class TypeResolver : IPDBService
	{
		private readonly TPI.Serializer tpi;
		private readonly TPIHash.Data tpiHash;

		private (uint, uint) GetClosestTIOFF(UInt32 typeIndex) {
			var typeIndexToOffset = tpiHash.TypeIndexToOffset;
			if (typeIndexToOffset == null) throw new InvalidOperationException();

			bool hasPrec = typeIndexToOffset.TryPredecessor(typeIndex, out var prec);
			bool hasSucc = typeIndexToOffset.TrySuccessor(typeIndex, out var succ);

			if (hasPrec && hasSucc) {
				//[prev] <this> [next]
				//$TODO: maybe succ is closer?
				return (prec.Key, prec.Value);
			} else if (hasPrec) {
				//[prev] <this> EOF
				return (prec.Key, prec.Value);
			} else if (hasSucc) {
				//BEGIN <this> [next]
				return (tpi.Data.Header.MinTypeIndex, 0);
			} else {
				throw new InvalidDataException();
			}
		}

		public ILeafResolver? GetTypeByIndex(UInt32 TypeIndex) {
			if (!tpi.HasTi(TypeIndex)) {
				if (tpi.IsBuiltinTi(TypeIndex)) {
					var builtin = new BuiltinTypeLeaf.Data(TypeIndex);
					return new DirectLeafData(new LeafContext(
						typeIndex: TypeIndex,
						type: LeafType.SPECIAL_BUILTIN,
						data: builtin
					));
				}

				return null;
			}

			var typeIndexToOffset = tpiHash.TypeIndexToOffset;
			if (typeIndexToOffset == null) throw new InvalidOperationException();

			UInt32 typeOffset;
			if (typeIndexToOffset.Contains(TypeIndex)) {
				typeOffset = tpi.Data.Header.HeaderSize + typeIndexToOffset[TypeIndex];
				return tpi.ReadType(typeOffset);
			} else {
				(var closestTi, var closestOff) = GetClosestTIOFF(TypeIndex);

				uint curOffset = closestOff;
				for (uint ti = closestTi; ti <= TypeIndex; ti++) {
					uint offset;
					if (typeIndexToOffset.Contains(ti)) {
						// use existing TIOff
						offset = typeIndexToOffset[ti];
						curOffset += tpi.GetLeafSize(offset);
					} else {
						typeIndexToOffset[ti] = curOffset;
						curOffset += tpi.GetLeafSize(curOffset);
					}
				}

				//safety
				if (!typeIndexToOffset.Contains(TypeIndex)) {
					throw new InvalidDataException($"Type Index {TypeIndex} not found");
				}

				return GetTypeByIndex(TypeIndex);
			}
		}

		public TypeResolver(IServiceContainer ctx) {
			tpi = ctx.GetService<TPI.Serializer>();
			tpiHash = ctx.GetService<TPIHash.Data>();
		}
	}
}
