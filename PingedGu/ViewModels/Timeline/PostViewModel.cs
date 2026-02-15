namespace PingedGu.ViewModels.Timeline
{
    public class PostViewModel
    {
        public string Content { get; set; }

        // IFormFile → a property for sending an image file via HTTPS request
        public IFormFile Image {  get; set; }
    }
}
