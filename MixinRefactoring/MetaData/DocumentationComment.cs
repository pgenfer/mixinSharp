using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Environment;

namespace MixinRefactoring
{
    /// <summary>
    /// stores the document comment of a class member.
    /// </summary>
    public class DocumentationComment
    {
        /// <summary>
        /// stores the documentation, every xml node
        /// is stored in one element.
        /// </summary>
        private List<DocumentationElement> _elements = 
            new List<DocumentationElement>();

        public DocumentationComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return;
            var xmlComment = XDocument.Parse(comment);
            _elements.AddRange(
                xmlComment
                .Element("member")
                .Descendants()
                .Select(x => new DocumentationElement(
                    x.Name.LocalName, 
                    x.Value,
                    x.Attributes().Select(a => new Attribute(a.Name.LocalName,a.Value)))));
        }

        public bool HasSummary => _elements.Any();
        public IEnumerable<DocumentationElement> Elements => _elements;

        /// <summary>
        /// stores an xml document element.
        /// </summary>
        public class DocumentationElement
        {
            private readonly List<Attribute> _attributes = new List<Attribute>();

            public DocumentationElement(string tag,string content, IEnumerable<Attribute> attributes)
            {
                Tag = tag;
                Content = content;
                _attributes.AddRange(attributes);
            }
            public string Tag { get; }
            public string Content { get; }
            public override string ToString() => $"<{Tag}> {Content} </{Tag}>";
            public IEnumerable<Tuple<string, string>> Attributes => _attributes.Select(x => Tuple.Create(x.Name, x.Value));      
        }

        /// <summary>
        /// represents an attribute in an xml comment
        /// </summary>
        public class Attribute
        {
            public Attribute(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; }
            public string Value { get; }
            public override string ToString() => $"{Name}=\"{Value}\"";
        }
    }
}
