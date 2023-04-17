#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Codegen
{
	public class CodeWriter
	{
		private readonly IEnumerable<SymbolNode> tree;

		private IndentedTextWriter itw;

		public CodeWriter(IEnumerable<SymbolNode> tree, TextWriter writer) {
			this.tree = tree;
			this.itw = new IndentedTextWriter(writer);
		}

		private Dictionary<SpecialType, string> SpecialTypeMaps = new Dictionary<SpecialType, string>() {
			{ SpecialType.None, "" },
			{ SpecialType.SByte, "int8_t" },
			{ SpecialType.Byte, "uint8_t" },
			{ SpecialType.UInt16, "uint16_t" },
			{ SpecialType.Int16Short, "int16_t" },
			{ SpecialType.UInt16Short, "uint16_t" },
			{ SpecialType.UInt32, "uint32_t" },
			{ SpecialType.Int32Long, "int32_t" },
			{ SpecialType.UInt32Long, "uint32_t" },
			{ SpecialType.UInt64, "uint64_t" },
			{ SpecialType.Int64Quad, "int64_t" },
			{ SpecialType.UInt64Quad, "uint64_t" },
			{ SpecialType.Void, "void" },
			{ SpecialType.WideCharacter, "wchar_t" },
			{ SpecialType.NarrowCharacter, "char" },
			{ SpecialType.SignedCharacter, "int8_t" },
			{ SpecialType.UnsignedCharacter, "uint8_t" },
			{ SpecialType.Character16, "int16_t" },
			{ SpecialType.Character32, "int32_t" },
		};

		private void WriteTypeNode(TypeNode type) {
			if (type.Visited)
				return;

			type.Visited = true;
		}

		private void WriteType(ILeafResolver? leafc) {
			if (leafc == null) return;
			var ctx = leafc.Ctx;
			if (ctx == null) return;

			switch (ctx.Type) {
				case LeafType.SPECIAL_BUILTIN:
					BuiltinTypeLeaf.Data? builtin = ctx.Data as BuiltinTypeLeaf.Data;
					if (builtin == null) throw new InvalidOperationException();

					itw.Write(SpecialTypeMaps[builtin.SpecialType]);
					if (builtin.TypeMode != SpecialTypeMode.Direct)
						itw.Write('*');
					break;
				case LeafType.LF_ARGLIST:
					if (ctx.Data is not Leaves.LF_ARGLIST.Data lfArgList) throw new InvalidOperationException();
					
					itw.Write('(');
					for (int i = 0; i < lfArgList.NumberOfArguments; i++) {
						WriteType(lfArgList.ArgumentTypes[i]);
						if (lfArgList.ArgumentTypes[i]?.Ctx?.Data is BuiltinTypeLeaf.Data bt
						    && i + 1 == lfArgList.NumberOfArguments
						    && bt.SpecialType == SpecialType.None
						    && bt.TypeMode == SpecialTypeMode.Direct
						) {
							itw.Write("...");
						} else {
							itw.Write($" a{i + 1}");
						}
						if (i + 1 < lfArgList.NumberOfArguments) {
							itw.Write(", ");
						}
					}
					itw.Write(')');
					break;
				case LeafType.LF_POINTER:
					if (ctx.Data is not Leaves.LF_POINTER.Data lfPtr) throw new InvalidOperationException();
					WriteType(lfPtr.UnderlyingType);
					itw.Write("*");
					if (lfPtr.Attributes.IsConst) {
						itw.Write(" const ");
					}
					switch (lfPtr.Attributes.PointerMode) {
						case PointerMode.LValueReference:
							itw.Write('&');
							break;
						case PointerMode.RValueReference:
							itw.Write("&&");
							break;
					}
					break;
				case LeafType.LF_MODIFIER:
					if (ctx.Data is not Leaves.LF_MODIFIER.Data lfMod) throw new InvalidOperationException();
					if (lfMod.Flags.HasFlag(CVModifier.Const)) {
						itw.Write("const ");
					}
					break;
				case LeafType.LF_ENUM:
					if (ctx.Data is not Leaves.LF_ENUM.Data lfEnum) throw new InvalidOperationException();
					itw.Write(lfEnum.Name);
					break;
				case LeafType.LF_STRUCTURE:
					if (ctx.Data is not Leaves.LF_CLASS_STRUCTURE_INTERFACE.Data lfcsi) throw new InvalidOperationException();
					itw.Write(lfcsi.Name);
					break;
				default:
					throw new NotImplementedException(ctx.Type.ToString());
			}
		}

		private void WriteGproc(Symbols.ProcSym32.Data gproc) {
			switch (gproc.Type?.Ctx.Data) {
				case Leaves.LF_PROCEDURE.Data lfProc:
					WriteType(lfProc.ReturnValueType);
					itw.Write($" {gproc.Name}");
					WriteType(lfProc.ArgumentListType);
					break;
				case Leaves.LF_MFUNCTION.Data lfMfunc:
					throw new NotImplementedException();
			}

			itw.Write(";");
			itw.WriteLine();
			itw.Flush(); //debug
		}

		private void WriteSymbol(SymbolNode sym) {
			if (sym.Visited)
				return;

			switch (sym.Symbol.Type) {
				case SymbolType.S_GPROC32:
					if (sym.Symbol.Data is not Symbols.ProcSym32.Data gproc) throw new InvalidOperationException();
					WriteGproc(gproc);
					break;
			}

			sym.Visited = true;
		}

		public void Write() {
			foreach (var sym in tree) {
				WriteSymbol(sym);
			}
		}
	}
}
