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
    public class RolesController : Controller
    {
        private IService<Role> roleService;
        public RolesController(IService<Role> roleService)
        {
            this.roleService = roleService;
        }
        [Route("roles")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DataGrid(DataTableSearchModel model)
        {
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GridSearchRestltDTO<Role> data =
                await roleService.FindByCriteriaAsync(model.Search.Value, model.Start, model.Length);

            DataTableModel<Role> result = new DataTableModel<Role>
            {
                Data = data.Items,
                DecordsFiltered = data.FilteredResultsCount,
                RecordsTotal = data.TotalResultsCount,
                Draw = model.Draw
            };
            return new JsonCustomResult(result);
        }
        public ActionResult Create()
        {
            return View("Create", new RoleViewModel());
        }

        public async Task<ActionResult> Details(string id)
        {
            Role model = await roleService.FindAsync(id);
            if(model == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View("View", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Role model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await roleService.SaveAsync(model);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (RoleManagementException ex)
                {
                    return Json(new { Error = ex.Message });
                }
                catch(Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return View("Create", model);
            }
        }

        public async Task<ActionResult> Delete(string Id)
        {
            if (String.IsNullOrEmpty(Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Role role = await roleService.FindAsync(Id);

            if (role == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            RoleViewModel model = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteConfirm(Role role)
        {
            try
            {
                await roleService.DeleteAsync(role);
            }
            catch (RoleManagementException ex)
            {
                return Json(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                roleService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}