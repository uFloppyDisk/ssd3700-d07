using EmailDemo.Repositories;
using EmailDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailDemo.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UserRoleController : Controller
    {
        private readonly RoleRepo _roleRepo;
        private readonly UserRepo _userRepo;
        private readonly UserRoleRepo _userRoleRepo;

        public UserRoleController(UserManager<IdentityUser> userManager,
                                  RoleRepo roleRepo,
                                  UserRepo userRepo,
                                  UserRoleRepo userRoleRepo,
                                  ILogger<UserRoleController> logger)
        {
            _roleRepo = roleRepo;
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
        }

        // Displays a list of all registered users.
        public IActionResult Index()
        {
            var users = _userRepo.GetAllUsers();
            return View(users);
        }


        // Displays the roles assigned to a specific user.
        public async Task<IActionResult> Detail(string userName,
                                                string message = "")
        {
            var roles = await _userRoleRepo.GetUserRolesAsync(userName);
            ViewBag.Message = message;
            ViewBag.UserName = userName;
            return View(roles);
        }

        // Displays the form for assigning a new role to a user.
        [HttpGet]
        public IActionResult Create(string? email)
        {
            ViewBag.RoleSelectList = _roleRepo.GetRoleSelectList();
            ViewBag.UserSelectList = _userRepo.GetUserSelectList(email);
            return View();
        }

        // Processes the role assignment for a user.
        [HttpPost]
        public async Task<IActionResult> Create(UserRoleVM userRoleVM)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _userRoleRepo.AddUserRoleAsync(userRoleVM.Email,
                                                      userRoleVM.RoleName);
                if (result)
                {
                    string message = $"{userRoleVM.RoleName} permissions " +
                                      "successfully added to " +
                                       userRoleVM.Email;

                    return RedirectToAction(nameof(Detail),
                        new { userName = userRoleVM.Email, message });
                }
                else
                {
                    ModelState.AddModelError("",
                                             "Failed to add role to user." +
                                             " The role might already " +
                                             " exist for this user.");
                }
            }

            ViewBag.RoleSelectList = _roleRepo.GetRoleSelectList();
            ViewBag.UserSelectList =
                            _userRepo.GetUserSelectList(userRoleVM.Email);
            return View(userRoleVM);
        }


        // Displays the confirmation page for removing a role from a user.
        [HttpGet]
        public IActionResult Delete(string email,
                                    string roleName)
        {
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("", "Email and Role Name " +
                                             "are required.");
                return RedirectToAction(nameof(Index));
            }

            var userRoleVM = new UserRoleVM
            {
                Email = email,
                RoleName = roleName
            };
            return View(userRoleVM);
        }

        // Processes the role removal from a user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
                                     DeleteConfirmed(UserRoleVM userRoleVM)
        {
            if (ModelState.IsValid)
            {
                var result =
                   await _userRoleRepo.RemoveUserRoleAsync(userRoleVM.Email,
                                                       userRoleVM.RoleName);

                if (result)
                {
                    string message = $"{userRoleVM.RoleName} permissions " +
                                     $"successfully removed from " +
                                     $"{userRoleVM.Email}.";

                    return RedirectToAction(nameof(Detail),
                        new { userName = userRoleVM.Email, message });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to remove role " +
                                                 "from user.");
                }
            }

            return RedirectToAction(nameof(Detail),
                                    new { userName = userRoleVM.Email });
        }
    }
}
