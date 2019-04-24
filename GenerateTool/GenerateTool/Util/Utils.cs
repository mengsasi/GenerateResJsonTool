using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool {

    static class Utils {

        /// <summary>
        /// 空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendSpace( this string str ) {
            return str + " ";
        }

        /// <summary>
        /// 逗号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendComma( this string str ) {
            return str + ",";
        }

        /// <summary>
        /// 中括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendLeftSquareBracket( this string str ) {
            return str + "[";
        }

        public static string AppendRightSquareBracket( this string str ) {
            return str + "]";
        }

        /// <summary>
        /// 大括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendLeftBrace( this string str ) {
            return str + "{";
        }

        public static string AppendRightBrace( this string str ) {
            return str + "}";
        }

        /// <summary>
        /// tap
        /// </summary>
        /// <param name="str"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string AppendTap( this string str, int count = 1 ) {
            for( int i = 0; i < count; i++ ) {
                str += "\t";
            }
            return str;
        }

        /// <summary>
        /// 换行
        /// </summary>
        /// <param name="str"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string AppendBr( this string str, int count = 1 ) {
            for( int i = 0; i < count; i++ ) {
                str += "\n";
            }
            return str;
        }

        /// <summary>
        /// 引号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendMark( this string str ) {
            return str + "\"";
        }

        /// <summary>
        /// 冒号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendColon( this string str ) {
            return str + ":";
        }
    }
}
