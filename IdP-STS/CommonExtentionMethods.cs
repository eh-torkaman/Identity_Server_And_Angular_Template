using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdP
{
    public static class CommonExtentionMethods
    {
        public static string TrimEvelNull(this  string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "" : input.Trim();
        }
    }
}
