using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Plainion.IronDoc.FSharp;

namespace Plainion.IronDoc
{
    class XmlDocTransformer
    {
        private readonly AssemblyLoader myLoader;
        private TextWriter myWriter;
        private XmlDocDocument myDocument;

        public XmlDocTransformer(AssemblyLoader loader)
        {
            //Debugger.Launch();

            myLoader = loader;
        }

        public void Transform(string assemblyFile, string outputFile)
        {
            var assembly = myLoader.Load(assemblyFile);

            var xmlDoc = Path.Combine(Path.GetDirectoryName(assemblyFile), Path.GetFileNameWithoutExtension(assemblyFile) + ".xml");

            using (var writer = new StreamWriter(outputFile))
            {
                Transform(assembly, XmlDocDocument.Load(xmlDoc), writer);
            }
        }

        public void Transform(Assembly assembly, XmlDocDocument xmlDoc, TextWriter writer)
        {
            myWriter = writer;
            myDocument = xmlDoc;

            myWriter.Write("# ");
            myWriter.WriteLine(myDocument.AssemblyName);

            foreach (var type in assembly.GetTypes().Where(t => t.IsPublic))
            {
                ProcessType(type);
            }
        }

        internal void Transform(Type type, XmlDocDocument xmlDoc, TextWriter writer)
        {
            myWriter = writer;
            myDocument = xmlDoc;

            ProcessType(type);
        }

        private void ProcessType(Type type)
        {
            myWriter.WriteLine();
            myWriter.Write("## ");
            myWriter.WriteLine(type.FullName);

            TryProcessMemberDoc(myDocument.GetXmlDocumentation(type), 2);

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

            ProcessMembers("Fields",
                type.GetFields(bindingFlags).Where(m => !m.IsPrivate),
                field => field.FieldType.FullName + " " + field.Name);

            ProcessMembers("Constructors",
                type.GetConstructors(bindingFlags).Where(m => !m.IsPrivate),
                ctor => type.Name + "(" + string.Join(", ", ctor.GetParameters().Select(p => p.ParameterType)) + ")");

            ProcessMembers("Properties",
                type.GetProperties(bindingFlags).Where(m => !m.GetMethod.IsPrivate),
                property => property.PropertyType.FullName + " " + property.Name);

            ProcessMembers("Events",
                type.GetEvents(bindingFlags).Where(m => !m.AddMethod.IsPrivate),
                evt => evt.EventHandlerType.FullName + " " + evt.Name);

            ProcessMembers("Methods",
                type.GetMethods(bindingFlags)
                    .Where(m => !m.IsSpecialName)
                    .Where(m => !m.IsPrivate),
                method => (method.ReturnType == typeof(void) ? "void" : method.ReturnType.FullName)
                    + " " + method.Name
                    + "(" + string.Join(", ", method.GetParameters().Select(p => p.ParameterType)) + ")");
        }

        private void ProcessMembers<T>(string headline, IEnumerable<T> allMembers, Func<T, string> GetMemberName) where T : MemberInfo
        {
            var members = allMembers.ToList();

            if (!members.Any())
            {
                return;
            }

            myWriter.WriteLine();
            myWriter.Write("### ");
            myWriter.WriteLine(headline);

            foreach (var member in members)
            {
                myWriter.WriteLine();

                myWriter.Write("#### ");
                myWriter.WriteLine(GetMemberName(member));

                myWriter.WriteLine();

                TryProcessMemberDoc(myDocument.GetXmlDocumentation(member), 4);
            }
        }

