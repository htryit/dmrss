using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using Model;

namespace BLL
{
    public interface INewsRepository
    {
        List<News> GetNews(string Code, int Page, int Pagesize);
        bool Exist(News Info);
        void Save(News Info);
        News Get(ObjectId Id);
        void UpdateCount(ObjectId Id);
    }
}
