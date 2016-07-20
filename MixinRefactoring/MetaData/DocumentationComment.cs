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
        /// store the documentation as string
        /// used for easier validation during tests
        /// </summary>
        private string _documentationString;
        
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

            // comments can either have a "member" or a "doc" element enclosing them
            var contentElement = xmlComment.Elements("member").FirstOrDefault();
            if (contentElement == null)
                contentElement = xmlComment.Elements("doc").FirstOrDefault();
            if (contentElement == null)
                return;

            var xmlElements = contentElement
                .Descendants()
                .ToList();

            // create the string representation of the documentation
            var stringBuilder = new StringBuilder();
            // create a string for every new line in the comment
            // and add a "///" in front of the line
            var lines = xmlElements
                .SelectMany(x => x.ToString().Split(new[] { NewLine }, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim())
                .ToList();
            lines.ForEach(x => stringBuilder.AppendLine($"/// {x.ToString()}"));
            _documentationString = stringBuilder.ToString();

            _elements.AddRange(
                xmlElements
                .Select(x => new DocumentationElement(
                    x.Name.LocalName, 
                    x.Value,
                    x.Attributes().Select(a => new Attribute(a.Name.LocalName,a.Value)))));
        }

        public bool HasSummary => _elements.Any();
        public IEnumerable<DocumentationElement> Elements => _elements;
        public override string ToString() => _documentationString;

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
