
namespace MixinRefactoring
{
    public class NameMixin
    {
        public string Name { get; set; }
        public override string ToString() => Name;
    }
}
