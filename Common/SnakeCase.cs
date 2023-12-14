using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class SnakeCase
    {
        public static string ToPascalCase(string text)
        {
            if (text == null)
            {
                return text;
            }
            if (text.Length < 2)
            {
                return text;
            }

            var sb = new StringBuilder();
            bool isUpper = false;
            sb.Append(char.ToUpperInvariant(text[0]));
            for (int i = 1; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '_' && !isUpper)
                {
                    isUpper = true;
                }
                else
                {
                    sb.Append(isUpper ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
                    isUpper = c == '_';
                }
            }
            return sb.ToString();
        }
    }
}
