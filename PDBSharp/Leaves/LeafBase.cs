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
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Smx.PDBSharp.Leaves
{
	public interface ILeafType {
		public string? UdtName { get; }
		public bool IsUdtSourceLine { get; }
		public bool IsGlobalDefnUdtWithUniqueName { get; }
		public bool IsLocalDefnUdtWithUniqueName { get; }
		public bool IsDefnUdt { get; }
		public bool IsGlobalDefnUdt { get; }
	}

	public class LeafTypeHelper {
		public static bool IsUdtAnon(ILeafType leaf) {
			string? udtName = leaf.UdtName;
			if (udtName == null) return false;

			string[] utag = new string[] {
					"::<unnamed-tag>",
					"::__unnamed"
				};

			foreach (string tag in utag) {
				if (udtName == tag.Substring(2))
					return true;

				if (udtName.EndsWith(tag))
					return true;
			}
			return false;
		}
	}

	public abstract class LeafBase
	{
		protected readonly IServiceContainer ctx;
		protected readonly SpanStream stream;

		private readonly ILazy<TypeDataReader> reader;

		public long Length {
			get {
				Debug.Assert(reader.Value != null);
				return reader.Value.Length;
			}
		}

		public LeafBase(IServiceContainer ctx, SpanStream stream) {
			this.ctx = ctx;
			this.stream = stream;

			reader = LazyFactory.CreateLazy(() => new TypeDataReader(ctx, stream));
		}

		protected TypeDataReader CreateReader() {
			Debug.Assert(reader.Value != null);
			return reader.Value;
		}

		protected TypeDataWriter CreateWriter(LeafType type, bool hasSize = true) {
			return new TypeDataWriter(ctx, stream, type, hasSize);
		}
	}
}
