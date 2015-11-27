using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapheneRpc
{
	public class Numeric
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		static public string SerialisedDecimal(decimal d)
		{
			return d.ToString("0.##########");
		}
	}
}
