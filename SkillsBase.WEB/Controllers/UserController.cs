using Microsoft.AspNet.Identity;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Exceptions;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.WEB.Models.Roles;
using SkillsBase.WEB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SkillsBase.WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly IService<User> userService;
        private readonly IService<Role> roleService;
        public UserController(IService<User> userService, IService<Role> roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }
        [Route("users")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            if(userId == null)
                return Json(new { Error = "User id is undefined." });
            if (roleName == null)
                return Json(new { Error = "Role name is undefined." });

            if (userService is IUserService serv)
            {
                try
                {
                    await serv.RemoveUserFromRoleAsync(roleName, userId);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (UserManagementException ex)
                {
                    return Json(new { Error = ex.Message });
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToRole(string userId, string roleName)
        {
            if (userId == null)
                return Json(new { Error = "User id is undefined." });
            if (roleName == null)
                return Json(new { Error = "Role name is undefined." });

            if (userService is IUserService serv)
            {
                try
                {
                    await serv.AddUserToRoleAsync(roleName, userId);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (UserManagementException ex)
                {
                    return Json(new { Error = ex.Message });
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [Route("user/{id}/roles")]
        public async Task<ActionResult> Roles(string id)
        {
            User user = await userService.FindAsync(id);

            if(user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            ViewBag.UserName = user.UserName;
            ViewBag.UserId = user.Id;

            IEnumerable<RoleViewModel> model = null;
            if(userService is IUserService serv)
            {
                IEnumerable<Role> roles = await serv.GetUserRolesAsync(id);
                model = roles.Select(r => new RoleViewModel() { Id = r.Id, Name = r.Name});
            }

            IEnumerable<Role> allRoles = await roleService.ListAllAsync();

            ViewBag.AllRoles = allRoles.Select(r => new RoleViewModel() { Id = r.Id, Name = r.Name });

            if (model == null)
                model = new List<RoleViewModel>();

            return View("UserRoles", model);
        }
        [HttpPost]
        public async Task<ActionResult> DataGrid(DataTableSearchModel model)
        {
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GridSearchRestltDTO<User> data =
                await userService.FindByCriteriaAsync(model.Search.Value, model.Start, model.Length);

            DataTableModel<User> result = new DataTableModel<User>
            {
                Data = data.Items,
                DecordsFiltered = data.FilteredResultsCount,
                RecordsTotal = data.TotalResultsCount,
                Draw = model.Draw
            };
            return new JsonCustomResult(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userService.Dispose();
                roleService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}