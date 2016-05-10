using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Plainion.IronDoc
{
    public class XmlDocDocument
    {
        private XmlDocDocument(IReadOnlyList<XElement> members)
        {
            Members = members;
        }

        public string AssemblyName { get; private set; }

        public IReadOnlyList<XElement> Members { get; private set; }

        public static XmlDocDocument Load(string file)
        {
            return Load(XElement.Load(file));
        }

        public static XmlDocDocument Load(XElement root)
        {
            return new XmlDocDocument(root.Element("members").Elements("member").ToList())
            {
                AssemblyName = root.Element("assembly").Element("name").Value
            };
        }

        public XElement GetXmlDocumentation(MemberInfo member)
        {
            var memberName = GetMemberElementName(member);
            return Members.SingleOrDefault(m => m.Attribute("name").Value == memberName);
        }

        /// <summary>
        /// Returns the expected name for a member element in the XML documentation file.
        /// </summary>
        /// <param name="member">The reflected member.</param>
        /// <returns>The name of the member element.</returns>
        private string GetMemberElementName(MemberInfo member)
        {
            char prefixCode;
            string memberName = (member is Type)
                                    ? ((Type)member).FullName // member is a Type
                                    : (member.DeclaringType.FullName + "." + member.Name); // member belongs to a Type

            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    // XML documentation uses slightly different constructor names
                    memberName = memberName.Replace(".ctor", "#ctor");
                    goto case MemberTypes.Method;
                case MemberTypes.Method:
                    prefixCode = 'M';

                    // parameters are listed according to their type, not their name
                    string paramTypesList = String.Join(
                        ",",
                        ((MethodBase)member).GetParameters()
                            .Cast<ParameterInfo>()
                            .Select(x => x.ParameterType.FullName
                            ).ToArray()
                        );
                    if (!String.IsNullOrEmpty(paramTypesList)) memberName += "(" + paramTypesList + ")";
                    break;

                case MemberTypes.Event:
                    prefixCode = 'E';
                    break;

                case MemberTypes.Field:
                    prefixCode = 'F';
                    break;

                case MemberTypes.NestedType:
                    // XML documentation uses slightly different nested type names
                    memberName = memberName.Replace('+', '.');
                    goto case MemberTypes.TypeInfo;
                case MemberTypes.TypeInfo:
                    prefixCode = 'T';
                    break;

                case MemberTypes.Property:
                    prefixCode = 'P';
                    break;

                default:
                    throw new ArgumentException("Unknown member type", "member");
            }

            // elements are of the form "M:Namespace.Class.Method"
            return String.Format("{0}:{1}", prefixCode, memberName);
        }
    }
}
