using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpAuthenticationAndAuthorization.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAuthenticationAndAuthorization.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> rolemanager;
        private readonly UserManager<IdentityUser> usermanager;

        public AdministrationController(RoleManager<IdentityRole> rolemanager, UserManager<IdentityUser> userManager)
        {
            this.rolemanager = rolemanager;
            this.usermanager = userManager;
        }
        public IActionResult ListUsers()
        {
           var users= usermanager.Users;
            return View(users);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user =await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with id={id} is not found";
                View("Not Found");
            }
            var userclaims =await usermanager.GetClaimsAsync(user);
            var userroles = await usermanager.GetRolesAsync(user);

            var model = new EditUserViewModel {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Claims = userclaims.Select(c => c.Value).ToList(),
                Roles=userroles
            
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await usermanager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with id={model.Id} is not found";
                View("Not Found");
            }
            user.Id = model.Id;
            user.UserName = model.UserName;
            user.Email = model.Email;
            var result=await  usermanager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult>DeleteRole(string id)
        {
            
                var role = await rolemanager.FindByIdAsync(id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"The role with {id} does not exists";
                    return NotFound();
                }
                else
                {
                    var result = await rolemanager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }


                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);

                    }
                    return View("ListRoles");
                                
            }
        }

        public async Task<IActionResult> ManageUserRoles(string userid)
        {
            ViewBag.userid = userid;
            var user = await usermanager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id={userid} does not exists";
                return NotFound();
            }

            var model = new List<UserRolesViewModel>();
            foreach (var role in rolemanager.Roles.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId=role.Id,
                    RoleName=role.Name
                };
                if(await usermanager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }

                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult>ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await usermanager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await usermanager.GetRolesAsync(user);
            var result = await usermanager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await usermanager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with id={id} is not found";
                View("Not Found");
            }
            
            var result = await usermanager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(user);
        }

        public IActionResult ListRoles()
        {
            var listRoles = rolemanager.Roles;
            return View(listRoles);
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole myrole = new IdentityRole
                {
                    Name = model.Name
                };
                IdentityResult result = await rolemanager.CreateAsync(myrole);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string Id)
        {
            
            var role = await rolemanager.FindByIdAsync(Id);
            //MySqlException: There is already an open DataReader associated with this Connection which must be closed first.
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {Id} cannot be found";
                return View("NotFound");
            }

            var model = new EditViewRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (var user in usermanager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await usermanager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditViewRoleModel model)
        {
            var role = await rolemanager.FindByIdAsync(model.Id);
            //MySqlException: There is already an open DataReader associated with this Connection which must be closed first.
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
               var result= await rolemanager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
                return View(model);
            }
            

            // Retrieve all the Users
            

            
        }
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await rolemanager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await usermanager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await usermanager.IsInRoleAsync(user, role.Name)))
                {
                    result = await usermanager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await usermanager.IsInRoleAsync(user, role.Name))
                {
                    result = await usermanager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            List<UserRoleViewModel> lstUserRoleModel = new List<UserRoleViewModel>();
            var role = rolemanager.FindByIdAsync(roleId);
            string rolename = role.Result.Name;
            if (role == null)
            {
                ViewBag.ErrorMessage = $"This role with id={roleId} is not found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            foreach (var user in usermanager.Users.ToList())
                {
                    
                        var objUserRoleViewModel = new UserRoleViewModel
                        {
                            UserId = user.Id,
                            UserName = user.UserName
                        };
                    if (await usermanager.IsInRoleAsync(user, rolename))
                    {
                        objUserRoleViewModel.IsSelected = true;
                    }
                    else
                    {
                        objUserRoleViewModel.IsSelected = false;
                    }
                    model.Add(objUserRoleViewModel);
                    }
            
            return View(model);

        }
    }
}
