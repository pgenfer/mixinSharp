namespace MixinRefactoring
{
    public interface IParameterList
    {
        void Add(Parameter parameter);
        int ParameterCount
        {
            get;
        }

        Parameter GetParameter(int index);
    }
}