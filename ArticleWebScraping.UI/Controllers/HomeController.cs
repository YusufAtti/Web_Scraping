using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ArticleWebScraping.UI.Models;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace ArticleWebScraping.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient();
        string url = $"https://localhost:7091/api/Scraping/getAllArticleList";
        var responseMessage = await client.GetAsync(url);

        if (responseMessage.IsSuccessStatusCode)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();
            var searchResults = JsonConvert.DeserializeObject<List<Article>>(content);
            return View(searchResults);
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return View();
        }
        else
        {
            string url = $"https://localhost:7091/api/Scraping/scraping?keyword={searchTerm}";
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                string url1 = $"https://localhost:7091/api/Scraping/getArticleList?keyword={searchTerm}";
                var responseMessage1 = await client.GetAsync(url1);

                if (responseMessage1.IsSuccessStatusCode)
                {
                    var content = await responseMessage1.Content.ReadAsStringAsync();
                    var searchResults = JsonConvert.DeserializeObject<List<Article>>(content);
                    return View(searchResults);
                }
            }
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return View();
        }
        else
        {
                var client = _httpClientFactory.CreateClient();
                string url1 = $"https://localhost:7091/api/Scraping/getArticleList?keyword={searchTerm}";
                var responseMessage1 = await client.GetAsync(url1);

                if (responseMessage1.IsSuccessStatusCode)
                    {
                        var content = await responseMessage1.Content.ReadAsStringAsync();
                        var searchResults = JsonConvert.DeserializeObject<List<Article>>(content);
                        return View(searchResults);
                    }
                return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listele(string q)
    {
        if (string.IsNullOrEmpty(q))
        {
            return View();
        }
        else
        {
            var client = _httpClientFactory.CreateClient();
            if(q == "yakin")
            {
                string url1 = $"https://localhost:7091/api/Scraping/getAllPublishDateYakin";
                var responseMessage1 = await client.GetAsync(url1);

                if (responseMessage1.IsSuccessStatusCode)
                {
                    var content = await responseMessage1.Content.ReadAsStringAsync();
                    var searchResults = JsonConvert.DeserializeObject<List<Article>>(content);
                    return View(searchResults);
                }
            }
            if(q == "uzak")
            {
                string url1 = $"https://localhost:7091/api/Scraping/getAllPublishDateUzak";
                var responseMessage1 = await client.GetAsync(url1);

                if (responseMessage1.IsSuccessStatusCode)
                {
                    var content = await responseMessage1.Content.ReadAsStringAsync();
                    var searchResults = JsonConvert.DeserializeObject<List<Article>>(content);
                    return View(searchResults);
                }
            }
            
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Page(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return View();
        }
        else
        {
            var client = _httpClientFactory.CreateClient();
            string url1 = $"https://localhost:7091/api/Scraping/getArticleByUrl?keyword={url}";
            var responseMessage1 = await client.GetAsync(url1);

            if (responseMessage1.IsSuccessStatusCode)
            {
                var content = await responseMessage1.Content.ReadAsStringAsync();
                var searchResults = JsonConvert.DeserializeObject<Article>(content);
                return View(searchResults);
            }
            return View();
        }
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

