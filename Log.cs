using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CSharpCC
{
    class Log 
    {
        private DateTime date;
        private string type;
        private string body;

        public Log(string logLine)
        {
            var lineSplits = logLine.Split("   ");
            switch (lineSplits.Length)
            {
                case 1:
                    date = DateTime.Now;
                    type = "RAW";
                    body = logLine;
                    break;
                case 2:
                    try
                    {
                        date = defaultTimeParse(lineSplits[0]);
                        type = "RAW";
                        body = lineSplits[1];
                    } catch
                    {
                        date = DateTime.Now;
                        type = lineSplits[0];
                        body = lineSplits[1];
                    }
                    break;
                default:
                    try
                    {
                        date = defaultTimeParse(lineSplits[0]);
                        type = lineSplits[1];
                        for (var i = 2; i < lineSplits.Length; i++)
                        {
                            body += lineSplits[i] + "   ";
                        }
                        body = body.Trim();
                    }
                    catch
                    {
                        date = DateTime.Now;
                        type = lineSplits[0];
                        for (var i = 1; i < lineSplits.Length; i++ ){
                            body += lineSplits[i] + "   ";
                        }
                        body = body.Trim();
                    }
                    break;

            }
        }

        
        public string GetLogType() => type;

        public string GetBody() => body;

        public DateTime GetDateTime() => date;

        private static DateTime defaultTimeParse(string dateTimeLine) 
        {
            string trimLine = dateTimeLine.Trim().Substring(4, 20);
            DateTime result = DateTime.ParseExact(trimLine, "MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return result;
        }
    }
}
