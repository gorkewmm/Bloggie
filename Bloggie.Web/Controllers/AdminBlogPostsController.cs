using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            _tagRepository = tagRepository;
            _blogPostRepository = blogPostRepository;
        }
        public async Task<IActionResult> Add()
        {
            //get tags from repository
            var allTags = await _tagRepository.GetAllAsync();
            var model = new AddBlogPostRequest()
            {
                Tags = allTags.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            //map view model to domain model
            var blogPost = new BlogPost()
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
            };

            var tags = new List<Tag>();
            var selectedTags = addBlogPostRequest.SelectedTags;
            foreach (var selectedTag in selectedTags)
            {
                var selectedTagGuid = Guid.Parse(selectedTag);

                var existedTag = await _tagRepository.GetAsync(selectedTagGuid);
                if (existedTag != null)
                {
                    tags.Add(existedTag);
                }
            }

            blogPost.Tags = tags;

            await _blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("Add");
        }

        public async Task<IActionResult> List()
        {
            var blogPosts = await _blogPostRepository.GetAllAsync();
            return View(blogPosts);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var existingBlogPost = await _blogPostRepository.GetAsync(id);
            var tagDomainsModel = await _tagRepository.GetAllAsync();

            if (existingBlogPost != null)
            {
                var model = new EditBlogPostRequest()
                {
                    Id = existingBlogPost.Id,
                    Heading = existingBlogPost.Heading,
                    PageTitle = existingBlogPost.PageTitle,
                    Content = existingBlogPost.Content,
                    ShortDescription = existingBlogPost.ShortDescription,
                    FeaturedImageUrl = existingBlogPost.FeaturedImageUrl,
                    UrlHandle = existingBlogPost.UrlHandle,
                    PublishedDate = existingBlogPost.PublishedDate,
                    Author = existingBlogPost.Author,
                    Visible = existingBlogPost.Visible,
                };

                var tags = tagDomainsModel.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
                model.Tags = tags;

                var selectedTags = existingBlogPost.Tags.
                    Select(x => x.Id.ToString()).ToArray();

                model.SelectedTags = selectedTags;

                return View(model);
            }

            return View(null);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            var blogPost = new BlogPost();

            blogPost.Id = editBlogPostRequest.Id;
            blogPost.Heading = editBlogPostRequest.Heading;
            blogPost.PageTitle = editBlogPostRequest.PageTitle;
            blogPost.Content = editBlogPostRequest.Content;
            blogPost.ShortDescription = editBlogPostRequest.ShortDescription;
            blogPost.FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl;
            blogPost.UrlHandle = editBlogPostRequest.UrlHandle;
            blogPost.PublishedDate = editBlogPostRequest.PublishedDate;
            blogPost.Author = editBlogPostRequest.Author;
            blogPost.Visible = editBlogPostRequest.Visible;

            var tagList = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(selectedTag, out var tagId))
                {
                    var foundedTag = await _tagRepository.GetAsync(tagId);
                    if (foundedTag != null)
                    {
                        tagList.Add(foundedTag);
                    }
                }
            }

            blogPost.Tags = tagList;

            var updatedBlog = await _blogPostRepository.UpdateAsync(blogPost);

            if (updatedBlog != null)
            {
                //Show success notification
                return RedirectToAction("Edit");
            }


            //Error success notification
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            var deletedBlogPost = await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                //Show succes notifications
                return RedirectToAction("List");
            }
            //Show errror notifications
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }
}