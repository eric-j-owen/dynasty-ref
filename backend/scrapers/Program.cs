// using System.Net.Http.Headers;
// using System.Net.Http.Json;
// using Microsoft.Extensions.Configuration;
// using Microsoft.EntityFrameworkCore;
using System.Text.Json;
// using System.IO;
using HtmlAgilityPack;

using Data;
using Data.Models;


await Scraper();
async Task Scraper()
{
    //initialize and set user agent
    var web = new HtmlWeb();
    web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    //testing- load page 1 of ktc rankings
    var htmlDoc = web.Load("https://keeptradecut.com/dynasty-rankings?page=0");

    var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

    Console.WriteLine(node.Name + "\n" + node.OuterHtml);
}
