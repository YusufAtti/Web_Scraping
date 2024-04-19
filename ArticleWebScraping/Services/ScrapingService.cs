using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using ArticleWebScraping.Entities;
using ArticleWebScraping.Helpers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;

namespace ArticleWebScraping.Services
{
	public class ScrapingService
	{
        string connString = "mongodb://localhost:27017";
        string databaseName = "WebScraping";
        string collectionName = "thesis";

        public async Task<List<TitleAndLinks>> FindLinks(string url)
        {

            List<TitleAndLinks> titleAndLinks = new List<TitleAndLinks>();

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var nodes = htmlDocument.DocumentNode.SelectNodes("//h5[@class='card-title']/a");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    string title = node.InnerText.Trim();
                    string link = node.Attributes["href"].Value;
                    titleAndLinks.Add(new TitleAndLinks { Title = title, Link = link });
                }
            }

            return titleAndLinks;

        }


        public async Task AddInfos(string url)
        { 

                var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
                var data = await helper.GetByUrlAsync(url);
                if (data == null)
                {
                List<string> keywords = new List<string>();
                List<string> authors = new List<string>();
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);

                var pubType = doc.DocumentNode.SelectSingleNode("//div[@class='kt-portlet__head-title']/span[@class='kt-font-bold']");
                var summaryNode = doc.QuerySelectorAll("div.article-abstract.data-section").FirstOrDefault();
                var keywordNodes = doc.QuerySelectorAll("div.article-keywords.data-section").FirstOrDefault()?.QuerySelectorAll("a");
                var articleAuthors = doc.QuerySelectorAll("p.article-authors").FirstOrDefault()?.QuerySelectorAll("a");
                //var articleName = doc.DocumentNode.SelectSingleNode("//h3[@class='article-title']");
                var articleName = doc.QuerySelector("div.h3.d-flex.align-items-baseline > h3.article-title");
                var references = doc.DocumentNode
                .SelectNodes("//ul[@class='fa-ul']/li[not(contains(@class, 'd-none reference-item'))]")
                ?.Select(li => li.InnerText.Trim())
                .ToList();

                var publishDateNode = doc.DocumentNode.SelectSingleNode("//th[text()='Yayımlanma Tarihi']/following-sibling::td");

                string publishDate = publishDateNode?.InnerText.Trim() ?? "";
                string articleName1 = articleName?.InnerText.Trim() ?? "";
                string pubType1 = pubType?.InnerText.Trim() ?? "";
                string ozet = summaryNode?.InnerText.Trim() ?? "";

                if (articleAuthors != null)
                {
                    await Task.WhenAll(articleAuthors.Select(async aut =>
                    {
                        string author = aut.InnerText.Trim();
                        if (!string.IsNullOrEmpty(author))
                        {
                            authors.Add(author);
                        }
                    }));
                }

                if (keywordNodes != null)
                {
                    await Task.WhenAll(keywordNodes.Select(async keywordNode =>
                    {
                        string keyword = keywordNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(keyword))
                        {
                            keywords.Add(keyword);
                        }
                    }));
                }
                int referenceCount = 0;
                if (references != null)
                {
                    referenceCount = references.Count;
                }

                string cleanedString = ozet.Replace("\r\n", "");


                var article = new Article()
                {
                    Summary = cleanedString,
                    ArticleUrl = url,
                    KeywordsForArticle = keywords,
                    Authors = authors,
                    ArticleName = articleName1,
                    PublishDate = publishDate,
                    PubType = pubType1,
                    References = references,
                    QuotationCount = referenceCount
                };

                await helper.InsertAsync(article);
            }

        }

        public async Task<List<Article>> GetArticlesFromMongoDB()
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articlesFromMongoDB = await helper.GetAllAsync();

            return (List<Article>)articlesFromMongoDB;
        }

        public async Task<List<Article>> SearchArticlesInElasticsearch(string searchTerm)
        {
            // Elasticsearch bağlantısı
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("articles"); // Elasticsearch index adınız
            var elasticClient = new ElasticClient(settings);

            // Elasticsearch'te arama yap
            var searchResponse = await elasticClient.SearchAsync<Article>(s => s
                .Query(q => q
                    .Terms(t => t
                        .Field(f => f.ArticleName) // KeywordsForArticle alanında arama yap
                        .Terms(searchTerm) // Aranan terimi içeren belgeleri al
                    )
                )
            );

            // Elasticsearch'ten gelen belgeleri döndür
            var articlesFromElasticsearch = searchResponse.Documents.ToList();
            return articlesFromElasticsearch;
        }

        
        public async Task<List<Article>> GetArticles(string keywords)
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articles = await helper.GetAllFilterAsync("KeywordsForArticle", keywords);
            return (List<Article>)articles;
        }

        public async Task<List<Article>> GetArticleByUrl(string keywords)
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articles = await helper.GetAllFilterAsync("KeywordsForArticle", keywords);
            return (List<Article>)articles;
        }

        public async Task<Article> GetArticleByUrl1(string keywords)
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articles = await helper.GetByUrlAsync(keywords);
            return articles;
        }

        public async Task<List<Article>> GetAllArticles()
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articles = await helper.GetAllAsync();
            return (List<Article>)articles;
        }

        public async Task<List<Article>> getAllPublishDateUzak()
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articles = await helper.GetAllAsync();
            var sortedList = articles.OrderBy(item => item.QuotationCount);

            return sortedList.ToList();
        }

        public async Task<List<Article>> getAllPublishDateYakin()
        {
            var helper = new MongoHelper<Article>(connString, databaseName, collectionName);
            var articles = await helper.GetAllAsync();
            var sortedList = articles.OrderByDescending(item => item.QuotationCount);

            return sortedList.ToList();

        }


    }
}

