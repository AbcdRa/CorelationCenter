using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Nest;
using StackExchange.Redis;

namespace CSharpCC {

    class Program
    {
        static void Main(string[] args)
        {
            //test();
            new CC(new string[] { "web_log_q" });
            Console.ReadLine();
        }

        static void test()
        {
            var logline1 = @"Mon Jun 07 2021 15:40:16 GMT + 0500(GMT + 05:00)   INFO   ::1   POST   /auth  registered:false";
            var logline2 = @"Mon Jun 07 2021 15:40:20 GMT + 0500(GMT + 05:00)   INFO   ::1   POST   /auth  registered:false";
            var logline3 = @"Mon Jun 07 2021 15:40:25 GMT + 0500(GMT + 05:00)   INFO   ::1   POST   /auth  registered:false";
            var logline4 = @"Mon Jun 07 2021 15:40:30 GMT + 0500(GMT + 05:00)   ERROR  ERROR";
            var logline5 = @"Mon Jun 07 2021 15:40:35 GMT + 0500(GMT + 05:00)   ERROR  ERROR";
            var logline6 = @"Mon Jun 07 2021 15:40:40 GMT + 0500(GMT + 05:00)   ERROR  ERROR";
            var log1 = new Log(logline1);
            var log2 = new Log(logline2);
            var log3 = new Log(logline3);
            var log4 = new Log(logline4);
            var log5 = new Log(logline5);
            var log6 = new Log(logline6);
            var filepath =  ".\\rules\\XMLFile1.xml";
            
            var center = new RulesCenter(filepath);
            var res1 = center.compute(log1);

            var res2 = center.compute(log2);

            var res3 = center.compute(log3);

            var res4 = center.compute(log4);

            var res5 = center.compute(log5);
            var res6 = center.compute(log6);
            Console.ReadLine();
        }
    }
}
