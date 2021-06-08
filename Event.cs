using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpCC
{
    class Event
    {
        public DateTime dateTime { get; set; }
        public int id { get; set; }
        public int level { get; set; }
        public int priority { get; set; }
        public string name { get; set; }
        public Event(Rule rule)
        {
            dateTime = rule.GetDateTime();
            id = rule.GetID();
            level = rule.GetLevel();
            priority = rule.GetPriority();
            name = rule.GetName();
        }

    }
}
