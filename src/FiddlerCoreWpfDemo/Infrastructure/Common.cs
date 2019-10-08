﻿using System.IO;
using System.Reflection;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    internal static class Common
    {
        public static readonly string AssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    }
}
