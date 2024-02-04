using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	public interface ISerializer<T>
	{
		T Read();
		void Write(T Data);
	}

	public interface ISerializerFactory<T> {
		public ISerializer<T> CreateSerializer();
	}
}
