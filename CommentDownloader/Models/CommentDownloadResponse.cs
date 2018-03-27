using System.Collections.Generic;
using CommentDownloader.Services;

namespace CommentDownloader.Models
{
    public class CommentDownloadResponse
    {
        public CommentDownloadResponse()
        {
            Comments = new List<Comment>();
        }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public List<Comment> Comments { get; set; }
        public string Source { get; set; }
        public string Error { get; set; }
    }
}