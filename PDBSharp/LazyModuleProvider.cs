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
using System.Linq;
using System.Text;

namespace Smx.PDBSharp
{
	public delegate void OnModuleReaderInitDelegate(IModule module);
	public class LazyModuleProvider : IModuleContainer
	{
		public ModuleInfo Info => modInfo;

		public IModule Module => lazyModule.Value;

		private readonly Context ctx;
		private readonly Lazy<IModule> lazyModule;

		public event OnModuleReaderInitDelegate OnModuleReaderInit;
		public event OnModuleDataDelegate OnModuleData;

		private readonly ModuleInfo modInfo;

		private IModule ReadModule() {
			if(modInfo.StreamNumber < 0) {
				return null;
			}
			byte[] modData = ctx.StreamTableReader.GetStream(modInfo.StreamNumber);
			OnModuleData?.Invoke(modInfo, modData);

			MemoryStream modStream = new MemoryStream(modData);
			UInt32 signature = new BinaryReader(modStream).ReadUInt32();
			modStream.Position = 0;

			IModule modReader;
			if (Enum.IsDefined(typeof(CodeViewSignature), signature)) {
				modReader = new CodeViewModuleReader(ctx, modInfo, modStream);
			} else {
				modReader = new SourceFileModuleReader(ctx.Pdb, modStream);
			}

			OnModuleReaderInit?.Invoke(modReader);
			return modReader;
		}

		public LazyModuleProvider(Context ctx, ModuleInfo mod) {
			this.ctx = ctx;
			this.modInfo = mod;
			this.lazyModule = new Lazy<IModule>(ReadModule);
		}
	}
}
