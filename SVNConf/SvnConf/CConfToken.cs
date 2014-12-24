using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNConf
{
	// 分割单元，一个独立的Token分段
    class CConfToken
    {
		private int m_type = CConfTokenType.UNKNOWN;

		public int Type
		{
			get { return m_type; }
			set { m_type = value; }
		}
		private string m_value = ""; // 存放的都是字符串单元

		public string Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		public CConfToken(int type = -1, string value = "")
		{
			m_type = type;
			m_value = value;
		}

		// 单例模式，不必使用多份Token，重用对象实例
		private static CConfToken m_token = new CConfToken();

		public CConfToken Create(int type = -1, string value = "")
		{
			m_token.Type = type;
			m_token.Value = value;
			return m_token;
		}
    }
}
