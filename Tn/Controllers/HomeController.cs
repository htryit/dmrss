using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BLL;
using Model;

using MongoDB.Bson;
using Microsoft.Extensions.Options;

namespace Tn.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsRepository newsbll;
        private readonly List<NewsFrom> fromlist = null;

        private Hashtable hashFromlist = null;
        
        public HomeController(INewsRepository NewsRepos,
            IOptions<List<NewsFrom>> FromList)
        {
            newsbll = NewsRepos;
            if (FromList != null && FromList.Value != null)
            {
                fromlist = FromList.Value;

                hashFromlist = new Hashtable();
                foreach(var fl in fromlist)
                {
                    hashFromlist.Add(fl.Code, fl.Title);
                }
            }
            
           
        }

        public IActionResult Index(string code = "")
        {
            ViewData["Title"] = "科技资讯聚合平台";
            ViewData["SiteName"] = "科技资讯聚合平台";
            ViewData["FromList"] = fromlist;
            ViewData["HasFromList"] = hashFromlist;

            int page = 1;
            var pagestr = Request.Query["p"].ToString();
            if (!string.IsNullOrEmpty(pagestr))
            {
                page = int.Parse(pagestr);
            }

            ViewData["Code"] = code;

            var newslist = newsbll.GetNews(code, page, 50);
            ViewData["News"] = newslist;

            ViewData["Page"] = page;

            return View();
        }

        public IActionResult Info(string Id)
        {
            ViewData["SiteName"] = "科技资讯聚合平台";
            ViewData["FromList"] = fromlist;
            var item = newsbll.Get(new ObjectId(Id));
            if (item != null)
            {
                ViewData["Title"] = item.Title + " - " + ViewData["SiteName"];
                ViewData["Item"] = item;
                ViewData["CodeName"] = hashFromlist[item.Code];

                newsbll.UpdateCount(item.Id);

            } 

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public void SiteMap()
        {
            StringBuilder sbOut = new StringBuilder();
            sbOut.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            var newslist = newsbll.GetNews("", 1, 50);

            if(newslist != null && newslist.Count > 0)
            {
                foreach(var ni in newslist)
                {
                    sbOut.Append($"<url><loc>http://tn.htryit.com/info-{ni.Id}.html</loc></url>");
                }
            }

            sbOut.Append("</urlset>");
            Response.ContentType = "text/xml";

            var bt = Encoding.GetEncoding("utf-8").GetBytes(sbOut.ToString());

            Response.Body.WriteAsync(bt, 0, bt.Count());
        }
    }
}
