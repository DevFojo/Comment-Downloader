using System.Threading.Tasks;
using CommentDownloader.Models;
using CommentDownloader.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentDownloader.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CommentDownloadService _commentDownloadService;

        public IndexModel(CommentDownloadService commentDownloadService)
        {
            _commentDownloadService = commentDownloadService;
        }

        [BindProperty] public CommentDownloadRequest CommentDownloadRequest { get; set; }

        [BindProperty] public CommentDownloadResponse CommentDownloadResponse { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            CommentDownloadResponse = new CommentDownloadResponse();
            if (string.IsNullOrWhiteSpace(CommentDownloadRequest.Url))
            {
                CommentDownloadResponse.Error = "Invalid download url";
            }
            else
            {
                CommentDownloadResponse = await _commentDownloadService.GetComments(CommentDownloadRequest);
            }
            return Page();
        }
    }
}