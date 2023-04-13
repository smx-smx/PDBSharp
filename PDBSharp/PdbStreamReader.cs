#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.IO;

namespace Smx.PDBSharp
{
	public enum PDBPublicVersion : UInt32
	{
		VC2 = 19941610,
		VC4 = 19950623,
		VC41 = 920924,
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

	public class PdbStreamReader : SpanStream
	{
		public readonly PDBPublicVersion Version;
		public readonly UInt32 Signature;
		public readonly UInt32 NumberOfUpdates; //AGE

		public readonly Guid NewSignature;

		public readonly NameIndexTableReader NameTable;

		private readonly bool ContainsIdStream;

		public PdbStreamReader(byte[] nameMapData) : base(nameMapData) {
			Version = ReadEnum<PDBPublicVersion>();
			Signature = ReadUInt32();
			NumberOfUpdates = ReadUInt32();

			if (Version < PDBPublicVersion.VC4 || Version > PDBPublicVersion.VC140) {
				return;
			}

			if (Version > PDBPublicVersion.VC70Dep) {
				NewSignature = Read<Guid>();
			}

			NameTable = Deserializers.ReadNameIndexTable(this);

			bool flagContinue = true;
			while (flagContinue && Position + sizeof(uint) < Length) {
				UInt32 signature = ReadUInt32();
				if (Enum.IsDefined(typeof(PDBPublicVersion), signature)) {
					PDBPublicVersion version = (PDBPublicVersion)signature;
					switch (version) {
						case PDBPublicVersion.VC110:
						case PDBPublicVersion.VC140:
							flagContinue = false;
							break;
					}
				} else if (Enum.IsDefined(typeof(PDBFeature), signature)) {
				}
			}
		}
	}
}
