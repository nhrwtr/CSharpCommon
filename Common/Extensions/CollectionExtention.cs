using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    /// <summary>
    /// コレクションの拡張クラス
    /// </summary>
    public static class CollectionExtention
    {
        /// <summary>
        /// リスト内に等価なオブジェクトが1つでも存在する場合trueを返します
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">反復可能リスト</param>
        /// <param name="target">検証対象のオブジェクト</param>
        /// <returns>true: 1つ以上存在する | false: 存在しない</returns>
        public static bool Any<T>(this IEnumerable<T> self, T target)
            where T : IEquatable<T>
        {
            foreach (var item in self)
            {
                if (item.Equals(target))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// リスト内に等価なオブジェクトが1つでも存在する場合trueを返します
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">反復可能リスト</param>
        /// <param name="target">検証対象のオブジェクト</param>
        /// <returns>true: 1つ以上存在する | false: 存在しない</returns>
        public static bool ParallelAny<T>(this IEnumerable<T> self, T target)
            where T : IEquatable<T>
        {
            return self.AsParallel().Any(x => x.Equals(target));
        }

        /// <summary>
        /// リスト内のオブジェクトが全て等価なオブジェクトだった場合trueを返します
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">反復可能リスト</param>
        /// <param name="target">検証対象のオブジェクト</param>
        /// <returns>true: 等価オブジェクト | false: 部分一致。または一致しない</returns>
        public static bool All<T>(this IEnumerable<T> self, T target)
            where T : IEquatable<T>
        {
            foreach (var item in self)
            {
                if (!item.Equals(target))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// リスト内のオブジェクトが全て等価なオブジェクトだった場合trueを返します
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">反復可能リスト</param>
        /// <param name="target">検証対象のオブジェクト</param>
        /// <returns>true: 等価オブジェクト | false: 部分一致。または一致しない</returns>
        public static bool ParallelAll<T>(this IEnumerable<T> self, T target)
            where T : IEquatable<T>
        {
            return self.AsParallel().All(x => x.Equals(target));
        }

        /// <summary>
        /// 最初の要素のとき、指定の処理を行うメソッド
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">イテレーション可能なコレクション</param>
        /// <param name="firstAction">指定の処理</param>
        /// <returns>イテレーション可能なコレクション</returns>
        public static IEnumerable<T> FirstWith<T>(this IEnumerable<T> self, Action<T> firstAction)
        {
            var enumerator = self.GetEnumerator();
            if(enumerator.MoveNext())
            {
                // first item 
                firstAction(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    // other wise items
                    yield return enumerator.Current;
                }
            }
        }

        /// <summary>
        /// 最初の要素を除いた要素全てに対し、指定の処理を行うメソッド
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">イテレーション可能なコレクション</param>
        /// <param name="otherwiseAction">指定の処理</param>
        /// <returns>イテレーション可能なコレクション</returns>
        public static IEnumerable<T> ExceptFirstWith<T>(this IEnumerable<T> self, Action<T> otherwiseAction)
        {
            var enumerator = self.GetEnumerator();
            if (enumerator.MoveNext())
            {
                // first item 
                yield return enumerator.Current;

                while (enumerator.MoveNext())
                {
                    // other wise items
                    otherwiseAction(enumerator.Current);
                    yield return enumerator.Current;
                }
            }
        }
    }
}
