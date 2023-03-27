using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IOTLib.IUISystem
{
    public interface ICustomCancelToken
    {
        CancellationToken CustomCancelToken { get; }
    }
}
