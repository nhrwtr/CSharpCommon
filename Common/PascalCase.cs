using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class PascalCase
    {
        public static string ToSnakeCase(string text)
        {
            if(text == null)
            {
                return text;
            }
            if (text.Length < 2)
            {
                return text.ToUpper();
            }

            var sb = new StringBuilder();
            sb.Append(char.ToUpperInvariant(text[0]));
            for (int i = 1; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(c);
                }
                else
                {
                    sb.Append(char.ToUpperInvariant(c));
                }
            }
            return sb.ToString();
        }
    }
}
