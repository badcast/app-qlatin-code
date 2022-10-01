//если у вашего ОС установлена Microsoft Office 2007 +, то данный метод будет работать иначе нужно затереть SUPPORT 
#define SUPPORT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SUPPORT
using Word = Microsoft.Office.Interop.Word;
#endif


namespace QCode
{
    public class Plugin
    {
        protected string _pluginName;
        protected double _version;
        public static bool isSupported
        {
            get =>
#if SUPPORT
                true;
#else
                false;
#endif
        }
        public string PluginName { get => _pluginName; }
        public double Version { get => _version; }
        public Plugin()
        {
        }
    }

#if SUPPORT
    public class WordPlugin : Plugin
    {
        public WordPlugin()
        {
            this._pluginName = "Word plugin extension";
            this._version = 1.0;
        }
        public void SaveTo(string fileNameOpen, string fileNameTo, DataBase.AlphabetType alphabetType, Word.WdSaveFormat saveFormat = Word.WdSaveFormat.wdFormatDocumentDefault)
        {
            //INIT COM Object
            Word.Application comWord = new Word.Application();
            comWord.Visible = false;
            
            object __F = fileNameOpen;
            var doc = comWord.Documents.Open(ref __F);
            Func<string, string> func = null;
            if (alphabetType == DataBase.AlphabetType.Latin)
            {
                func = DataBase.TranslateToLatin;
            }
            else if (alphabetType == DataBase.AlphabetType.Cyrillic)
            {
                func = DataBase.TranslateToCyrillic;
            }
              
            var rgs = doc.Content.Paragraphs;
            int count = rgs.Count;
            for (int i = 1; i <= count; i++)
            {
                Word.Paragraph r = rgs[i];

                string lastText = r.Range.Text;
                if(lastText == "\r\a")
                {
                    count++;

                    if (rgs.Count < count)
                        count--;

                    continue;
                }
                if (string.IsNullOrWhiteSpace(lastText) || lastText == Environment.NewLine)
                {
                    continue;
                }
                string frmT = r.Format.ToString();
                r.Range.Text = func(lastText);
            }


            doc.SaveAs(fileNameTo, FileFormat: saveFormat);
            doc.Close();

        }
    }
#endif
}
