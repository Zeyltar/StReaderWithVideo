using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StReaderWithVideo
{
    class Subtitle
    {
        public static List<Subtitle> List = new List<Subtitle>();

        private bool isItalic_;
        public bool IsItalic
        {
            get { return isItalic_;  }
        }

        private TimeSpan start_;
        public TimeSpan Start
        {
            get { return start_; }
        }

        private TimeSpan end_;
        public TimeSpan End
        {
            get { return end_; }
        }

        private string text_;
        public string Text
        {
            get { return text_; }
        }

        private Subtitle(TimeSpan start, TimeSpan end, string text, bool isItalic = false)
        {
            start_ = start;
            end_ = end;
            text_ = text;
            isItalic_ = isItalic;
            List.Add(this);
        }

        public static void ReadSrtFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                if(Path.GetExtension(path) != ".srt")
                    throw new Exception("error with file extension: " + Path.GetExtension(path));
                string l = "";
                List<string> data = new List<string>();
                while ((l = sr.ReadLine()) != null)
                {
                    if (IsEmptyOrAllSpaces(l))
                    {
                        CreateSubtitleFromData(data);
                        data.Clear();
                    }
                    else
                        data.Add(l);
                }
                
            }
        }

        private static bool IsEmptyOrAllSpaces(string str)
        {
            return null != str && str.All(c => c.Equals(' '));
        }
        private static void CreateSubtitleFromData(List<string> data)
        {
            TimeSpan start = new TimeSpan(0, 0, 0);
            TimeSpan end = new TimeSpan(0, 0, 0);
            string text = "";
            int index;

            if (!int.TryParse(data[0], out index))
                throw new Exception("error on subtitle: " + data[0] + " is not an integer");

            if (data[1].Contains("-->"))
            {
                string[] sptS = data[1].Split(new string[] { " --> " }, StringSplitOptions.None);
                if (TimeSpan.TryParse(sptS[0], out TimeSpan result1) && TimeSpan.TryParse(sptS[1], out TimeSpan result2))
                {
                    start = result1;
                    end = result2;
                }
                else
                    throw new Exception("error on subtitle timing at " + index + " : " + data[1]);
            }

            for (int i = 2; i < data.Count; i++)
            {
                if (text.Length > 0)
                    text += '\n';
                text += data[i];
            }

            bool italic;
            if (italic = IsItalicText(text))
                text = text.Replace("<i>", "").Replace("</i>", "");

            new Subtitle(start, end, text, italic);
        }

        private static bool IsItalicText(string text)
        {
            return text.Contains("<i>");
        }
    }
}
