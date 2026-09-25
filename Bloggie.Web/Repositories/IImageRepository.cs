namespace Bloggie.Web.Repositories
{
    public interface IImageRepository
    {
        Task<string> UplaodAsync(IFormFile file);
    }
}
