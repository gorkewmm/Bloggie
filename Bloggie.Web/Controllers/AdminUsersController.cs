using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AdminUsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IActionResult> List()
        {
            var users = await _userRepository.GetAll();

            var usersViewModel = new UserViewModel();
            usersViewModel.Users = new List<User>();
            foreach (var user in users)
            {
                var exampleUser = new User
                {
                    Id = Guid.Parse(user.Id),
                    EmailAddress = user.Email,
                    Username = user.UserName
                };
                usersViewModel.Users.Add(exampleUser);
            }

            return View(usersViewModel);
        }
    }
}
