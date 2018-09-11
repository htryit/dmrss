using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Options;


using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;

using Model;


namespace BLL
{
    public class DbContext
    {
        private readonly IMongoDatabase _db = null;

        public DbContext(IOptions<DbOptions> DbOptions)
        {
            var client = new MongoClient(DbOptions.Value.ConnectionString);
            if (client != null)
                _db = client.GetDatabase(DbOptions.Value.Database);
        }


        public IMongoCollection<News> News
        {
            get
            {
                return _db.GetCollection<News>("News");
            }
        }
    }

}
