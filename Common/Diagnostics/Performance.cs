using System;
using System.Diagnostics;

namespace SharpTool.Diagnostics
{
    /// <summary>
    /// パフォーマンスを計測するクラス
    /// </summary>
    public class Performance : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly Action _callback = null;

        /// <summary>
        /// 経過時間を取得する
        /// </summary>
        public TimeSpan LapTime => _stopwatch.Elapsed;

        /// <summary>
        /// 計測を開始する
        /// </summary>
        public Performance()
        {
            _callback = () => { Console.WriteLine($"elapsed time: {_stopwatch.Elapsed.TotalSeconds:0.000}(s), {_stopwatch.Elapsed.TotalMilliseconds:0.000}(ms)"); };
            _stopwatch.Start();
        }

        /// <summary>
        /// 計測を開始する
        /// </summary>
        /// <param name="callback">計測終了後コルーチン</param>
        public Performance(Action callback)
        {
            _callback = callback;
            _stopwatch.Start();
        }

        /// <summary>
        /// 計測をストップする
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
        }

        /// <summary>
        /// 計測を再開する
        /// </summary>
        public void Restart()
        {
            _stopwatch.Restart();
        }

        /// <summary>
        /// このオブジェクトを破棄状態にする
        /// </summary>
        public void Dispose()
        {
            _stopwatch.Stop();
            _callback();
        }
    }
}
