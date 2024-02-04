#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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

	namespace PDBStream
	{
		public class Data : IPDBService {
			public PDBPublicVersion Version;
			public UInt32 Signature;
			public UInt32 NumberOfUpdates; //AGE

			public Guid? NewSignature;
			public NameIndexTable.Lookup? NameTable;
		}

		public class Serializer(SpanStream stream) {
			public Data Data = new Data();
			public Data Read() {
				var Version = stream.ReadEnum<PDBPublicVersion>();
				var Signature = stream.ReadUInt32();
				var NumberOfUpdates = stream.ReadUInt32();

				if (Version < PDBPublicVersion.VC4 || Version > PDBPublicVersion.VC140) {
					throw new NotImplementedException();
				}

				Guid? NewSignature = null;
				if (Version > PDBPublicVersion.VC70Dep) {
					NewSignature = stream.Read<Guid>();
				}

				var NameTable = new NameIndexTable.Lookup(Deserializers.ReadNameIndexTable(stream));

				bool flagContinue = true;
				while (flagContinue && stream.Position + sizeof(uint) < stream.Length) {
					UInt32 signature = stream.ReadUInt32();
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

				Data = new Data {
					Version = Version,
					Signature = Signature,
					NumberOfUpdates = NumberOfUpdates,
					NewSignature = NewSignature,
					NameTable = NameTable
				};

				return Data;
			}
		}
	}
}
