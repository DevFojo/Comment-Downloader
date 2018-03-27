using System.ComponentModel.DataAnnotations;

namespace CommentDownloader.Models
{
    public class CommentDownloadRequest
    {
        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }
    }
}