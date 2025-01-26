using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChosungComboBox
{

    internal static class ChosungHelper
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
                if (keyword[i] >= 'ㄱ' && keyword[i] <= 'ㅎ') // 초성
                {
                    for (int j = 0; j < __chars1.Count; j++)
                    {
                        if (keyword[i] == __chars1[j])
                        {
                            pattern += string.Format("[{0}-{1}]", __chars2[j], (char)(__chars2[j + 1] - 1));
                        }
                    }
                }
                else if (keyword[i] >= '가')
                {
                    // 받침이 있는지 검사
                    int magic = ((keyword[i] - '가') % 588);

                    // 받침이 없을 때
                    if (magic == 0)
                    {
                        pattern += string.Format("[{0}-{1}]", keyword[i], (char)(keyword[i] + 27));
                    }

                    // 받침이 있을 때
                    else
                    {
                        magic = 27 - (magic % 28);
                        pattern += string.Format("[{0}-{1}]", keyword[i], (char)(keyword[i] + magic));
                    }
                }
                else if (keyword[i] >= 'A' && keyword[i] <= 'z')
                {
                    pattern += keyword[i];
                }
                else if (keyword[i] >= '0' && keyword[i] <= '9')
                {
                    pattern += keyword[i];
                }
                else if (keyword[i] == ' ')
                {
                    pattern += " ";
                }
                else
                {
                    pattern += keyword[i];
                }
            }
            return pattern;
        }

        internal static bool IsMatch(string input, string pattern)
        {
            try
            {
                return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase, new TimeSpan(0, 0, 0, 0, 99));
            }
            catch
            {
                return false;
            }
        }

    }

}
