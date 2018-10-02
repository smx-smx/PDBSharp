#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	/**
	 *  [S_FRAMEPROC]
		00000000   1E 00 12 10 C8 00 00 00  C0 00 00 00 08 00 00 00   ····È···À·······
		00000010   00 00 00 00 00 00 00 00  00 00 [00 80] 12 00 00 00   ···········?···· 

		$TODO: 00 80 is skipped, is this intended? padding issues?
	 **/

	[Flags]
	public enum FrameProcSymFlags : UInt32
	{
		HasAlloca = 1 << 0,
		HasSetJmp = 1 << 1,
		HasLongJmp = 1 << 2,
		HasInlineAsm = 1 << 3,
		HasEH = 1 << 4,
		HasInlineSpec = 1 << 5,
		HasSEH = 1 << 6,
		IsNaked = 1 << 7,
		HasSecurityChecks = 1 << 8, // /GS
		HasAsyncEH = 1 << 9, // /EHa
		GSNoStackOrdering = 1 << 10,
		WasInlined = 1 << 11,
		HasGSCheck = 1 << 12,
		HasSafeBuffers = 1 << 13,
		//skip 14-15, 16-17
		HasPogo = 1 << 18, //PGO/PGU
		HasValidPogoCounts = 1 << 19,
		OptimizedForSpeed = 1 << 20,
		HasCFGChecks = 1 << 21,
		HasCFWChecks = 1 << 22
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct FRAMEPROCSYM
	{
		public UInt32 FrameSize;
		public UInt32 PaddingSize;
		public UInt32 PaddingOffset;
		public UInt32 SizeOfCalleeSavedRegisters;
		public UInt32 ExceptionHandlerOffset;
		public UInt16 ExceptionHandlerSection;

		private UInt32 flags;

		public FrameProcSymFlags Flags {
			get {
				return (FrameProcSymFlags)flags;
			}
		}

		public UInt16 EncodedLocalBasePointer {
			get {
				return (UInt16)((flags >> 14) & 3);
			}
		}

		public UInt16 EncodedParamBasePointer {
			get {
				return (UInt16)((flags >> 16) & 3);
			}
		}
	}

	public class FrameProcSymReader : SymbolReaderBase
	{
		public readonly FRAMEPROCSYM Data;
		public FrameProcSymReader(Stream stream) : base(stream) {
			Data = ReadStruct<FRAMEPROCSYM>();
		}
	}
}
