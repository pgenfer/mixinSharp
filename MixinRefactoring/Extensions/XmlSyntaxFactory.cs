/// <summary>
/// as suggested here
/// https://github.com/dotnet/roslyn/issues/218
/// this class is used to improve the comment generation.
/// It seems like that this helper is not yet part of Roslyn,
/// and is also not available through a package,
/// so it is replicated here.
/// Methods which were not needed were removed and the
/// MultiLineElement method was modified to support multi line
/// document comments (with additional attributes)
/// </summary>
namespace MixinSharp
{
    using System;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using System.Text;
    using System.Linq;
    internal static class XmlSyntaxFactory
    {
        public static DocumentationCommentTriviaSyntax DocumentationComment(params XmlNodeSyntax[] content)
        {
            return DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, List(content))
                .WithTrailingTrivia(EndOfLine(Environment.NewLine))
                .WithLeadingTrivia(DocumentationCommentExterior("/// "));                
        }

        public static XmlElementSyntax MultiLineElement(
            string localName,
            string content,
            IEnumerable<Tuple<string,string>> attributes)
        {
            var attributeList = attributes.Select(x => TextAttribute(x.Item1, x.Item2)).OfType<XmlAttributeSyntax>();

            var tag = XmlName(localName);
            var startTag = XmlElementStartTag(tag) // begin every new start tag on a new line
                .WithLeadingTrivia(EndOfLine(Environment.NewLine), DocumentationCommentExterior("/// "))
                .WithAttributes(List<XmlAttributeSyntax>(attributeList)); // add attributes to start tag                
            var endTag = XmlElementEndTag(tag);

            // this list stores the documentation content within this tag,
            // one node element for every line
            var contentNodes = new List<XmlNodeSyntax>();

            // first simplify all line breaks
            content = content.Replace(Environment.NewLine, "\n").Replace("\r", "\n");
           
            var buffer = new StringBuilder();
            // skip spaces at the beginning of the line
            var trimAtStart = true;
            // simple state machine, reads every character
            // trims the spaces at the beginning and stores every line
            // in a single node
            foreach (var c in content)
            {
                switch(c)
                {
                    case '\n':
                        contentNodes.Add(Text(buffer.ToString()));
                        buffer.Clear();
                        contentNodes.Add(NewLine());
                        trimAtStart = true;
                        break;
                    case ' ':
                        if (!trimAtStart)
                            buffer.Append(c);
                        break;
                    default:
                        buffer.Append(c);
                        trimAtStart = false;
                        break;                    
                }
            }

            // if there was no linebreak (or something was left after the last line break)
            // add it to the content            
            if (buffer.Length > 0)
                contentNodes.Add(Text(buffer.ToString()));

            return XmlElement(startTag, List(contentNodes.ToArray()), endTag);        
        }
    
        public static SyntaxList<XmlNodeSyntax> List(params XmlNodeSyntax[] nodes)
        {
            return SyntaxFactory.List(nodes);
        }

        private static XmlTextSyntax Text(string value)
        {
            return Text(TextLiteral(value));
        }

        private static XmlTextSyntax Text(params SyntaxToken[] textTokens)
        {
            return XmlText(TokenList(textTokens));
        }

        private static XmlTextAttributeSyntax TextAttribute(string name, string value)
        {
            return TextAttribute(name, TextLiteral(value));
        }

        private static XmlTextAttributeSyntax TextAttribute(string name, params SyntaxToken[] textTokens)
        {
            return TextAttribute(XmlName(name), SyntaxKind.DoubleQuoteToken, TokenList(textTokens));
        }

        public static XmlTextAttributeSyntax TextAttribute(XmlNameSyntax name, SyntaxKind quoteKind, SyntaxTokenList textTokens)
        {
            return XmlTextAttribute(
                name,
                Token(quoteKind),
                textTokens,
                Token(quoteKind))
                .WithLeadingTrivia(Whitespace(" "));
        }

        public static XmlCrefAttributeSyntax CrefAttribute(CrefSyntax cref)
        {
            return CrefAttribute(cref, SyntaxKind.DoubleQuoteToken);
        }

        public static XmlCrefAttributeSyntax CrefAttribute(CrefSyntax cref, SyntaxKind quoteKind)
        {
            return XmlCrefAttribute(
                XmlName("cref"),
                Token(quoteKind),
                cref,
                Token(quoteKind))
                .WithLeadingTrivia(Whitespace(" "));
        }

        private static XmlTextSyntax NewLine()
        {
            return Text(TextNewLine());
        }

        private static SyntaxToken TextNewLine()
        {
            return TextNewLine(true);
        }

        private static SyntaxToken TextNewLine(bool continueComment)
        {
            SyntaxToken token = XmlTextNewLine(
                TriviaList(),
                Environment.NewLine,
                Environment.NewLine,
                TriviaList());

            if (continueComment)
                token = token.WithTrailingTrivia(DocumentationCommentExterior("/// "));

            return token;
        }

        private static SyntaxToken TextLiteral(string value)
        {
            string encoded = new XText(value).ToString();
            return XmlTextLiteral(
                TriviaList(),
                encoded,
                value,
                TriviaList());
        }
    }
}