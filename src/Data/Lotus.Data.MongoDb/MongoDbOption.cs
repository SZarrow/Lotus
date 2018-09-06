using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.Data.MongoDb
{
    [Serializable]
    public class MongoDbOption
    {
        public MongoDbOption() { }
        public MongoDbOption(String connectionString, String dbName)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (String.IsNullOrWhiteSpace(dbName))
            {
                throw new ArgumentNullException(nameof(dbName));
            }

            this.ConnectionString = connectionString;
            this.DbName = dbName;
        }

        public String ConnectionString { get; set; }
        public String DbName { get; set; }
    }
}
