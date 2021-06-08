using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace CSharpCC { 

    public enum State { INACTIVE, PROGRESS, COMPLETE, TERMINATE }

    class Listener
    {
        private DateTime created;
        private DateTime endTime;
        private int trigerFreq;
        private int freq;
        private int offset;
        private List<int> ruleIds;
        private State state;
        private int level;
        public Listener(XElement listenerXML, List<Rule> rules, int level)
        {
            this.level = level;
            state = State.INACTIVE;
            offset = int.Parse(listenerXML.Attribute("time").Value);
            trigerFreq = int.Parse(listenerXML.Attribute("freq").Value);
            ruleIds = new List<int>();
            
            foreach (var idXML in listenerXML.Element("ids").Elements())
            {
                ruleIds.Add(int.Parse(idXML.Value));
            }

            update(rules);
            
        }

        public State update(List<Rule> rules)
        {
            var rulesCount = rules.Count;
            Rule[] rulesArr = new Rule[rulesCount];
            rules.CopyTo(rulesArr);
            List<Rule> rulesCopy = new List<Rule>(rulesArr);
            rulesCopy.Sort((r1, r2) => DateTime.Compare(r2.GetDateTime(), r1.GetDateTime()));
            foreach (var rule in rulesCopy)
            {
                if (rule.GetLevel() >= level) continue;
                var ruleTime = rule.GetDateTime();
                if (state == State.PROGRESS && ruleTime > endTime)
                {
                    state = State.TERMINATE;
                    return state;
                }
                if (ruleIds.Contains(rule.GetID()) && rule.GetState()==State.COMPLETE)
                {
                
                    if (state == State.INACTIVE)
                    {
                        freq = 0;
                        created = ruleTime;
                        endTime = created.AddSeconds(offset);
                        state = State.PROGRESS;
                    }

                    if (state == State.PROGRESS && ruleTime < endTime)
                    {
                        freq++;
                    }

                    if (freq >= trigerFreq)
                    {
                        state = State.COMPLETE;
                        return state;
                    }

                }
            }
            return state;
        }

        public State GetState() => state;
    }
}
