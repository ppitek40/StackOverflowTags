using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestEase;
using StackOverflowTags.Controllers;
using StackOverflowTags.Models;

namespace StackOverflowTags.APIs
{
    public class StackOverflowApiAdapter
    {
        private const string Filter = "filter=!21k7qaosV))FH-ep*dHKr";
        private const string ApiVersion = "2.3";
        private const string Site = "stackoverflow";
        private const string Sort = "popular";
        private const string Order = "desc";
        private readonly IStackOverflowApi _stackOverflowApi;
        private readonly ILogger<TagsController> _logger;

        public StackOverflowApiAdapter(ILogger<TagsController> logger)
        {
            _stackOverflowApi = RestClient.For<IStackOverflowApi>("https://api.stackexchange.com/"+ApiVersion);
            _logger = logger;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            var result = new List<Tag>();
            
            for (var i = 1; i <= 10; i++)
            {
                Stream response;

                try
                {
                    response = await _stackOverflowApi.GetTagsAsync(i,100, Site, Sort, Order, Filter);
                }
                catch (ApiException e)
                {
                    _logger.LogError(e, "Unable to get response due to API exception");
                    return null;
                }

                var content = DeserializeResponse<GetTagsResponse>(response);
                result.AddRange(content.Items);
                await response.DisposeAsync();
            }

            _logger.LogDebug("Successfully obtained 1000 tags");
            return result;

        }
        public T DeserializeResponse<T>(Stream stream)
        {
            string stringContent;

            using (var gZipStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gZipStream))
            {
                stringContent = streamReader.ReadToEndAsync().Result;
            }

            var content = JsonSerializer.Deserialize<T>(stringContent);

            return content;
        }

    }

    public class GetTagsResponse
    {
        [JsonPropertyName("items")]
        public List<Tag> Items { get; set; }
    }

    public interface IStackOverflowApi
    {
        [Get("tags")]
        Task<Stream> GetTagsAsync([Query] int page,[Query] int pagesize, [Query] string site,[Query] string sort, [Query] string order, [RawQueryString] string filter);
    }
}
