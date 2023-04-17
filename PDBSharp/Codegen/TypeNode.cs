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
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Codegen
{
	public class TypeNode : INode
	{
		public readonly LeafContext Type;
		public bool Visited { get; set; }

		public IList<TypeNode> TypeDependencies = new List<TypeNode>();

		public TypeNode(LeafContext leafContext) {
			this.Type = leafContext;
			this.HandleType();
		}

		private void HandleType() {
			switch (this.Type.Data) {
				case Leaves.LF_CLASS_STRUCTURE_INTERFACE.Data csi:
					if (csi.DerivedType != null) {
						AddDependency(csi.DerivedType);
					}

					if (csi.VShapeTableType != null) {
						AddDependency(csi.VShapeTableType);
					}
					break;
				case Leaves.LF_POINTER.Data lfPointer:
					if (lfPointer.UnderlyingType != null) {
						AddDependency(lfPointer.UnderlyingType);
					}
					break;
				case Leaves.LF_ENUM.Data lfEnum:
					if (lfEnum.FieldType != null) {
						AddDependency(lfEnum.FieldType);
					}
					if (lfEnum.UnderlyingType != null) {
						AddDependency(lfEnum.UnderlyingType);
					}
					break;
				case Leaves.LF_ARGLIST.Data lfArgList:
					foreach (var arg in lfArgList.ArgumentTypes) {
						if (arg != null) {
							AddDependency(arg);
						}
					}
					break;
				case Leaves.LF_ARRAY.Data lfArray:
					if (lfArray.ElementType != null) {
						AddDependency(lfArray.ElementType);
					}
					if (lfArray.IndexingType != null) {
						AddDependency(lfArray.IndexingType);
					}
					break;
				case Leaves.LF_FIELDLIST.Data lfFieldList:
					foreach (var field in lfFieldList.Fields) {
						if (field != null) {
							AddDependency(field);
						}
					}
					break;
				case Leaves.LF_PROCEDURE.Data lfProc:
					if (lfProc.ArgumentListType != null) {
						AddDependency(lfProc.ArgumentListType);
					}
					if (lfProc.ReturnValueType != null) {
						AddDependency(lfProc.ReturnValueType);
					}
					break;
				case Leaves.LF_UNION.Data lfUnion:
					if (lfUnion.FieldType != null) {
						AddDependency(lfUnion.FieldType);
					}
					break;
			}
		}

		private TypeNode AddDependency(ILeafResolver leafContext) {
			TypeNode node = new TypeNode(leafContext.Ctx);
			TypeDependencies.Add(node);
			return node;
		}
	}
}
