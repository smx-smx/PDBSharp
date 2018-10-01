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
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	class Program
	{
		static void Main(string[] args) {
			if(args.Length < 1) {
				Console.Error.WriteLine("Usage: [file.pdb]");
				Environment.Exit(1);
			}

			var file = new FileStream(args[0], FileMode.Open, FileAccess.Read);
			new PDBFile(file);

			Console.ReadLine();
		}
	}
}
