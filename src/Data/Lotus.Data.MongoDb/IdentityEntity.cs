using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Lotus.Data.MongoDb
{
    [Serializable]
    public abstract class IdentityEntity
    {
        [BsonId]
        public Int64 MongoId { get; set; }

        protected IdentityEntity()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            this.MongoId = BitConverter.ToInt64(bytes, 0);
        }
    }
}
