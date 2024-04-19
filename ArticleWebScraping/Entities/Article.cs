using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArticleWebScraping.Entities
{
    public class Article
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

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

