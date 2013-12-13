using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource4Net
{
    public class StringSplitter
    {
        public static string[] SplitIntoLines(string text)
        {
            List<string> lines = new List<string>();

            //bool endFound = false;
            //bool searchingForFirstChar = true;
            int lineLength = 0;
            char previous = char.MinValue;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n' || c == '\r')
                {
                    bool isCRLFPair = previous=='\r' && c == '\n';

                    if (!isCRLFPair)
                    {
                        string line = text.Substring(i - lineLength, lineLength);
                        lines.Add(line);
                    }

                    lineLength = 0;
                }
                else
                {
                    lineLength++;
                }
                previous = c;
            }

            return lines.ToArray();
        }
    }
}
