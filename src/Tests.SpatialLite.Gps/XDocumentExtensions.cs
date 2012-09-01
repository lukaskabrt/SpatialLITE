//Code source: http://blogs.msdn.com/b/ericwhite/archive/2009/01/28/equality-semantics-of-linq-to-xml-trees.aspx#comments
//From the blog post: Equality Semantics of LINQ to XML Trees by Eric White

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Tests.SpatialLite.Gps {
    /// <summary>
    /// Extends XDocument with correct DeepEquals function
    /// </summary>
    public static class XDocumentExtensions {
        public static string ToStringAlignAttributes(this XDocument document) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                document.WriteTo(xmlWriter);
            return stringBuilder.ToString();
        }

        private static class Xsi {
            public static XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            public static XName schemaLocation = xsi + "schemaLocation";
            public static XName noNamespaceSchemaLocation = xsi + "noNamespaceSchemaLocation";
        }

        public static XDocument Normalize(XDocument source, XmlSchemaSet schema) {
            bool havePSVI = false;
            // validate, throw errors, add PSVI information
            if (schema != null) {
                source.Validate(schema, null, true);
                havePSVI = true;
            }
            return new XDocument(
                    source.Declaration,
                    source.Nodes().Select(n => {
                        // Remove comments, processing instructions, and text nodes that are
                        // children of XDocument.  Only white space text nodes are allowed as
                        // children of a document, so we can remove all text nodes.
                        if (n is XComment || n is XProcessingInstruction || n is XText)
                            return null;
                        XElement e = n as XElement;
                        if (e != null)
                            return NormalizeElement(e, havePSVI);
                        return n;
                    }
                    )
            );
        }

        public static bool DeepEqualsWithNormalization(XDocument doc1, XDocument doc2,
                XmlSchemaSet schemaSet) {
            XDocument d1 = Normalize(doc1, schemaSet);
            XDocument d2 = Normalize(doc2, schemaSet);
            return XNode.DeepEquals(d1, d2);
        }

        private static IEnumerable<XAttribute> NormalizeAttributes(XElement element,
                bool havePSVI) {
            return element.Attributes()
                            .Where(a => !a.IsNamespaceDeclaration &&
                                    a.Name != Xsi.schemaLocation &&
                                    a.Name != Xsi.noNamespaceSchemaLocation)
                            .OrderBy(a => a.Name.NamespaceName)
                            .ThenBy(a => a.Name.LocalName)
                            .Select(
                                    a => {
                                        if (havePSVI) {
                                            var dt = a.GetSchemaInfo().SchemaType.TypeCode;
                                            switch (dt) {
                                                case XmlTypeCode.Boolean:
                                                    return new XAttribute(a.Name, (bool)a);
                                                case XmlTypeCode.DateTime:
                                                    return new XAttribute(a.Name, (DateTime)a);
                                                case XmlTypeCode.Decimal:
                                                    return new XAttribute(a.Name, (decimal)a);
                                                case XmlTypeCode.Double:
                                                    return new XAttribute(a.Name, (double)a);
                                                case XmlTypeCode.Float:
                                                    return new XAttribute(a.Name, (float)a);
                                                case XmlTypeCode.HexBinary:
                                                case XmlTypeCode.Language:
                                                    return new XAttribute(a.Name,
                                                            ((string)a).ToLower());
                                            }
                                        }
                                        return a;
                                    }
                            );
        }

        private static XNode NormalizeNode(XNode node, bool havePSVI) {
            // trim comments and processing instructions from normalized tree
            if (node is XComment || node is XProcessingInstruction)
                return null;
            XElement e = node as XElement;
            if (e != null)
                return NormalizeElement(e, havePSVI);
            // Only thing left is XCData and XText, so clone them
            return node;
        }

        private static XElement NormalizeElement(XElement element, bool havePSVI) {
            if (havePSVI) {
                var dt = element.GetSchemaInfo();
                switch (dt.SchemaType.TypeCode) {
                    case XmlTypeCode.Boolean:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                (bool)element);
                    case XmlTypeCode.DateTime:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                (DateTime)element);
                    case XmlTypeCode.Decimal:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                (decimal)element);
                    case XmlTypeCode.Double:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                (double)element);
                    case XmlTypeCode.Float:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                (float)element);
                    case XmlTypeCode.HexBinary:
                    case XmlTypeCode.Language:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                ((string)element).ToLower());
                    default:
                        return new XElement(element.Name,
                                NormalizeAttributes(element, havePSVI),
                                element.Nodes().Select(n => NormalizeNode(n, havePSVI))
                        );
                }
            } else {
                return new XElement(element.Name,
                        NormalizeAttributes(element, havePSVI),
                        element.Nodes().Select(n => NormalizeNode(n, havePSVI))
                );
            }
        }

    }

}
