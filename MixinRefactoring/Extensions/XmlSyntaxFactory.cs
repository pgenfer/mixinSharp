/// <summary>
/// as suggested here
/// https://github.com/dotnet/roslyn/issues/218
/// this class is used to improve the comment generation.
/// It seems like that this helper is not yet part of Roslyn,
/// and is also not available through a package,
/// so it is replicated here
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
            var startTag = XmlElementStartTag(tag)
                .WithLeadingTrivia(EndOfLine(Environment.NewLine), DocumentationCommentExterior("/// "))
                .WithAttributes(List<XmlAttributeSyntax>(attributeList));
                
            var endTag = XmlElementEndTag(tag);

            var contentNodes = new List<XmlNodeSyntax>();

            // first simplify all line breaks
            content = content.Replace(Environment.NewLine, "\n").Replace("\r", "\n");
           
            var buffer = new StringBuilder();
            // skip spaces at the beginning of the line
            var trimAtStart = true;
            // store the text per line 
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

            // TODO:
            // The rules:
            // use new lines in the same way as in the content nodes,
            // when a node starts with a new line, also add one and when it stops
            // with a new line, also add one

            // also always add a newline after an end tag (or before a start tag, let's see what works better)
            // A good method would be:
            // public static XmlElementSyntax MultiLineElement(XmlNameSyntax name, string content)
            // and the string in content is used to create the syntax nodes
            // so we have a state machine that creates nodes until it reaches a newline, then it will add the
            // new line node and will continue with the next node


            // add a new line after every content element
            //var contentWithNewLine = new List<XmlNodeSyntax>();
            //foreach (var node in content)
            //{
            //    contentWithNewLine.Add(NewLine());
            //    contentWithNewLine.Add(node);
            //}
            //return XmlElement(
            //    XmlElementStartTag(name),
            //    List(contentWithNewLine.ToArray()),
            //    //content.Insert(0, NewLine()).Add(NewLine()),
            //    XmlElementEndTag(name));
        }

        public static XmlElementSyntax Element(string localName, SyntaxList<XmlNodeSyntax> content)
        {
            return Element(XmlName(localName), content);
        }

        public static XmlElementSyntax Element(XmlNameSyntax name, SyntaxList<XmlNodeSyntax> content)
        {
            return XmlElement(
                XmlElementStartTag(name),
                content,
                XmlElementEndTag(name));
        }

        public static XmlEmptyElementSyntax EmptyElement(string localName)
        {
            return XmlEmptyElement(XmlName(localName));
        }

        public static SyntaxList<XmlNodeSyntax> List(params XmlNodeSyntax[] nodes)
        {
            return SyntaxFactory.List(nodes);
        }

        public static XmlTextSyntax Text(string value)
        {
            return Text(TextLiteral(value));
        }

        public static XmlTextSyntax Text(params SyntaxToken[] textTokens)
        {
            return XmlText(TokenList(textTokens));
        }

        public static XmlTextAttributeSyntax TextAttribute(string name, string value)
        {
            return TextAttribute(name, TextLiteral(value));
        }

        public static XmlTextAttributeSyntax TextAttribute(string name, params SyntaxToken[] textTokens)
        {
            return TextAttribute(XmlName(name), SyntaxKind.DoubleQuoteToken, TokenList(textTokens));
        }

        public static XmlTextAttributeSyntax TextAttribute(string name, SyntaxKind quoteKind, SyntaxTokenList textTokens)
        {
            return TextAttribute(XmlName(name), SyntaxKind.DoubleQuoteToken, textTokens);
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

        public static XmlTextSyntax NewLine()
        {
            return Text(TextNewLine());
        }

        public static SyntaxToken TextNewLine()
        {
            return TextNewLine(true);
        }

        public static SyntaxToken TextNewLine(bool continueComment)
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

        public static SyntaxToken TextLiteral(string value)
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