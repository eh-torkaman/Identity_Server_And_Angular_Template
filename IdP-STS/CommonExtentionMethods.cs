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


        public static string ReplaceAllNonAlphaNumericExceptAllowableListOFCharacters(this string input, string allowableListOFCharacters ="", Boolean toUpper = false)
        {
            char[] arr = input.ToCharArray();
            arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)) || (allowableListOFCharacters.Contains(c))));
            input = new string(arr);
            if (toUpper)
                input = input.ToUpper();
            return input;
        }
    }
}
