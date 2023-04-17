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
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public delegate void OnModuleReaderInitDelegate(IModule module);
	public class LazyModuleProvider : IModuleContainer
	{
        public ModuleInfo Info { get; }

        public IModule? Module => lazyModule.Value;

		private readonly ILazy<IModule?> lazyModule;

		public event OnModuleReaderInitDelegate? OnModuleReaderInit;
		public event OnModuleDataDelegate? OnModuleData;

		private readonly IServiceContainer ctx;

		private readonly StreamTableReader StreamTable;

		private IModule? ReadModule() {
			if (Info.StreamNumber < 0) {
				return null;
			}
			byte[] modData = StreamTable.GetStream(Info.StreamNumber);
			OnModuleData?.Invoke(Info, modData);

			SpanStream modStream = new SpanStream(modData);
			UInt32 signature = modStream.ReadUInt32();
			modStream.Position = 0;

			IModule modReader;
			if (Enum.IsDefined(typeof(CodeViewSignature), signature)) {
				modReader = new CodeViewModuleReader(ctx, Info, modStream);
			} else {
				modReader = new SourceFileModuleReader(ctx, modStream);
			}

			OnModuleReaderInit?.Invoke(modReader);
			return modReader;
		}

		public LazyModuleProvider(IServiceContainer ctx, ModuleInfo mod) {
			this.ctx = ctx;
			this.StreamTable = ctx.GetService<StreamTableReader>();

			this.Info = mod;
			this.lazyModule = LazyFactory.CreateLazy(ReadModule);
		}
	}
}
