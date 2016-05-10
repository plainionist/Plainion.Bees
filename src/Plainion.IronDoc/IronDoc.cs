using System;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Plainion.IronDoc
{
    public class IronDoc : Task
    {
        [Required]
        public string Assembly { get; set; }

        [Output]
        public string MarkdownDoc { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "IronDoc generation started");

            try
            {
                using (var loader = new AssemblyLoader())
                {
                    var transformer = new XmlDocTransformer(loader);
                    transformer.Transform(Assembly, MarkdownDoc);

                    return true;
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var msg in ex.LoaderExceptions.Select(e => e.Message))
                {
                    Log.LogError(msg);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }

            return false;
        }
    }
}
