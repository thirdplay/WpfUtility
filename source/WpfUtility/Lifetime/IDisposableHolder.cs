using System;
using System.Collections.Generic;

namespace WpfUtility.Lifetime
{
    /// <summary>
    /// 複数のIDisposableオブジェクトをまとめた機能を提供するインターフェイス。
    /// </summary>
    public interface IDisposableHolder : IDisposable
    {
        /// <summary>
        /// 複数のIDisposableオブジェクトをまとめたコレクションを取得します。
        /// </summary>
        ICollection<IDisposable> CompositeDisposable { get; }
    }
}