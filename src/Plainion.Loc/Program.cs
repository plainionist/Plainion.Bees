using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Plainion.Scripts.Loc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var source = Path.GetFullPath(".");

            if (args.Length > 0)
            {
                if (args[0].Contains("*"))
                {
                    var pattern = new Regex("^" + args[0].Replace("*", ".*") + "$");
                    DirStats.DirectoryExcludes.AddRange(
                        Directory.GetDirectories(source)
                            .Where(x => !pattern.IsMatch(Path.GetFileName(x)))
                        );
                }
                else
                {
                    source = args[0];
                }
            }

            var stats = DirStats.Create(null, source);

            stats.Process();

            var tree = stats.BuildTree();
            tree.CollectedStats().Print();

            var settings = new XmlWriterSettings();
            settings.Indent = true;

            var output = Path.Combine(source, "Loc.xml");
            var writer = XmlWriter.Create(output, settings);

            tree.ToXml().WriteTo(writer);
            writer.Close();

            Console.WriteLine($"Output written to: {output}");
        }
    }
}
