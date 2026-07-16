using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFirstBlogTests
{
    public class PostsControllerTests
    {
        private readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000")
        };

        private Task<HttpResponseMessage> PostAsync(object body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return _client.PostAsync("/posts", content);
        }

        [Test]
        public async Task CreatePost_WithValidData_Returns201WithPost()
        {
            var response = await PostAsync(new { title = "some title", description = "some content" });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var post = body.RootElement.GetProperty("post");

            Assert.That(post.GetProperty("title").GetString(), Is.EqualTo("some title"));
            Assert.That(post.GetProperty("description").GetString(), Is.EqualTo("some content"));
        }

        [Test]
        public async Task CreatePost_WithBlankTitle_Returns400WithErrors()
        {
            var response = await PostAsync(new { title = "", description = "some content" });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var errors = body.RootElement.GetProperty("errors");

            Assert.That(errors[0].GetString(), Is.EqualTo("Title cannot be blank"));
        }
    }
}
