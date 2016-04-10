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
    }
}
