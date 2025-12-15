using EmailDemo.Data;
using EmailDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmailDemo.Repositories
{
    public class RoleRepo
    {
        private readonly ApplicationDbContext _db;

        public RoleRepo(ApplicationDbContext db)
        {
            _db = db;
            CreateInitialRole();
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            var roles = _db.Roles.ToList();

            return roles;
        }

        public IEnumerable<RoleVM> GetAllRolesVM()
        {
            var roles =
                _db.Roles.Select(r => new RoleVM
                {
                    RoleName = r.Name
                }).ToList();

            return roles;
        }

        public IdentityRole? GetRole(string roleName)
        {
            IdentityRole? role =
            _db.Roles.FirstOrDefault(r => r.Name == roleName);

            return role;
        }

        public RoleVM? GetRoleVM(string roleName)
        {
            IdentityRole? role = GetRole(roleName);

            if (role != null)
            {
                return new RoleVM { RoleName = role.Name };
            }

            return null;
        }


        public bool DoesRoleHaveUsers(string roleName)
        {
            IdentityRole? role = GetRole(roleName);
            if (role == null)
            {
                return false;
            }
            return
                _db.UserRoles.Any(r => r.RoleId == role.Id);
        }

        public bool CreateRole(string roleName)
        {
            if (GetRole(roleName) != null)
            {
                return false;
            }

            try
            {
                _db.Roles.Add(new IdentityRole
                {
                    Id = roleName.ToLower().Substring(0, 2),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating role '" +
                                  roleName + "' : " +
                                  ex.Message);
            }
            return false;
        }

        public bool DeleteRole(string roleName)
        {
            try
            {
                IdentityRole? role = GetRole(roleName);

                if (role == null)
                {
                    Console.WriteLine("Role not found.");
                    return false;
                }

                if (DoesRoleHaveUsers(roleName))
                {
                    Console.WriteLine("Role '" + roleName +
                                      "' cannot be deleted " +
                                      " because it has " +
                                      "associated users.");
                    return false;
                }
                _db.Roles.Remove(role);
                _db.SaveChanges();
                return true;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error deleting role '" +
                                  roleName + "' : " +
                                  ex.Message);
            }
            return false;
        }

        public SelectList GetRoleSelectList()
        {
            var roles = GetAllRoles().Select(r =>
                new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

            SelectList roleSelectList =
            new SelectList(roles, "Value", "Text");

            return roleSelectList;
        }

        public void CreateInitialRole()
        {
            const string ADMIN = "Admin";

            var role = GetRole(ADMIN);

            if (role == null)
            {
                CreateRole(ADMIN);
            }
        }
    }

}
