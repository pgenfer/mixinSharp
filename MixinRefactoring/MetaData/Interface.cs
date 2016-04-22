namespace MixinRefactoring
{
    /// <summary>
    /// representation of an interface
    /// that is implemented by a class.
    /// Currently, only the name of the interface
    /// is important to us
    /// </summary>
    public class Interface
    {
        private readonly NameMixin _name = new NameMixin();

        public Interface(string name)
        {
            Name = name;
        }

        public string Name
        {
            get { return _name.Name; }
            set { _name.Name = value; }
        }

        public override string ToString() => _name.ToString();
    }
}