using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Plainion.IronDoc
{
    internal class AssemblyLoader : IDisposable
    {
        private List<string> myAssemblyBaseDirs;

        public AssemblyLoader()
        {
            myAssemblyBaseDirs = new List<string>();
            myAssemblyBaseDirs.Add(AppDomain.CurrentDomain.BaseDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
        }

        private Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var loadedAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                    .FirstOrDefault(asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));

            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            var assemblyName = new AssemblyName(args.Name);
            var dependentAssemblyPath = GetAssemblyPath(assemblyName);

            if (!File.Exists(dependentAssemblyPath))
            {
                try
                {
                    // e.g. .NET assemblies, assemblies from GAC
                    return Assembly.ReflectionOnlyLoad(args.Name);
                }
                catch
                {
                    // ignore exception here - e.g. System.Windows.Interactivity - app will work without
                    Debug.WriteLine("Failed to load: " + assemblyName);
                    return null;
                }
            }

            return Load(dependentAssemblyPath);
        }

        private string GetAssemblyPath(AssemblyName assemblyName)
        {
            return myAssemblyBaseDirs
                .SelectMany(baseDir => new string[] {".dll", ".exe"}
                    .Select( ext => Path.Combine(baseDir, assemblyName.Name + ext)))
                .FirstOrDefault(File.Exists);
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        internal Assembly Load(string assemblyFile)
        {
            myAssemblyBaseDirs.Add(Path.GetDirectoryName(assemblyFile));

            byte[] assemblyBytes = File.ReadAllBytes(assemblyFile);
            return Assembly.ReflectionOnlyLoad(assemblyBytes);
        }
    }
}