        private void TryProcessMemberDoc(XElement member, int level)
        {
            if (member == null)
            {
                return;
            }

            foreach (var summary in member.Elements("summary"))
            {
                myWriter.WriteLine( Utils.NormalizeSpace(summary.Value));

                foreach (var p in member.Elements("para"))
                {
                    myWriter.WriteLine(Utils.NormalizeSpace(p.Value));
                }
            }

            var headlineMarker = "#".PadLeft(level + 1, '#');

            if (member.Elements("remarks").Any())
            {
                myWriter.WriteLine();
                myWriter.WriteLine("> " + headlineMarker + " Remarks");

                foreach (var remarks in member.Elements("remarks"))
                {
                    myWriter.WriteLine();
                    myWriter.Write("> ");
                    myWriter.WriteLine(Utils.NormalizeSpace(remarks.Value));
                }
            }

            if (member.Elements("param").Any())
            {
                myWriter.WriteLine();
                myWriter.WriteLine("> " + headlineMarker + " Parameters");

                foreach (var param in member.Elements("param"))
                {
                    myWriter.WriteLine();

                    myWriter.Write("> **");

                    myWriter.Write(param.Attribute("name") == null ? string.Empty : param.Attribute("name").Value);

                    myWriter.Write(":**  ");
                    myWriter.Write(Utils.NormalizeSpace(param.Value));
                    myWriter.WriteLine();
                }
            }

            if (member.Elements("returns").Any())
            {
                myWriter.WriteLine();
                myWriter.WriteLine("> " + headlineMarker + " Return value");

                foreach (var r in member.Elements("returns"))
                {
                    myWriter.WriteLine();
                    myWriter.Write("> ");
                    myWriter.WriteLine(Utils.NormalizeSpace(r.Value));
                }
            }

            if (member.Elements("exception").Any())
            {
                myWriter.WriteLine();
                myWriter.WriteLine("> " + headlineMarker + " Exceptions");

                foreach (var ex in member.Elements("exception"))
                {
                    myWriter.WriteLine();

                    myWriter.Write("> **");

                    var cref = Utils.SubstringAfter(ex.Attribute("cref").Value, ':');
                    myWriter.Write(cref);

                    myWriter.Write(":**  ");
                    myWriter.Write(ex.Value);
                    myWriter.WriteLine();
                }
            }

            if (member.Elements("example").Any())
            {
                myWriter.WriteLine();
                myWriter.WriteLine("> " + headlineMarker + " Example");
                myWriter.WriteLine();
                myWriter.WriteLine(">");

                foreach (var param in member.Elements("example"))
                {
                    TryProcessC(param);
                }
            }

            TryProcessC(member.Element("c"));
            TryProcessCode(member.Element("code"));
            TryProcessInclude(member.Element("include"));
            TryProcessParameterRef(member.Element("paramref"));
            TryProcessPermission(member.Element("permission"));
            TryProcessSee(member.Element("see"));
            TryProcessSeeAlso(member.Element("seealso"));
        }

        private void TryProcessC(XElement e)
        {
            if (e == null) return;

            myWriter.WriteLine();
            myWriter.WriteLine("```");
            myWriter.WriteLine();
            myWriter.WriteLine(Utils.NormalizeSpace(e.Value));
            myWriter.WriteLine("```");
        }

        private void TryProcessCode(XElement e)
        {
            if (e == null) return;

            myWriter.WriteLine();
            myWriter.Write("`");
            myWriter.Write(e.Value);
            myWriter.WriteLine("`");
        }

        private void TryProcessInclude(XElement e)
        {
            if (e == null) return;

            myWriter.Write("[External file]({");
            myWriter.Write(e.Attribute("file").Value);
            myWriter.WriteLine("})");
        }

        private void TryProcessParameterRef(XElement e)
        {
            if (e == null) return;

            myWriter.WriteLine("*");
            myWriter.Write(e.Attribute("name").Value);
            myWriter.WriteLine("*");
        }

        private void TryProcessPermission(XElement e)
        {
            if (e == null) return;

            myWriter.WriteLine();

            myWriter.Write("**Permission:** *");
            myWriter.Write(e.Attribute("cref").Value);
            myWriter.WriteLine("*");

            myWriter.WriteLine(Utils.NormalizeSpace(e.Value));
        }

        private void TryProcessSee(XElement e)
        {
            if (e == null) return;

            myWriter.WriteLine();
            myWriter.Write("> *See: ");
            myWriter.Write(e.Attribute("cref").Value);
            myWriter.WriteLine("*");
        }

        private void TryProcessSeeAlso(XElement e)
        {
            if (e == null) return;

            myWriter.WriteLine();
            myWriter.WriteLine("> *See also: ");
            myWriter.WriteLine(e.Attribute("cref").Value);
        }
    }
}
