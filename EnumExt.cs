using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mToolkitFrameworkExtensions
{
    public static class EnumExt
    {
        public static string[] GetEnumsString<T>()
        {
            return GetEnums<T>().Select(x => x.ToString()).ToArray();
        }

        public static T[] GetEnums<T>()
        {
            Array enumValues = Enum.GetValues(typeof(T));
            T[] values = new T[enumValues.Length];

            for (int i = 0; i < enumValues.Length; i++)
                values[i] = ((T) enumValues.GetValue(i));

            return values;
        }
    }
}
