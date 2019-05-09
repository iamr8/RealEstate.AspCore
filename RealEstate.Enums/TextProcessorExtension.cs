using System.Text;
using System.Text.RegularExpressions;

namespace RealEstate.Base
{
    public static class TextProcessorExtension
    {
        private static string ApplyModeratePersianRules(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.ContainsFarsi())
                return text;

            return text
                .ApplyPersianYeKe()
                .ApplyHalfSpaceRule()
                .YeHeHalfSpace()
                .CleanupZwnj()
                .CleanupExtraMarks();
        }

        private static string ConvertDigitsToLatin(this string s)
        {
            var sb = new StringBuilder();
            foreach (var t in s)
            {
                switch (t)
                {
                    //Persian digits
                    case '\u06f0':
                        sb.Append('0');
                        break;

                    case '\u06f1':
                        sb.Append('1');
                        break;

                    case '\u06f2':
                        sb.Append('2');
                        break;

                    case '\u06f3':
                        sb.Append('3');
                        break;

                    case '\u06f4':
                        sb.Append('4');
                        break;

                    case '\u06f5':
                        sb.Append('5');
                        break;

                    case '\u06f6':
                        sb.Append('6');
                        break;

                    case '\u06f7':
                        sb.Append('7');
                        break;

                    case '\u06f8':
                        sb.Append('8');
                        break;

                    case '\u06f9':
                        sb.Append('9');
                        break;

                    //Arabic digits
                    case '\u0660':
                        sb.Append('0');
                        break;

                    case '\u0661':
                        sb.Append('1');
                        break;

                    case '\u0662':
                        sb.Append('2');
                        break;

                    case '\u0663':
                        sb.Append('3');
                        break;

                    case '\u0664':
                        sb.Append('4');
                        break;

                    case '\u0665':
                        sb.Append('5');
                        break;

                    case '\u0666':
                        sb.Append('6');
                        break;

                    case '\u0667':
                        sb.Append('7');
                        break;

                    case '\u0668':
                        sb.Append('8');
                        break;

                    case '\u0669':
                        sb.Append('9');
                        break;

                    default:
                        sb.Append(t);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Fixes common writing mistakes caused by using a bad keyboard layout,
        ///     such as replacing Arabic Ye with Persian one and so on ...
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string ApplyPersianYeKe(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? string.Empty
                : text.Replace(ArabicYeChar, PersianYeChar).Replace(ArabicKeChar, PersianKeChar).Trim();
        }

        private const char ArabicKeChar = (char)1603;
        private const char ArabicYeChar = (char)1610;
        private const char PersianKeChar = (char)1705;
        private const char PersianYeChar = (char)1740;

        /// <summary>
        ///     Adds zwnj char between word and prefix/suffix
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string ApplyHalfSpaceRule(this string text)
        {
            //put zwnj between word and prefix (mi* nemi*)
            string phase1 = Regex.Replace(text, @"\s+(ن?می)\s+", " $1‌");

            //put zwnj between word and suffix (*tar *tarin *ha *haye)
            return Regex.Replace(phase1, @"\s+(تر(ی(ن)?)?|ها(ی)?)\s+", "‌$1 ");
        }

        public static string FixPersian(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.ContainsFarsi())
                return text;

            return text
                .ApplyModeratePersianRules()
                .ConvertDigitsToLatin()
                .Replace((char)1610, (char)1740)
                .Replace((char)1603, (char)1705)
                .Trim();
        }

        /// <summary>
        ///     Replaces more than one ! or ? mark with just one
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string CleanupExtraMarks(this string text)
        {
            string phase1 = Regex.Replace(text, "(!){2,}", "$1");
            return Regex.Replace(phase1, "(؟){2,}", "$1");
        }

        /// <summary>
        ///     Removes unnecessary zwnj char that are succeeded/preceded by a space
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string CleanupZwnj(this string text)
        {
            return Regex.Replace(text, @"\s+‌|‌\s+", " ");
        }

        /// <summary>
        ///     Does text contain Persian characters?
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>true/false</returns>
        private static bool ContainsFarsi(this string text)
        {
            return Regex.IsMatch(text, @"[\u0600-\u06FF]");
        }

        /// <summary>
        ///     Converts ه ی to ه‌ی
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string YeHeHalfSpace(this string text)
        {
            return Regex.Replace(text, @"(\S)(ه[\s‌]+[یي])(\s)", "$1ه‌ی‌$3"); // fix zwnj
        }
    }
}