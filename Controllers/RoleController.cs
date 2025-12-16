using EmailDemo.Repositories;
using EmailDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailDemo.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class RoleController : Controller
    {
        private readonly RoleRepo _roleRepo;
        private readonly ILogger<RoleController> _logger;

        public RoleController(RoleRepo roleRepo,
                              ILogger<RoleController> logger)
        {
            _roleRepo = roleRepo;
            _logger = logger;
        }

        public ActionResult Index(string message = "")
        {
            IEnumerable<RoleVM> roles = _roleRepo.GetAllRolesVM();
            ViewBag.Message = message;

            return View(roles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new RoleVM());
        }

        [HttpPost]
        public ActionResult Create(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess =
                    _roleRepo.CreateRole(roleVM.RoleName);

                if (isSuccess)
                {
                    string message = "Successfully added " +
                                     roleVM.RoleName + " to Roles";

                    return RedirectToAction(nameof(Index),
                                            new { message });
                }
                else
                {
                    string message = "Role creation failed. " +
                                     roleVM.RoleName + " may " +
                                     "already exist.";

                    ModelState.AddModelError("", message);
                    _logger.LogError(message);
                }
            }
            return View(roleVM);
        }

        [HttpGet]
        public ActionResult Delete(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                string message = "Role name cannot be empty.";
                _logger.LogWarning(message);
                return RedirectToAction(nameof(Index), new { message });
            }

            RoleVM? role = _roleRepo.GetRoleVM(roleName);

            if (role == null)
            {
                string message = $"Role '{roleName}' not found.";
                _logger.LogWarning(message);
                return RedirectToAction(nameof(Index),
                    new { message = message });
            }

            return View(role);
        }

        [HttpPost]
        public ActionResult Delete(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess =
                    _roleRepo.DeleteRole(roleVM.RoleName);

                if (isSuccess)
                {
                    string message = "Successfully removed " +
                                     roleVM.RoleName + " from Roles";

                    return RedirectToAction(nameof(Index),
                        new { message = message });
                }
                else
                {
                    string message = "Role deletion failed. " +
                                     roleVM.RoleName + " may " +
                                     "have users attached.";

                    ModelState.AddModelError("", message);
                    _logger.LogError(message);
                }
            }

            return View(roleVM);
        }
    }
}
