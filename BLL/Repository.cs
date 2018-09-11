using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

using Model;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;

namespace BLL
{
    public class NewsRepository : INewsRepository
    {
        private DbContext _db = null;

        public NewsRepository(IOptions<DbOptions> DbOptions)
        {
            _db = new DbContext(DbOptions);
        }
        

        public List<News> GetNews(string Code, int Page, int Pagesize)
        {
            FilterDefinition<News> filter = Builders<News>.Filter.Empty;
            if (!string.IsNullOrEmpty(Code))
            {
                filter = Builders<News>.Filter.Eq("Code", Code);
            }

            var sort = Builders<News>.Sort.Descending("PubTime");
     
            return _db.News.Find(filter).Sort(sort).Limit(Pagesize).Skip((Page - 1) * Pagesize).ToList();
        }

        public void Save(News Info)
        {
            _db.News.InsertOne(Info);
        }

        public bool Exist(News Info)
        {
            var filter = Builders<News>.Filter.Where(p => p.Code == Info.Code && p.Link == Info.Link);
            var chk = _db.News.Find(filter).FirstOrDefault();
            return chk != null;
        }

        public News Get(ObjectId Id)
        {
            var filter = Builders<News>.Filter.Where(p => p.Id == Id);

            return _db.News.Find(filter).FirstOrDefault();
        }

        public void UpdateCount(ObjectId Id)
        {
            var filter = Builders<News>.Filter.Where(p => p.Id == Id);
            var update = Builders<News>.Update.Inc("ViewCount", 1);
            _db.News.FindOneAndUpdate(filter, update);
        }
    }
}
