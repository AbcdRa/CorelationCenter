using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CSharpCC
{
    class Rule
    {
        private int id;
        private int level;
        private int priority;
        private string name;
        private string[] types;
        private XElement conditions;
        private State state;
        public DateTime date;
        private Dictionary<string, Listener> listeners;


        public Rule(XElement ruleXML) {
            date = DateTime.Now;
            state = State.INACTIVE;
            id = int.Parse(ruleXML.Attribute("id").Value);
            priority = int.Parse(ruleXML.Attribute("priority").Value);
            level = int.Parse(ruleXML.Attribute("level").Value);
            name = ruleXML.Element("name").Value;
            if (level == 0) {
                types = ParseTypes(ruleXML.Element("types"));
            }
            conditions = ruleXML.Element("if");
            listeners = new Dictionary<string, Listener>();
        }

        private string[] ParseTypes(XElement typesXML) {
            List<string> types = new List<string>();
            foreach(var typeXML in typesXML.Elements())
            {
                types.Add(typeXML.Value);
            }
            return types.ToArray();
        }

        public bool Check(Log log, List<Rule> rules)
        {
            state = State.INACTIVE;
            bool result;
            if (level == 0)
            {
                result = ParseLog(log);
            }
            else
            {
                result = ParseRules(rules);
            }
            return result;
            
        }


        private bool ParseLog(Log log)
        {
            if(!CheckTypes(log.GetLogType())) {
                return false;
            }
            //По умолчанию выполняем операцию И
            foreach(var element in conditions.Elements())
            {
                if(!partParseLog(log, element)) return false;
            }
            state = State.COMPLETE;
            //date = log.GetDateTime();
            date = DateTime.Now;
            return true;
        }


        private bool partParseLog(Log log, XElement root)
        {
            string rootName = root.Name.LocalName;
            if(rootName.Equals("parse"))
            {
                if(root.Attribute("type").Value == "match")
                {
                    return new Regex(root.Value, RegexOptions.IgnoreCase).IsMatch(log.GetBody());
                }
            }
            else if(rootName.Equals("and"))
            {
                foreach(var node in root.Elements())
                {
                    if (!partParseLog(log, node)) return false;
                }
                return true;
            }
            else if(rootName.Equals("or")) {
                foreach (var node in root.Elements())
                {
                    if (partParseLog(log, node)) return true;
                }
                return false;
            }
            throw new Exception("Неправильный формат");
        }


        private bool CheckTypes(string type)
        {
            for(int i=0; i<types.Length; i++)
            {
                if(types[i].Equals(type))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ParseRules(List<Rule> rules)
        {
            //По умолчанию выполняем операцию И
            foreach (var element in conditions.Elements())
            {
                if (!partParseRules(rules, element)) return false;
            }
            state = State.COMPLETE;
            listeners.Clear();
            date = DateTime.Now;
            return true;
        } 

        private bool partParseRules(List<Rule> rules, XElement root)
        {
            string rootName = root.Name.LocalName;
            if (rootName.Equals("id"))
            {
                var rule = rules.Find((r) => r.GetID() == int.Parse(root.Value));
                return rule.state == State.COMPLETE;
            }
            else if (rootName.Equals("listen"))
            {
                var listenerId = root.Attribute("id").Value;
                State listenerState;
                if(listeners.ContainsKey(listenerId))
                {
                    var listener = listeners[listenerId];
                    listenerState = listener.update(rules);
                }
                else
                {
                    var listener = new Listener(root, rules, level);
                    listenerState = listener.GetState();
                    listeners[listenerId] = listener;
                }
                if (listenerState == State.COMPLETE) {
                    return true;
                }
                if (listenerState == State.TERMINATE) listeners.Remove(listenerId);

                return false;
            }
            else if (rootName.Equals("and"))
            {
                foreach (var node in root.Elements())
                {
                    if (!partParseRules(rules, node)) return false;
                }
                return true;
            }
            else if (rootName.Equals("or"))
            {
                foreach (var node in root.Elements())
                {
                    if (partParseRules(rules, node)) return true;
                }
                return false;
            }
            throw new Exception("Неправильный формат");
        }




        public DateTime GetDateTime() => date;

        public string GetName() => name;

        public int GetPriority() => priority;

        public State GetState() => state;

        public int GetID() => id;

        public int GetLevel() => level;
    }
}
