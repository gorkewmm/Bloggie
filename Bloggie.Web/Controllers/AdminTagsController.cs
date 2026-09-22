using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly BloggieDbContext _context;
        public AdminTagsController(BloggieDbContext context)
        {
            _context = context;
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddTagRequest addTagRequest)
        {
            var tag = new Tag()
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            var tags = _context.Tags.ToList();

            return View(tags);
        }

        public IActionResult Edit(Guid id)
        {
            var editTagRequest = new EditTagRequest();
            var tag = _context.Tags.Find(id);
            if (tag == null)
            {
                return RedirectToAction("List");
            }

            editTagRequest.Id = tag.Id;
            editTagRequest.Name = tag.Name;
            editTagRequest.DisplayName = tag.DisplayName;

            return View(editTagRequest);
        }

        [HttpPost]
        public IActionResult Edit(EditTagRequest editTagRequest)
        {
            var tag = _context.Tags.Find(editTagRequest.Id);
            if(tag == null)
            {
                return RedirectToAction("Edit", new {editTagRequest.Id});
            }

            tag.Name = editTagRequest.Name;
            tag.DisplayName = editTagRequest.DisplayName;

            _context.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult Delete(EditTagRequest editTagRequest)
        {
            var tag = _context.Tags.Find(editTagRequest.Id);

            if (tag != null)
            {
                _context.Tags.Remove(tag);
                _context.SaveChanges();

                return RedirectToAction("List");
            }

            return RedirectToAction("Edit", new {id = editTagRequest.Id});

        }
    }
}
