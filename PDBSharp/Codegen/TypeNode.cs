#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System.Collections.Generic;

namespace Smx.PDBSharp.Codegen
{
	public class TypeNode : INode
	{
		public readonly ILeafContainer Type;
		public bool Visited { get; set; }

		public IList<TypeNode> TypeDependencies = new List<TypeNode>();

		public TypeNode(ILeafContainer leaf) {
			this.Type = leaf;
			this.HandleType();
		}

		private void HandleType() {
			switch (this.Type.Data) {
				case LF_CLASS_STRUCTURE_INTERFACE csi:
					AddDependency(csi.DerivedType);
					AddDependency(csi.VShapeTableType);
					break;
				case LF_POINTER lfPointer:
					AddDependency(lfPointer.UnderlyingType);
					break;
				case LF_ENUM lfEnum:
					AddDependency(lfEnum.FieldType);
					AddDependency(lfEnum.UnderlyingType);
					break;
				case LF_ARGLIST lfArgList:
					foreach (var arg in lfArgList.ArgumentTypes) {
						AddDependency(arg);
					}
					break;
				case LF_ARRAY lfArray:
					AddDependency(lfArray.ElementType);
					AddDependency(lfArray.IndexingType);
					break;
				case LF_FIELDLIST lfFieldList:
					foreach (var field in lfFieldList.Fields) {
						AddDependency(field);
					}
					break;
				case LF_PROCEDURE lfProc:
					AddDependency(lfProc.ArgumentListType);
					AddDependency(lfProc.ReturnValueType);
					break;
				case LF_UNION lfUnion:
					AddDependency(lfUnion.FieldType);
					break;
			}
		}

		private TypeNode AddDependency(ILeafContainer leaf) {
			TypeNode node = new TypeNode(leaf);
			TypeDependencies.Add(node);
			return node;
		}
	}
}
