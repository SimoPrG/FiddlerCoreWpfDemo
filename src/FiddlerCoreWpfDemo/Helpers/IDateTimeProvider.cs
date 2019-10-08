using System;

namespace FiddlerCoreWpfDemo.Helpers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
