using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class StringExtension
    {
        public static string ConvertFieldNameToParameterName(this string fieldName)
        {
            // try to convert the field name to a parameter name,
            // some options:
            // _mixin => mixin
            if (fieldName.Length > 1 && fieldName.StartsWith("_"))
                return fieldName.Remove(0, 1);
            // mMixin => mixin
            if (fieldName.Length > 1 && fieldName.StartsWith("m") && char.IsUpper(fieldName[1]))
                return fieldName.Remove(0, 2).Insert(0, fieldName[0].ToString().ToLower());
            // m_Mixin => mixin
            if(fieldName.Length > 2 && fieldName.StartsWith("m_") && char.IsUpper(fieldName[2]))
                return fieldName.Remove(0, 3).Insert(0, fieldName[0].ToString().ToLower());
            // default case: just add an additional _
            return $"_{fieldName}";
        }

        /// <summary>
        /// tries to convert a useful field name from a given type name.
        /// Leading "I"s will be removed, also trailing "Mixin"s.
        /// Example:
        /// INamedMixin => _namedMixin
        /// NamedMixin => _namedMixin
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string ConvertTypeNameToFieldName(this string typeName)
        {
            var fieldName = typeName;
            if (fieldName.StartsWith("I") && fieldName.Length > 1)
                fieldName = fieldName.Remove(0,1);
            if (fieldName.Length > 0)
            {
                var temp = fieldName;
                fieldName = temp.Remove(0, 1).Insert(0, fieldName[0].ToString().ToLower());
            }
            fieldName = $"_{fieldName}";
            return fieldName;
        }
    }
}
