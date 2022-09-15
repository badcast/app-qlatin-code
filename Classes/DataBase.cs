using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode
{
    public static class DataBase
    {
        public enum AlphabetType
        {
            Undefined = 0,
            Cyrillic = 1,
            Latin = 2
        }

        public struct LatinDoubleChar
        {
            public char a { get; }
            public char b { get; }
            public bool IsUpperChar
            {
                get
                {
                    return char.IsUpper(a) && char.IsLower(b);
                }
            }

            public int SourceIndex { get; }
            public string Word { get { return a.ToString() + b.ToString(); } }
            public LatinDoubleChar(int sourceIndex, char a, char b)
            {
                this.SourceIndex = sourceIndex;
                this.a = a;
                this.b = b;
            }

            public static bool IsConfirm(LatinDoubleChar latin, char a, char b)
            {
                if (char.ToLower(latin.a) == char.ToLower(a) && char.ToLower(latin.b) == char.ToLower(b))
                {
                    return true;
                }
                return false;
            }

            public static string ModifiedToString(LatinDoubleChar latin, bool upper)
            {
                string res = latin.Word;

                if (upper)
                {
                    res = char.ToUpper(latin.a).ToString() + char.ToLower(latin.b).ToString();
                }
                else
                    res = res.ToLower();

                return res;
            }

            public static LatinDoubleChar ModifiedToDoubleChar(LatinDoubleChar source, bool upper)
            {
                string s = ModifiedToString(source, upper);
                return new DataBase.LatinDoubleChar(source.SourceIndex, s[0], s[1]);
            }

            public static readonly LatinDoubleChar Empty;
        }
        public const string LIBRARY_NAME = @"QL - Converter MMK - A.Askarova";

        private static List<string> _CyrillicUnicode = new List<string>()
        {
                "А",
                "Ә",
                "Б",
                "В",
                "Г",
                "Ғ",
                "Д",
                "Е",
                "Ё",
                "Ж",
                "З",
                "И",
                "Й",
                "К",
                "Қ",
                "Л",
                "М",
                "Н",
                "Ң",
                "О",
                "Ө",
                "П",
                "Р",
                "С",
                "Т",
                "У",
                "Ұ",
                "Ү",
                "Ф",
                "Х",
                "Һ",
                "Ц",
                "Ч",
                "Ш",
                "Ы",
                "І",
                "Э",
                "Ю",
                "Я"
        };
        private static List<string> _LatinUnicode = new List<string>()
        {
                "A",
                "Á",
                "B",
                "V",
                "G",
                "Ǵ",
                "D",
                "E",
                "Ё",
                "J",
                "Z",
                "I",
                "I",
                "K",
                "Q",
                "L",
                "M",
                "N",
                "Ń",
                "O",
                "Ó",
                "P",
                "R",
                "S",
                "T",
                "Ý",
                "U",
                "Ú",
                "F",
                "H",
                "Һ",
                "TS",//ц
                "CH",//ч
                "SH",//Ш
                "Y",//У
                "I",//і
                "E",//Э
                "IÝ",//Ю  Iý
                "IA"//Я
        };
        private static Dictionary<char, char> specialSymbol = new Dictionary<char, char>()
        {
            { 'и', 'ı' },
            { 'й', 'ı' }
        };
        private static List<char> _deleteChars = new List<char>() { 'щ', 'ь', 'ъ' };

        private static List<LatinDoubleChar> _doubleUpperCaseLatinCache;
        private static LatinDoubleChar lastCache;

        private static bool UndefineSymbol(char correct)
        {
            return char.IsSymbol(correct) || char.IsWhiteSpace(correct);
        }

        private static bool IsDeleteChar(char c)
        {
            return _deleteChars.Any((f) => char.ToLower(c) == f);
        }

        private static char ReplaceSpecialSymbol(char c)
        {
            char toReplce;
            if(specialSymbol.TryGetValue(c, out toReplce))
            {
                return toReplce;
            }
            return c;
        }

        private static bool IsLatinDoubleChar(char a, char b, out int index)
        {
            index = -1;
            ChunkDoubleCache();
            if (LatinDoubleChar.IsConfirm(lastCache, a, b))
            {
                index = lastCache.SourceIndex;
                return true;
            }

            for (int i = 0; i < _doubleUpperCaseLatinCache.Count; i++)
            {
                var f = _doubleUpperCaseLatinCache[i];
                if (LatinDoubleChar.IsConfirm(f, a, b))
                {
                    index = f.SourceIndex;
                    lastCache = f;
                    return true;
                }
            }

            return false;
        }
        private static LatinDoubleChar GetCacheFromIndex(int index)
        {
            for (int i = 0; i < _doubleUpperCaseLatinCache.Count; i++)
            {
                if (_doubleUpperCaseLatinCache[i].SourceIndex == index)
                {
                    return _doubleUpperCaseLatinCache[i];
                }

            }
            return LatinDoubleChar.Empty;
        }
        private static void ChunkDoubleCache()
        {
            if (_doubleUpperCaseLatinCache == null || _doubleUpperCaseLatinCache.Count == 0)
            {
                _doubleUpperCaseLatinCache = _doubleUpperCaseLatinCache ?? new List<LatinDoubleChar>();

                for (int i = _LatinUnicode.Count - 1; i > 0; i--)
                {
                    string res = _LatinUnicode[i];
                    if (res.Length > 1)
                    {
                        _doubleUpperCaseLatinCache.Add(new LatinDoubleChar(i, res[0], res[1]));
                    }

                }

            }
        }

        private static Tuple<AlphabetType, int> GetCharFromAt(char test)
        {
            AlphabetType type = AlphabetType.Undefined;
            int charIndex = -1;

            if (UndefineSymbol(test))
            {
                type = AlphabetType.Undefined;
            }
            else
            {
                test = char.ToUpper(test);
                for (int i = 0; i < _CyrillicUnicode.Count; i++)
                {
                    //Test Cyrillic
                    char getC = _CyrillicUnicode[i][0];
                    if (getC == test)
                    {
                        type = AlphabetType.Cyrillic;
                        charIndex = i;
                        break;
                    }
                    //Test Latin
                    char getL = _LatinUnicode[i][0];
                    if (getL == test)
                    {
                        type = AlphabetType.Latin;
                        charIndex = i;
                        break;
                    }
                }
            }

            return Tuple.Create(type, charIndex);
        }

        public static AlphabetType CharFrom(char test)
        {
            return GetCharFromAt(test).Item1;
        }

        /// <summary>
        /// Преобразует оригинальную строку в Латинский вид
        /// </summary>
        /// <param name="parseString"></param>
        /// <returns></returns>
        public static string TranslateToLatin(string cyrillicString)
        {
            if (string.IsNullOrEmpty(cyrillicString))
                return cyrillicString;
            string _translated = "";
            for (int i = 0; i < cyrillicString.Length; i++)
            {
                char c = ReplaceSpecialSymbol(cyrillicString[i]);
                if (IsDeleteChar(c))
                    continue;
                var type = GetCharFromAt(c);
                if (type.Item1 == AlphabetType.Cyrillic)
                {
                    string reChange = "";
                    int index = type.Item2;
                    reChange = _LatinUnicode[index];
                    if (char.IsLower(c))
                    {
                        reChange = reChange.ToLower();
                    }
                    _translated += reChange;

                    continue;
                }
                _translated += c;

            }

            return _translated;
        }
        public static string TranslateToCyrillic(string latinString)
        {
            if (string.IsNullOrEmpty(latinString))
                return latinString;

            string _translated = "";
            int length = latinString.Length;
            for (int i = 0; i < length; i++)
            {
                char c = latinString[i];
                if (IsDeleteChar(c))
                    continue;

                var from = GetCharFromAt(c);
                if (from.Item1 == AlphabetType.Latin)
                {
                    string to = _CyrillicUnicode[from.Item2];

                    if (i < length - 1)
                    {
                        char a = latinString[i];
                        char b = latinString[i + 1];
                        int index = -1;
                        if (IsLatinDoubleChar(a, b, out index))
                        {
                            var cache = GetCacheFromIndex(index);
                            to = _CyrillicUnicode[index];
                            i++;
                        }

                    }

                    if (char.IsLower(c))
                    {
                        to = to.ToLower();
                    }
                    else
                    {
                        to = to.ToUpper();
                    }

                    _translated += to;
                    continue;
                }

                _translated += c;
            }

            return _translated;
        }
    }
}
