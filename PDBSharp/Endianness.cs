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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
    /* http://stackoverflow.com/a/2624377 */
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Struct, Inherited = true)]
    public class EndianAttribute : Attribute
    {
        public Endianness Endianness { get; private set; }

        public EndianAttribute(Endianness endianness)
        {
            this.Endianness = endianness;
        }
    }

    public enum Endianness
    {
        BigEndian,
        LittleEndian
    }
}
