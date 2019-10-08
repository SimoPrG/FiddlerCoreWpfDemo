using System;

namespace FiddlerCoreWpfDemo.Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
