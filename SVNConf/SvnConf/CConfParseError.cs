using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

/*
 * throw new CConfParseError("", 10, ?)
 */

namespace SVNConf
{
    class CConfParseError : ApplicationException
    {
		private string m_value = ""; // 存放的都是字符串单元
		private int m_location = 0; // 当前分析的位置记录

		public CConfParseError()
        {
        }

		public CConfParseError(string message)
			: base(message)
		{
		}

		public CConfParseError(string message, int loc, string value)
			: base(message)
		{
		}
		
		public CConfParseError(string message, Exception inner)
            : base(message, inner)
        {
        }

		protected CConfParseError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
    }
}
