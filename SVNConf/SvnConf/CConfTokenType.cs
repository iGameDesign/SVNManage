using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNConf
{
	class CConfTokenType
	{
		public const int UNKNOWN = -1; // 未知的token
		// 各种可能作为token的使用ASC的编码作为定义，一举两得
		public const int LEFT_BRACKET = 91;  // 符号"[" 分段名称开始
		public const int RIGHT_BRACKET = 93;  // 符号"]" 分段名称结束
		public const int NUMBERSIGN = 35;  // 符号"#" 注释行
		public const int EQUAL = 61;  // 符号"=" 表达式赋值
		public const int AT = 64;  // 符号"@" 用于分组变量
		public const int BACKQUOTE = 96;  // 符号"`" 反引号
		public const int TILDE = 126; // 符号"~" 
		public const int EXCLAM = 33; // 符号"!" 
		public const int DOLLAR = 36; // 符号"$" 
		public const int PERCENT = 37; // 符号"%" 
		public const int CARET = 94; // 符号"^" 
		public const int AND = 38; // 符号"&" 
		public const int STAR = 42; // 符号"*" 
		public const int PARENLEFT = 40; // 符号"("
		public const int PARENRIGHT = 41; // 符号")"
		public const int MINUS = 45;	// 符号"-"
		public const int UNDERSCORE = 95; // 符号"_"
		public const int PLUS = 43; // 符号"+"
		public const int BRACELEFT = 123; // 符号"{"
		public const int BRACERIGHT = 125; // 符号"}"
		public const int SEMICOLON = 59; // 符号";"
		public const int COLON = 58; // 符号":"
		public const int QUOTE = 39; // 符号"'"
		public const int DOUBLEQUOTE = 34; // 符号'"'
		public const int SLASH = 47; // 符号"/"
		public const int BACKSLASH = 92; // 符号"\"
		public const int BAR = 124; // 符号"|"
		public const int COMMA = 44; // 符号","
		public const int LESS = 60; // 符号"<"
		public const int GREATER = 62; // 符号">"
		public const int QUESTION = 63; // 符号"?"
		public const int SPACE = 32; // 符号" "
	}
}
