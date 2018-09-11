using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Http;
using System.Net;
using System.Xml;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

using Model;
using BLL;
using System.Threading;

namespace Spider
{
    public class RssSpider
    {
        List<NewsFrom> SrcList;
        
        INewsRepository NewsBll;


        public RssSpider()
        {

            dynamic type = (new Program()).GetType();
            string basedir = Path.GetDirectoryName(type.Assembly.Location);

            var builder = new ConfigurationBuilder()
           //     .SetBasePath(Directory.GetCurrentDirectory())
                   .SetBasePath(basedir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();
            var services = new ServiceCollection().AddOptions();
            services.Configure<DbOptions>(opts =>
            {
                opts.ConnectionString = config.GetSection("DbConn:ConnectionString").Value;
                opts.Database = config.GetSection("DbConn:Database").Value;
            });
            services.Configure<List<NewsFrom>>(config.GetSection("NewsFrom"));

            var serviceProvider = services.BuildServiceProvider();

            NewsBll = new NewsRepository(serviceProvider.GetService<IOptions<DbOptions>>());
            SrcList = serviceProvider.GetService<IOptions<List<NewsFrom>>>().Value;
        }

        public void Start()
        {
            while(true)
            {
                Spider();
                Thread.Sleep(10 * 60 * 1000);
            }
            
        }

        private void Spider()
        {
            if (SrcList != null)
            {
                foreach (var src in SrcList)
                {
                    var xml = GetXml(src.Url);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        try
                        {
                            var news = ParseRssXml(xml);
                            if (news != null)
                            {
                                foreach (var n in news)
                                {
                                    n.Code = src.Code;
                                    if (NewsBll.Exist(n)) //存在则表示已经获取过了，不再插入
                                    {
                                        break;
                                    }

                                    NewsBll.Save(n);
                                }
                            }
                        }catch
                        {

                        }
                    }
                }
            }
        }

        private List<News> ParseRssXml(string Xml)
        {
            List<News> list = new List<News>();
            using (StringReader sr = new StringReader(Xml))
            {
                using (var reader = XmlReader.Create(sr))
                {
                    News item = null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            var itemname = reader.Name;

                            if (itemname == "item")
                            {
                                if (item != null)
                                {
                                    list.Add(item);
                                }

                                item = new News();
                                item.SpiderTime = DateTime.Now;
                                item.ViewCount = 0;
                            }
                            else
                            {
                                if (item != null)
                                {
                                    if (reader.Read())
                                    {
                                        var val = reader.Value;
                                        switch (itemname)
                                        {
                                            case "title":
                                                item.Title = val;
                                                break;
                                            case "link":
                                                item.Link = val;
                                                break;
                                            case "description":
                                            case "content:encoded":
                                                item.Context = val;
                                                break;
                                            case "pubDate":
                                                item.PubTime = DateTime.Parse(val).AddHours(8); //+8区
                                                break;
                                        }
                                    }

                                }

                            }
                        }
                    }
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private string GetXml(string RssUrl)
        {
            var ret = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(RssUrl);
            req.Method = "GET";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            try
            {
                var res = (HttpWebResponse)req.GetResponseAsync().Result;
                if (res != null)
                {
                    Stream s = res.GetResponseStream();
                    StreamReader sr = new StreamReader(s, Encoding.UTF8);
                    ret = sr.ReadToEnd();
                }
            }
            catch
            {

            }
            

            return ret;
        }
    }
}
