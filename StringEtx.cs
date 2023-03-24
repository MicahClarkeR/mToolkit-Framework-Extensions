using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mToolkitFrameworkExtensions
{
    public static class StringEtx
    {
        public static string GetGUID() => Guid.NewGuid().ToString();
    }
}
