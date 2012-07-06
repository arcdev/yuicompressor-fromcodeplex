using System;
using System.Reflection;

namespace Yahoo.Yui.Compressor
{
    internal static class AssemblyResolver
    {
        private static bool initialised;
        private static readonly object syncLock = new Object();

        public static void Initialise()
        {
            if (initialised)
            {
                return;
            }
            lock (syncLock)
            {
                if (initialised)
                {
                    return;
                }
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    var resourceName = "Yahoo.Yui.Compressor.Resources." + new AssemblyName(args.Name).Name + ".dll";
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            var assemblyData = new Byte[stream.Length];
                            stream.Read(assemblyData, 0, assemblyData.Length);
                            initialised = true;
                            return Assembly.Load(assemblyData);
                        }
                        throw new DllNotFoundException("Cannot find embedded resource assembly named " + resourceName);
                    }
                };
            }
        }
    }
}
