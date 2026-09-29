using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminUsersController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
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

        [HttpPost]
        public async Task<IActionResult> List(UserViewModel userViewModel)
        {
            var identityUser = new IdentityUser()
            {
                Email = userViewModel.Email,
                UserName = userViewModel.Username,
            };

            var result = await _userManager.CreateAsync(identityUser, userViewModel.Password);

            if(result != null)
            {
                if (result.Succeeded)
                {
                    var roles = new List<string>() { "User" };

                    if (userViewModel.AdminRoleCheckbox)
                    {
                        roles.Add("Admin");
                    }
                    var roleResult = await _userManager.AddToRolesAsync(identityUser, roles);

                    if (roleResult != null && roleResult.Succeeded)
                    {
                        return RedirectToAction("List", "AdminUsers");
                    }
                }
            }           
            return View();

        }
    }
}
