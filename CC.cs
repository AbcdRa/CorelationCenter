using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Nest;
using StackExchange.Redis;

namespace CSharpCC
{
    class CC
    {
        public CC(String[] qNames, String redisAddress="localhost:6379", String elasticAddress="http://localhost:9200")
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisAddress);
            IDatabase db = redis.GetDatabase();
            Console.WriteLine("Redis запущен");
            var settings = new ConnectionSettings(new Uri(elasticAddress)).DefaultIndex("events");
            var client = new ElasticClient(settings);
            Console.WriteLine("ElasticSearch запущен");
            foreach (var qName in qNames)
            {
                CCThread ccThread = new CCThread(qName, db, client);
                Thread t = new Thread(new ThreadStart(ccThread.listenQ));
                t.Start();
            }
            Console.ReadLine();
        
        }
    }
}
