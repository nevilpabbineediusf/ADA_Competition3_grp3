using Competition3Grp3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Competition3Grp3.Controllers
{
    public class NewsController : Controller
    {
        private readonly HttpClient _httpClient;

        public NewsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            string url = "https://newsapi.org/v2/everything?q=tesla&from=2025-03-22&sortBy=publishedAt&apiKey=dde5b8bdc8d14abb91eb0f15d7bebd3a";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                return Content($"API call failed: {response.StatusCode}\n\n{error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var articles = new List<Article>();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                var articleList = root.GetProperty("articles");

                foreach (var item in articleList.EnumerateArray())
                {
                    articles.Add(new Article
                    {
                        SourceName = item.GetProperty("source").GetProperty("name").GetString(),
                        Title = item.GetProperty("title").GetString(),
                        Url = item.GetProperty("url").GetString()
                    });
                }
            }

            return View(articles);
        }
    }

}
