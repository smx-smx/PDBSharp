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
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public class TypeResolver
	{
		private readonly TPIReader tpi;
		private readonly HashDataReader tpiHash;

		private (uint, uint) GetClosestTIOFF(UInt32 typeIndex) {
			bool hasPrec = tpiHash.TypeIndexToOffset.TryPredecessor(typeIndex, out var prec);
			bool hasSucc = tpiHash.TypeIndexToOffset.TrySuccessor(typeIndex, out var succ);

			if (hasPrec && hasSucc) {
				//[prev] <this> [next]
				//$TODO: maybe succ is closer?
				return (prec.Key, prec.Value);
			} else if (hasPrec) {
				//[prev] <this> EOF
				return (prec.Key, prec.Value);
			} else if (hasSucc) {
				//BEGIN <this> [next]
				return (tpi.Header.MinTypeIndex, 0);
			} else {
				throw new InvalidDataException();
			}
		}

		public ILeafContainer GetTypeByIndex(UInt32 TypeIndex) {
			if (!tpi.HasTi(TypeIndex)) {
				if (tpi.IsBuiltinTi(TypeIndex)) {
					ILeaf builtin = new BuiltinTypeLeaf(TypeIndex);
					return new DirectLeafProvider(TypeIndex, LeafType.SPECIAL_BUILTIN, builtin);
				}
				return null;
			}

			UInt32 typeOffset;
			if (tpiHash.TypeIndexToOffset.Contains(TypeIndex)) {
				typeOffset = tpi.Header.HeaderSize + tpiHash.TypeIndexToOffset[TypeIndex];
				return tpi.ReadType(typeOffset);
			} else {
				(var closestTi, var closestOff) = GetClosestTIOFF(TypeIndex);

				uint curOffset = closestOff;
				for (uint ti = closestTi; ti <= TypeIndex; ti++) {
					uint offset;
					if (tpiHash.TypeIndexToOffset.Contains(ti)) {
						// use existing TIOff
						offset = tpiHash.TypeIndexToOffset[ti];
						curOffset += tpi.GetLeafSize(offset);
					} else {
						tpiHash.TypeIndexToOffset[ti] = curOffset;
						curOffset += tpi.GetLeafSize(curOffset);
					}
				}

				//safety
				if (!tpiHash.TypeIndexToOffset.Contains(TypeIndex)) {
					throw new InvalidDataException($"Type Index {TypeIndex} not found");
				}

				return GetTypeByIndex(TypeIndex);
			}
		}

		public TypeResolver(IServiceContainer ctx) {
			tpi = ctx.GetService<TPIReader>();
			tpiHash = ctx.GetService<HashDataReader>();
		}
	}
}
