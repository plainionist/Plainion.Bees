using System;
using System.IO;
using NUnit.Framework;
using Plainion.IronDoc.Tests.Fakes;

namespace Plainion.IronDoc.Tests
{
    [TestFixture]
    class XmlDocToMarkdownTests
    {
        private XmlDocTransformer myTransformer;
        private XmlDocDocument myXmlDocumentation;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (var loader = new AssemblyLoader())
            {
                myTransformer = new XmlDocTransformer(loader);

                var assembly = GetType().Assembly;
                var docFile = Path.Combine(Path.GetDirectoryName(assembly.Location), Path.GetFileNameWithoutExtension(assembly.Location) + ".xml");

                myXmlDocumentation = XmlDocDocument.Load(docFile);
            }
        }

        [Test]
        public void SimpleSummary()
        {
            var markdownDocument = Transform(typeof(SimplePublicClass));

            Assert.That(markdownDocument, Does.Contain(@"
## Plainion.IronDoc.Tests.Fakes.SimplePublicClass
This is a summary
"));
        }

        private string Transform(Type type)
        {
            using (var writer = new StringWriter())
            {
                myTransformer.Transform(type, myXmlDocumentation, writer);
                return writer.ToString();
            }
        }
    }
}
