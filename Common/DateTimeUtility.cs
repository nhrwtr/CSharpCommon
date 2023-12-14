using System;

namespace Common
{
    public class DateTimeUtility
    {
        /// <summary>
        /// 絶対日時のオフセットを維持しつつ日時型に変換する
        /// </summary>
        /// <param name="dateTime">絶対日時</param>
        /// <returns>
        /// Offset=Utcの場合：<see cref="DateTimeKind.Utc"/> の日時型。
        /// Offset=Localの場合：<see cref="DateTimeKind.Local"/> の日時型。
        /// それ以外(特定の時差・タイムゾーンを指定した)の場合：時差を含む<see cref="DateTimeKind.Unspecified"/> の日時型
        /// </returns>
        public static DateTime KeepOffsetTo(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
            {
                // UTC標準時の場合
                return dateTime.UtcDateTime;
            }
            else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
            {
                // コンピュータの時差が合致する場合
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            }
            else
            {
                // コンピュータの時差と異なる場合
                return dateTime.DateTime;
            }
        }
    }
}
