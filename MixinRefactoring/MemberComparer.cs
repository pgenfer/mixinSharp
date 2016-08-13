using System;

namespace MixinRefactoring
{
    public class MemberComparer
    {
        public virtual bool IsSameAs(Member first, Member second)
        {
            return IsEqual((dynamic)first, (dynamic)second);
        }

        protected virtual bool IsEqual(Member first, Member second)
        {
            return false;
        }

        /// <summary>
        /// checks if the second property is an implementation
        /// of the first property.
        /// In case of properties, it is enough to check the name
        /// since two properties in the same class cannot have the same name
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        protected virtual bool IsEqual(Property first, Property second)
        {
            return first.Name == second.Name;
        }

        /// <summary>
        /// two events are equal if they have the same name.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        protected virtual bool IsEqual(Event first, Event second)
        {
            return first.Name == second.Name;
        }

        /// <summary>
        /// checks if one method has the same signature as
        /// the other method. This is the case if the name and
        /// the order and signature of the parameters is the same.
        /// The return type does not play a rule since
        /// two methods with the same name and parameters cannot
        /// have the same return type.
        /// </summary>
        /// <param name = "first"></param>
        /// <param name = "second"></param>
        /// <returns></returns>
        protected virtual bool IsEqual(Method first, Method second)
        {
            if (first.Name == second.Name)
            {
                if (first.ParameterCount == second.ParameterCount)
                {
                    for (var i = 0; i < first.ParameterCount; i++)
                    {
                        if (!first.GetParameter(i).IsEqual(second.GetParameter(i)))
                            return false;
                    }
                    return true; // all parameters are same => methods are equal
                }

                return false;
            }

            return false;
        }
    }
}