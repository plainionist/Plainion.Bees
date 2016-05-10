using System;

namespace Plainion.IronDoc
{
    class Program
    {
        private static void Main(string[] args)
        {
            string apiDocFile = null;
            string outputFile = null;

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Equals("-h", StringComparison.OrdinalIgnoreCase))
                {
                    Usage();
                    Environment.Exit(0);
                }
                else if (args[i].Equals("-apidoc", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 == args.Length)
                    {
                        throw new ArgumentException("-apidoc requires an argument");
                    }

                    i++;
                    apiDocFile = args[i];
                }
                else if (args[i].Equals("-output", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 == args.Length)
                    {
                        throw new ArgumentException("-output requires an argument");
                    }

                    i++;
                    outputFile = args[i];
                }
            }

            using (var loader = new AssemblyLoader())
            {
                var transformer = new XmlDocTransformer(loader);
                transformer.Transform(apiDocFile, outputFile);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Plainion.IronDoc [Options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -h               - Prints this help");
            Console.WriteLine("  -apidoc <file>   - .Net XML API documentation file");
            Console.WriteLine("  -output <file>   - full path to output file");
        }
    }
}
