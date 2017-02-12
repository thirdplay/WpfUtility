using System;
using System.Collections.Generic;

namespace WpfUtility.Lifetime
{
    public interface IDisposableHolder : IDisposable
    {
        ICollection<IDisposable> CompositeDisposable { get; }
    }
}