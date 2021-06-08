using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace CSharpCC
{
    class RulesCenter
    {
        private List<Rule> rules;
        public  RulesCenter(string filepath)
        {
            StreamReader reader = File.OpenText(filepath);
            XDocument doc = XDocument.Load(reader);
            rules = new List<Rule>();
            var rulesNode = doc.Root.Elements();
            foreach (var rule in rulesNode)
            {
                rules.Add(new Rule(rule));
            }
        }

        public List<Rule> compute(Log log)
        {
            List<Rule> activeRules = new List<Rule>();
            rules.Sort((r1, r2) => r1.GetLevel() - r2.GetLevel());
            for(int i= 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                if (rule.Check(log, rules))
                {
                    Console.WriteLine("Сработало правило " + rule.GetName());
                    activeRules.Add(rule);
                };
            }

            return activeRules;
        }

    }
}
