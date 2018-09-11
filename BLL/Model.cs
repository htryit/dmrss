using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Model
{
    public class News
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Code { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Context { get; set; }
        public DateTime PubTime { get; set; }
        public DateTime SpiderTime { get; set; }
        public int ViewCount { get; set; }
    }

    public class NewsFrom
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Site { get; set; }
        public string Url { get; set; }
    }
}
