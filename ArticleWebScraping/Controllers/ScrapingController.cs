using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArticleWebScraping.Entities;
using System.Collections;
using System.Text.Json;
using ArticleWebScraping.Services;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArticleWebScraping.Controllers
{
    [Route("api/[controller]")]
    public class ScrapingController : Controller
    {

        private readonly ScrapingService _articleService;


        public ScrapingController(HttpClient httpClient, ScrapingService scrapingService)
        {
            _articleService = new ScrapingService();
        }
        

        [HttpGet("scraping")]
        public async Task<IActionResult> SearchString([FromQuery] String keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Aranacak kelimeyi boş girdiniz.");
            }

            string url = $"https://dergipark.org.tr/tr/search?q={keyword}&section=articles";

            List<TitleAndLinks> links = await _articleService.FindLinks(url);

            foreach (var item in links)
            {
                await _articleService.AddInfos(item.Link);
            }

            return Ok("Scraping is ok.");
        }

        [HttpGet("getArticleList")]
        public async Task<IActionResult> getArticleList([FromQuery] String keyword)
        {
            List<Article> articles = await _articleService.GetArticles(keyword);
            return Ok(articles);
        }

        [HttpGet("getArticleByUrl")]
        public async Task<IActionResult> getArticleByUrl([FromQuery] String keyword)
        {
            Article article = await _articleService.GetArticleByUrl1(keyword);
            return Ok(article);
        }


        [HttpGet("getAllArticleList")]
        public async Task<IActionResult> getAllArticleList()
        {
            List<Article> articles = await _articleService.GetAllArticles();
            return Ok(articles);
        }

        [HttpGet("getAllPublishDateYakin")]
        public async Task<IActionResult> getAllPublishDateYakin()
        {
            List<Article> articles = await _articleService.getAllPublishDateYakin();
            return Ok(articles);
        }

        [HttpGet("getAllPublishDateUzak")]
        public async Task<IActionResult> getAllPublishDateUzak()
        {
            List<Article> articles = await _articleService.getAllPublishDateUzak();
            return Ok(articles);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] String keyword)
        {
            List<Article> articles = await _articleService.GetArticles(keyword);
            return Ok(articles);
        }

    }
}

