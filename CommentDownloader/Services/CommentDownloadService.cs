using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommentDownloader.Models;
using Newtonsoft.Json.Linq;

namespace CommentDownloader.Services
{
    public class CommentDownloadService
    {
        private readonly string _apiKey = "AIzaSyDT3xayIRu9-fEdhGtiW7f92TAdCpSnphs";
        private const string YoutubeHost = "www.youtube.com";
        private const string AmazonHost = "www.amazon.com";

        public async Task<CommentDownloadResponse> GetComments(CommentDownloadRequest request)
        {
            var response = new CommentDownloadResponse();
            var downloadUri = new Uri(request.Url);
            switch (downloadUri.Host)
            {
                case YoutubeHost:
                    var youtubeCommentsResponse = await GetYoutubeComments(downloadUri);
                    response.Source = "YOUTUBE";
                    response.Comments = youtubeCommentsResponse.Comments;
                    response.IsSuccessful = youtubeCommentsResponse.IsSuccessful;
                    response.Message = string.IsNullOrWhiteSpace(youtubeCommentsResponse.Message) ? youtubeCommentsResponse.Error : youtubeCommentsResponse.Message;
                    break;
                case AmazonHost:
                    var amazonCommentResponse = await GetAmazonComments(request);
                    break;
                default:
                    response.IsSuccessful = false;
                    response.Message = "";
                    break;
            }

            return response;
        }

        private async Task<CommentDownloadResponse> GetAmazonComments(CommentDownloadRequest request)
        {
            throw new NotImplementedException();
        }

        private async Task<CommentDownloadResponse> GetYoutubeComments(Uri requestUri)
        {
            var response = new CommentDownloadResponse();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            var videoId = GetYoutubeVideoId(requestUri.Query);
            if (string.IsNullOrWhiteSpace(videoId))
            {
                response.IsSuccessful = false;
                response.Error = "Invalid video id";
            }
            else
            {
                var responseMessage = await httpClient.GetAsync($"/youtube/v3/commentThreads?part=snippet%2Creplies&maxResults=100&videoId={videoId}&key={_apiKey}");
                var responseString = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    response.IsSuccessful = true;
                    var youtubeCommentResponseJson = JObject.Parse(responseString);
                    var youtubeComments = youtubeCommentResponseJson["items"];
                    foreach (var youtubeComment in youtubeComments)
                    {
                        var commentDetails = youtubeComment["snippet"]["topLevelComment"]["snippet"];
                        response.Comments.Add(new Comment
                        {
                            Author = commentDetails["authorDisplayName"].ToObject<string>(),
                            Date = commentDetails["publishedAt"].ToObject<string>(),
                            Rating = commentDetails["likeCount"].ToObject<int>(),
                            Body = commentDetails["textOriginal"].ToObject<string>()
                        });
                    }

                }
                else
                {
                    response.IsSuccessful = false;
                    response.Error = responseString;
                }
            }

            return response;
        }

        private static string GetYoutubeVideoId(string query)
        {
            var splitQuery = query.Split("=");
            return splitQuery.Length > 0 ? splitQuery[1] : string.Empty;
        }
    }
}