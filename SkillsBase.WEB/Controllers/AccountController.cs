using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using SkillsBase.BLL.DTO;
using System.Security.Claims;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Exceptions;
using System;
using System.Data.SqlClient;
using System.Net;
using SkillsBase.WEB.Models.Account;
using Microsoft.AspNet.Identity;
using SkillsBase.BLL.Models;
using System.Linq;
using SkillsBase.WEB.Models.Domain;

namespace SkillsBase.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private readonly IService<Employee> employeeService;
        public AccountController(IService<Employee> employeeService, IAccountService accountService)
        {
            if (employeeService == null)
                throw new ArgumentNullException(nameof(employeeService));

            if (accountService == null)
                throw new ArgumentNullException(nameof(accountService));

            this.employeeService = employeeService;
            this.accountService = accountService;
        }
        public ActionResult Login()
        {
            return View();
        }

        public async Task<ActionResult> Skills()
        {
            IEnumerable<EmployeeSkillDTO> model = null;
            if (employeeService is IEmployeeService serv)
            {
                model = await serv.GetEmployeeSkills(User.Identity.GetUserId());
                if (model != null)
                {
                    ViewBag.Domains = model.GroupBy(d => d.DomainId).Select(d => new DomainViewModel() { Id = d.First().DomainId, Name = d.First().DomainName });
                } else
                {
                    model = new List<EmployeeSkillDTO>();
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return View("Skills", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO { Email = model.Email, Password = model.Password };
                ClaimsIdentity claim = null;
                try
                {
                    claim = await accountService.Authenticate(userDto);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.FromUnixTimeSeconds(1200),
                        IsPersistent = true
                    }, claim);
                    return RedirectToAction("profile", "account");
                }
                catch (AccountException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    UserName = model.Name,
                    Role = "user"
                };
                try
                {
                    await accountService.Create(userDto);
                }
                catch (AccountException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

                return RedirectToAction("profile", "account");
            }
            return View(model);
        }
        [Authorize]
        [Route("account/profile")]
        public async Task<ActionResult> MyProfile()
        {
            UserProfileModel model = null;
            try
            {
                ProfileDTO dto = await accountService.GetProfile(User.Identity.GetUserId());
                if (dto != null)
                {
                    model = new UserProfileModel { Id = dto.Id, Age = dto.Age, FirstName = dto.FirstName, LastName = dto.LastName, Profession = dto.Profession };
                }
                else
                {
                    model = new UserProfileModel();
                }
                
            } catch(Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            return View("Profile", model);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(UserProfileModel model)
        {
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            if (ModelState.IsValid)
            {
                try
                {
                    ProfileDTO dto = new ProfileDTO { Id = model.Id, FirstName = model.FirstName, LastName = model.LastName, Profession = model.Profession, Age = model.Age };
                    await accountService.UpdateProfile(dto);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return View("Profile", model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                employeeService.Dispose();
                accountService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}