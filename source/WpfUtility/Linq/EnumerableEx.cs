using System.Collections.Generic;
using System.Linq;

namespace WpfUtility.Linq
{
    /// <summary>
    /// 列挙子の拡張機能を提供します。
    /// </summary>
    public static class EnumerableEx
    {
        /// <summary>
        /// <see cref="System.Collections.IEnumerable"/> 型のコレクションのメンバーを連結します。
        /// 各メンバーの間には、指定した区切り記号が挿入されます。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="source">連結する値を格納しているコレクション。</param>
        /// <param name="separator">区切り記号として使用する文字列。</param>
        /// <returns></returns>
        public static string JoinString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source is IEnumerable<string>
                ? (IEnumerable<string>)source
                : source.Select(x => x.ToString()));
        }
    }
}