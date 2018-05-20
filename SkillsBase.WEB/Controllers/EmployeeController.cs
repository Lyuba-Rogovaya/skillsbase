using Microsoft.AspNet.Identity;
using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.WEB.Models.Domain;
using SkillsBase.WEB.Models.Employees;
using SkillsBase.WEB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using SkillsBase.BLL;

namespace SkillsBase.WEB.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IService<Domain> domainService;
        private readonly IService<Employee> employeeService;
        public EmployeeController(IService<Domain> domainService, IService<Employee> empService)
        {
            if (domainService == null)
                throw new ArgumentNullException(nameof(domainService));

            if (empService == null)
                throw new ArgumentNullException(nameof(empService));

            this.domainService = domainService;
            this.employeeService = empService;
        }

        [Route("employees")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateEmployeeSkill(EmployeeSkillDTO model)
        {
            if (employeeService is IEmployeeService serv)
            {
                try
                {
                    await serv.SaveSkillLevel(model, User.Identity.GetUserId());
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
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
        [Route("employees/bank")]
        public async Task<ActionResult> Bank()
        {
            IEnumerable<Domain> domains = await domainService.ListAllAsync();
            ViewBag.Domains = new List<SelectListItem>();
            ViewBag.Skills = new List<SelectListItem>();
            ViewBag.Domains = domains.Select(d => new SelectListItem() { Text = d.Name, Value = d.Id.ToString() }).ToList();

            if (domains.Count() > 0)
            {
                if (domainService is IDomainService serv)
                {
                    IEnumerable<Skill> skills = await serv.GetSkills(domains.FirstOrDefault().Id);
                    ViewBag.Skills = skills.Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            return View("SkillsBank", new List<EmployeeSkillViewModel>());
        }


        [HttpPost]
        public async Task<ActionResult> GetSkillsByDomainId(int domainId)
        {
            if(domainId <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            IEnumerable<SelectListItem> result = null;
            if (domainService is IDomainService serv)
            {
                try
                {
                    IEnumerable<Skill> skills = await serv.GetSkills(domainId);
                    result = skills.Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
                    return Json(result);
                }
                catch (ArgumentException ex)
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
        public async Task<ActionResult> GetEmployeesBySkillId(int domainId, int skillId, int? level)
        {
            if (domainId <= 0 || skillId <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            IEnumerable<EmployeeSkillViewModel> result = null;
            if (employeeService is IEmployeeService serv)
            {
                try
                {
                    SkillLevel[] levelArray = (level == null) ? new[] { SkillLevel.Biginner, SkillLevel.Intermediate, SkillLevel.Advanced, SkillLevel.Expert } : new[] { (SkillLevel)level };
                    IEnumerable<EmployeeSkillDTO> employees = await serv.FindEmployeesBySkill(domainId, skillId, levelArray);
                    result = employees.Select(e => new EmployeeSkillViewModel() { DomainId = e.DomainId, Level = e.Level.ToString(), SkillId = e.SkillId, Employee = e.Employee });
                    return Json(result);
                } catch(ArgumentException ex)
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
        public async Task<ActionResult> DataGrid(DataTableSearchModel model)
        {
            if(model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GridSearchRestltDTO<Employee> data =
                await employeeService.FindByCriteriaAsync(model.Search.Value, model.Start, model.Length);

            IEnumerable<EmployeeViewModel> items = null;
            if (data.Items != null)
            {
                items = data.Items.Select(e => new EmployeeViewModel { Id = e.Id, UserName = e.UserName, Age = e.Age, Email = e.Email, FullName = e.FullName, PhoneNumber = e.PhoneNumber, Profession = e.Profession });
            } else
            {
                items = new List<EmployeeViewModel>();
            }
           
            DataTableModel<EmployeeViewModel> result = new DataTableModel<EmployeeViewModel>
            {
                Data = items,
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
                employeeService.Dispose();
                domainService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}