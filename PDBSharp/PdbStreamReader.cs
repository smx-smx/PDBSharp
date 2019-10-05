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
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	public enum PDBVersion : UInt32
	{
		VC2 = 19941610,
		VC4 = 19950623,
		VC41 = 19950814,
		VC50 = 19960307,
		VC98 = 19970604,
		VC70 = 20000404,
		VC70Dep = 19990604,  // deprecated vc70 implementation version
		VC80 = 20030901,
		VC110 = 20091201,
		VC140 = 20140508
	}

	public enum PDBFeature : UInt32
	{
		NoTypeMerge = 0x4D544F4E, //NOTM
		MinimalDebugInfo = 0x494E494D //MINI
	}

	public class PdbStreamReader : ReaderBase
	{
		public readonly PDBVersion Version;
		public readonly UInt32 Signature;
		public readonly UInt32 NumberOfUpdates; //AGE

		public readonly Guid NewSignature;

		public readonly NameIndexTableReader NameTable;

		private readonly bool ContainsIdStream;

		public PdbStreamReader(Context ctx, Stream stream) : base(stream) {
			Version = ReadEnum<PDBVersion>();
			Signature = ReadUInt32();
			NumberOfUpdates = ReadUInt32();

			if(Version < PDBVersion.VC4 || Version > PDBVersion.VC140) {
				return;
			}

			if(Version > PDBVersion.VC70Dep) {
				NewSignature = ReadStruct<Guid>();
			}

			NameTable = Deserializers.ReadNameIndexTable(this);

			bool flagContinue = true;
			while(flagContinue && stream.Position + sizeof(uint) < stream.Length) {
				UInt32 signature = ReadUInt32();
				if(Enum.IsDefined(typeof(PDBVersion), signature)) {
					PDBVersion version = (PDBVersion)signature;
					switch (version) {
						case PDBVersion.VC110:
						case PDBVersion.VC140:
							flagContinue = false;
							break;
					}	
				} else if(Enum.IsDefined(typeof(PDBFeature), signature)) {
				}
			}
		}
	}
}
