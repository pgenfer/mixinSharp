
using Microsoft.CodeAnalysis.CSharp;

namespace MixinRefactoring
{
    /// <summary>
    /// stores all kind of names/identifiers during the processing.
    /// Since identifiers can always be reserved keywords but are not prefixed
    /// when retrieved during parsing, this mixin automatically adds a "@" in front
    /// of all names that equal a reserved keyword.
    /// </summary>
    public class NameMixin
    {
        private string _name;

        private static string PrefixKeywords(string name)
        {
            var isAnyKeyword = SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None;
            if (isAnyKeyword)
                name = $"@{name}";
            return name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = PrefixKeywords(value); }
        }

        public override string ToString() => Name;
    }
}
