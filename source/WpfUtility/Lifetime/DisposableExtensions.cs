using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUtility.Lifetime
{
    /// <summary>
    /// Disposableの拡張機能を提供します。
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// <see cref="IDisposable"/> オブジェクトを、指定した <see cref="IDisposableHolder.CompositeDisposable"/> に追加します。
        /// </summary>
        /// <typeparam name="T"><see cref="IDisposable"/>を継承した任意のクラス</typeparam>
        /// <param name="disposable">登録するインスタンス</param>
        /// <param name="obj">登録先のオブジェクト</param>
        /// <returns>登録するインスタンス</returns>
        public static T AddTo<T>(this T disposable, IDisposableHolder obj) where T : IDisposable
        {
            if (obj == null)
            {
                disposable.Dispose();
            }
            else
            {
                obj.CompositeDisposable.Add(disposable);
            }

            return disposable;
        }
    }
}
