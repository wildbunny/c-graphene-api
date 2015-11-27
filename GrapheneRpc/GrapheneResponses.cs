using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapheneRpc
{
	public class GrapheneErrorResponse
	{
		public int id;
		public GrapheneError error;
	}

	public class GrapheneResponse<T>
	{
		public int id;
		public T result;
	}

	public class GrapheneSocketResponse<T>
	{
		public int id;
		public string error;
		public GrapheneMethods method;
		public T result;
	}
}
