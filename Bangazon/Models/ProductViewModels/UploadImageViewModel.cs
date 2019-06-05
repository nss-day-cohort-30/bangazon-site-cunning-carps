using Microsoft.AspNetCore.Http;


namespace Bangazon.Models.ProductViewModels
{
    public class UploadImageViewModel
    {

        public Product product { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
