using System.Xml;

namespace DidacticalEnigma.Core.Models.Formatting
{
    public class XmlRichFormattingRenderer
    {
        public XmlDocument Render(RichFormatting document)
        {
            var xmlDocument = new XmlDocument();
            var root = xmlDocument.CreateElement("root");
            foreach (var paragraph in document.Paragraphs)
            {
                switch (paragraph)
                {
                    case LinkParagraph link:
                    {
                        var linkElement = xmlDocument.CreateElement("link");
                        linkElement.SetAttribute("target", link.Target.ToString());
                        linkElement.SetAttribute("text", link.DisplayText);

                        root.AppendChild(linkElement);
                    }
                        break;
                    case TextParagraph text:
                    {
                        var paragraphElement = xmlDocument.CreateElement("par");
                        foreach (var c in text.Content)
                        {
                            var spanElement = xmlDocument.CreateElement("span");
                            spanElement.InnerText = c.Content;
                            spanElement.SetAttribute("fontSize", c.FontSize.ToString());
                            spanElement.SetAttribute("fontName", c.FontName);
                            if (c.Emphasis)
                            {
                                spanElement.SetAttribute("emphasis", "true");
                            }

                            paragraphElement.AppendChild(spanElement);
                        }
                        root.AppendChild(paragraphElement);
                    }
                        break;
                }
            }

            xmlDocument.AppendChild(root);
            return xmlDocument;
        }
    }
}