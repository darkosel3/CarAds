
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Threading.Tasks;

namespace CarAds.Settings
{
    public class MongoDbConfig
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public string ConnectionString
        {
            get
            {
                string v = $"mongodb://{Host}:{Port}";
                return v;
            }
        }

    }
}
