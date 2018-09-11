using System;
using System.Threading;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            RssSpider rs = new RssSpider();
            rs.Start();
        }
    }
}