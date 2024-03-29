﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Megamozg
{
    [Export(typeof(IGrabber.IGrabber))]
    public class Megamozg : IGrabber.IGrabber
    {

        public Megamozg() { }


        public int Interval { get { return 3; } }
        public string Tag { get { return "Megamozg"; } }
        public string Url { get { return "http://megamozg.ru/"; } }

        public List<Tuple<string, string>> GetInfo()
        {
            try
            {
                List<Tuple<string, string>> cache = new List<Tuple<string, string>>();
                string data = MyWebParser.Parser.GetHtml(Url);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(data);
                HtmlNode body = doc.DocumentNode.SelectSingleNode("//div[@class=\"posts shortcuts_items\"]");
                foreach (HtmlNode node in body.SelectNodes("//div[@class=\"post shortcuts_item\"]"))
                {
                    HtmlNode temp = node.SelectSingleNode("//div[@id=\"" + node.Id + "\"]/h1[@class=\"title\"]/a");
                    cache.Add(new Tuple<string, string>(temp.InnerHtml, temp.Attributes["href"].Value));
                }
                return cache;
            }
            catch
            {
                return null;
            }

        }

    }
}
