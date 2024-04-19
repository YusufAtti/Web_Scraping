using System;
namespace ArticleWebScraping.UI.Models
{
	public class Article
	{
        public string ArticleName { get; set; }

        public List<string> Authors { get; set; }

        public string PubType { get; set; }

        public string PublishDate { get; set; }

        public string PublisherName { get; set; }

        public List<string> KeywordsForSearch { get; set; }

        public List<string> KeywordsForArticle { get; set; }

        public string Summary { get; set; }

        public List<string> References { get; set; }

        public int QuotationCount { get; set; }

        public int DoiNumber { get; set; }

        public string ArticleUrl { get; set; }
    }
}

