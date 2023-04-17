#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion

namespace Smx.PDBSharp
{
	public delegate void OnModuleDataDelegate(ModuleInfo modInfo, byte[] data);

	public interface IModuleContainer
	{
		event OnModuleDataDelegate OnModuleData;
		ModuleInfo Info { get; }
		IModule? Module { get; }
	}
}
