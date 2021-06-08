using System;
using System.Threading;
using StackExchange.Redis;
using Nest;
using System.Collections.Generic;

namespace CSharpCC
{
    class CCThread
    {
        private string qName;
        private IDatabase database;
        private IElasticClient elasticClient;
        private RulesCenter rulesCenter;

        public CCThread(string qName, IDatabase database, IElasticClient elasticClient)
        {
            this.qName = qName;
            this.database = database;
            this.elasticClient = elasticClient;
            string rulepath = ".\\rules\\XMLFile1.xml";
            rulesCenter = new RulesCenter(rulepath);
        }
        public void listenQ()
        {
            while (true)
            {
                var logLine = database.ListRightPop(qName);
                if(!logLine.HasValue)
                {
                    continue;
                }
                Log log = new Log(logLine);
                List<Rule> rules = rulesCenter.compute(log);
                foreach (var rule in rules)
                {
                    var ev = new Event(rule);
                    Console.WriteLine(rule.GetName() + " - отправлено в ElascticSearch");
                    elasticClient.IndexDocument<Event>(ev);
                }
            }
        }

    }
}
