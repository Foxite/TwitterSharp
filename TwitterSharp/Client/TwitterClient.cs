﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TwitterSharp.Request;
using TwitterSharp.Request.Internal;
using TwitterSharp.Response;

namespace TwitterSharp.Client
{
    public class TwitterClient
    {
        public TwitterClient(string bearerToken)
        {
            _httpClient = new();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        #region TweetSearch
        public async Task<Tweet[]> GetTweetsByIdsAsync(params string[] ids)
        {
            var str = await _httpClient.GetStringAsync(_baseUrl + "tweets?ids=" + string.Join(",", ids.Select(x => HttpUtility.HtmlEncode(x))));
            return JsonSerializer.Deserialize<Answer<Tweet[]>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Data ?? Array.Empty<Tweet>();
        }

        public async Task<Tweet[]> GetTweetsFromUserIdAsync(string userId)
        {
            var str = await _httpClient.GetStringAsync(_baseUrl + "users/" + HttpUtility.HtmlEncode(userId) + "/tweets");
            return JsonSerializer.Deserialize<Answer<Tweet[]>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Data ?? Array.Empty<Tweet>();
        }
        #endregion TweetSearch

        #region TweetStream
        public async Task<StreamInfo[]> GetInfoTweetStreamAsync()
        {
            var str = await _httpClient.GetStringAsync(_baseUrl + "tweets/search/stream/rules");
            return JsonSerializer.Deserialize<Answer<StreamInfo[]>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Data ?? Array.Empty<StreamInfo>();
        }

        public async Task NextTweetStreamAsync(Action<Tweet> onNextTweet)
        {
            var stream = await _httpClient.GetStreamAsync(_baseUrl + "tweets/search/stream");
            using StreamReader reader = new(stream);
            while (!reader.EndOfStream)
            {
                var str = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(str))
                    continue;
                var result = JsonSerializer.Deserialize<Answer<Tweet>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Data;
                onNextTweet(result);
            }
        }

        public async Task<StreamInfo[]> AddTweetStreamAsync(params StreamRequest[] request)
        {
            var content = new StringContent(JsonSerializer.Serialize(new StreamRequestAdd { Add = request }), Encoding.UTF8, "application/json");
            var str = await (await _httpClient.PostAsync(_baseUrl + "tweets/search/stream/rules", content)).Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Answer<StreamInfo[]>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Data ?? Array.Empty<StreamInfo>();
        }

        public async Task<int> DeleteTweetStreamAsync(params string[] ids)
        {
            var content = new StringContent(JsonSerializer.Serialize(new StreamRequestDelete { Delete = new StreamRequestDeleteIds { Ids = ids } }), Encoding.UTF8, "application/json");
            var str = await (await _httpClient.PostAsync(_baseUrl + "tweets/search/stream/rules", content)).Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Answer<object>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Meta.Summary.Deleted;
        }
        #endregion TweetStream

        #region UserSearch
        public async Task<User[]> GetUsersAsync(params string[] usernames)
        {
            var str = await _httpClient.GetStringAsync(_baseUrl + "users/by?usernames=" + string.Join(",", usernames.Select(x => HttpUtility.HtmlEncode(x))));
            return JsonSerializer.Deserialize<Answer<User[]>>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Data ?? Array.Empty<User>();
        }
        #endregion UserSearch

        private const string _baseUrl = "https://api.twitter.com/2/";

        private readonly HttpClient _httpClient;
    }
}
