using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    /// <summary>
    /// <see cref="System.String"/>型の拡張クラス
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 対象文字列がマッチした場合対象文字列を削除した文字列を返却する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="word">対象文字列</param>
        /// <returns>除去後の文字列を新しく返す。マッチしなかった場合は、このオブジェクト自体を返却する</returns>
        public static string RemoveAt(this string self, string word)
        {
            return self.Replace(word, "");
        }

        #region params

        /// <summary>
        /// 対象文字列いずれかがマッチした場合trueを返す
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="possibility">対象文字列(配列)</param>
        /// <returns>true: 該当あり | false: 該当なし</returns>
        public static bool AnyOne(this string self, params string[] possibility)
        {
            foreach (var word in possibility)
            {
                if (word == self)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定した区切り文字列で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="separator">区切り文字列。複数指定可能</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, params string[] separator)
        {
            return self.Split(separator, StringSplitOptions.None);
        }

        /// <summary>
        /// 指定した区切り文字で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="separator">区切り文字。複数指定可能</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, params char[] separator)
        {
            return self.Split(separator, StringSplitOptions.None);
        }

        /// <summary>
        /// 指定した区切り文字列で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="option">空文字になった時のオプション</param>
        /// <param name="separator">区切り文字列。複数指定可能</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, StringSplitOptions option, params string[] separator)
        {
            return self.Split(separator, option);
        }

        /// <summary>
        /// 指定した区切り文字で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="option">空文字になった時のオプション</param>
        /// <param name="separator">区切り文字。複数指定可能</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, StringSplitOptions option, params char[] separator)
        {
            return self.Split(separator, option);
        }

        #endregion params

        #region IEnumerable<string>

        /// <summary>
        /// 対象文字列いずれかがマッチした場合trueを返す
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="possibility">対象文字列(反復子)</param>
        /// <returns>true: 該当あり | false: 該当なし</returns>
        public static bool AnyOne(this string self, IEnumerable<string> possibility)
        {
            foreach (var word in possibility)
            {
                if (word == self)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定した区切り文字列で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="separator">区切り文字列。反復子</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, IEnumerable<string> separator)
        {
            return self.Split(separator.ToArray(), StringSplitOptions.None);
        }

        /// <summary>
        /// 指定した区切り文字で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="separator">区切り文字。反復子</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, IEnumerable<char> separator)
        {
            return self.Split(separator.ToArray(), StringSplitOptions.None);
        }

        /// <summary>
        /// 指定した区切り文字列で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="option">空文字になった時のオプション</param>
        /// <param name="separator">区切り文字列。反復子</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, StringSplitOptions option, IEnumerable<string> separator)
        {
            return self.Split(separator.ToArray(), option);
        }

        /// <summary>
        /// 指定した区切り文字で文字列を分割する
        /// </summary>
        /// <param name="self">文字列</param>
        /// <param name="option">空文字になった時のオプション</param>
        /// <param name="separator">区切り文字。反復子</param>
        /// <returns>分割された文字列配列</returns>
        public static string[] Split(this string self, StringSplitOptions option, IEnumerable<char> separator)
        {
            return self.Split(separator.ToArray(), option);
        }

        #endregion IEnumerable<string>
    }
}
