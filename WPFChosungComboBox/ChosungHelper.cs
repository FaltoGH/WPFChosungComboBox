using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFChosungComboBox
{

    public static class ChosungHelper
    {

        private static readonly IReadOnlyList<char> __chars1 = new char[19]{
'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ',
'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ',
'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };

        private static readonly IReadOnlyList<char> __chars2 = new char[20]{
'가', '까', '나', '다', '따',
'라', '마', '바', '빠', '사',
'싸', '아', '자', '짜', '차',
'카', '타', '파', '하', (char)('힣'+1)};


        internal static string GetPattern(string keyword)
        {
            string pattern = "";
            for (int i = 0; i < keyword.Length; i++)
            {
                char keychar = keyword[i];

                if (keychar >= 'ㄱ' && keychar <= 'ㅎ') // 초성
                {
                    for (int j = 0; j < __chars1.Count; j++)
                    {
                        if (keychar == __chars1[j])
                        {
                            pattern += string.Format("[{0}-{1}]", __chars2[j], (char)(__chars2[j + 1] - 1));
                        }
                    }
                }
                else if (keychar >= '가')
                {
                    // 받침이 있는지 검사
                    int magic = ((keychar - '가') % 588);

                    // 받침이 없을 때
                    if (magic == 0)
                    {
                        pattern += string.Format("[{0}-{1}]", keychar, (char)(keychar + 27));
                    }

                    // 받침이 있을 때
                    else
                    {
                        magic = 27 - (magic % 28);
                        pattern += string.Format("[{0}-{1}]", keychar, (char)(keychar + magic));
                    }
                }
                else if (char.IsLetterOrDigit(keychar))
                {
                    pattern += keychar;
                }
                else if (keychar == ' ')
                {
                    pattern += " ";
                }
                else if ((keychar == '(') || (keychar == ')') || (keychar == '\\') || (keychar == '[') || (keychar == ']'))
                {
                    pattern += ("\\" + keychar);
                }
                else
                {
                    pattern += keyword[i];
                }
            }
            return pattern;
        }


        private static readonly TimeSpan matchTimeout = new TimeSpan(0, 0, 0, 0, 99);


        internal static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase, matchTimeout);
        }


        internal static bool TryIsMatch(string input, string pattern)
        {
            bool ret;
            try
            {
                ret = IsMatch(input, pattern);
            }
            catch
            {
                ret = false;
            }
            return ret;
        }


        public static IReadOnlyDictionary<string, string> ReplaceDict = new Dictionary<string, string>()
        {
            ["ㄳ"] = "ㄱㅅ",
            ["ㄵ"] = "ㄴㅈ",
            ["ㄶ"] = "ㄴㅎ",
            ["ㄺ"] = "ㄹㄱ",
            ["ㄻ"] = "ㄹㅁ",
            ["ㄼ"] = "ㄹㅂ",
            ["ㄽ"] = "ㄹㅅ",
            ["ㄾ"] = "ㄹㅌ",
            ["ㄿ"] = "ㄹㅍ",
            ["ㅀ"] = "ㄹㅎ",
            ["ㅄ"] = "ㅂㅅ"
        };

        internal static void Separate(TextBox textBox)
        {
            string text = textBox.Text;
            string oldText = text;
            int selectionStart = textBox.SelectionStart;

            foreach (var kv in ChosungHelper.ReplaceDict)
            {
                text = text.Replace(kv.Key, kv.Value);
            }

            if (oldText == text)
            {

            }
            else
            {
                textBox.Text = text;
                textBox.SelectionStart = selectionStart + 2;
            }

        }

    }

}
