using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Lotus.Data.MongoDb.Tests.Entities
{
    [Serializable]
    public class Product : IdentityEntity
    {
        public String ProductName { get; set; }
        public Decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
