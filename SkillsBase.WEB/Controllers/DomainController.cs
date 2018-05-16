using SkillsBase.BLL.DTO;
using SkillsBase.BLL.Exceptions;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Models;
using SkillsBase.WEB.Models.Domain;
using SkillsBase.WEB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsBase.WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class DomainController : Controller
    {
        private readonly IService<Domain> domainService;
        public DomainController(IService<Domain> domService)
        {
            if (domService == null)
                throw new ArgumentNullException(nameof(domService));

            domainService = domService;
        }
        [Route("domains")]
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Create()
        {
            return View("AddEdit", new DomainViewModel());
        }
        public async Task<ActionResult> Edit(int Id)
        {
            if(Id <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            Domain dto = await domainService.FindAsync(Id);
            if(dto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            DomainViewModel model = new DomainViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, Skills = dto.Skills };
            return View("AddEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> Save(DomainViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Domain dto = new Domain { Id = model.Id, Name = model.Name, Description = model.Description, Skills = model?.Skills };
                    await domainService.SaveAsync(dto);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (DomainException ex)
                {
                    return Json(new { Error = ex.Message });
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            } else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("AddEdit", model);
            }

        }
        [HttpPost]
        public async Task<ActionResult> RemoveSkill(int skillId)
        {
            if (skillId <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (domainService is IDomainService serv)
            {
                try
                {
                    await serv.RemoveSkill(skillId);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (DomainException ex)
                {
                    return Json(new { Error = ex.Message });

                } catch(Exception ex)
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
        public async Task<ActionResult> AddSkill(Skill skill)
        {
            if (skill == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (domainService is IDomainService serv)
            {
                try
                {
                    int skillId = await serv.AddSkill(skill);
                    return Json(new { SkillId = skillId }); 
                }
                catch (DomainException ex)
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
        public async Task<JsonCustomResult> DataGrid(DataTableSearchModel model)
        {
            GridSearchRestltDTO<Domain> data =
                await domainService.FindByCriteriaAsync(model.Search.Value, model.Start, model.Length);
            IEnumerable<DomainViewModel> items = new List<DomainViewModel>();

            if (data.Items != null)
            {
                items = data.Items.Select(d => new DomainViewModel { Id = d.Id, Name = d.Name, Description = d.Description });
            }

            DataTableModel<DomainViewModel> result = new DataTableModel<DomainViewModel>
            {
                Data = items,
                DecordsFiltered = data.FilteredResultsCount,
                RecordsTotal = data.TotalResultsCount,
                Draw = model.Draw
            };
            return new JsonCustomResult(result);
        }

        public async Task<ActionResult> Delete(int Id)
        {
            if(Id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            Domain domain = await domainService.FindAsync(Id);
            if(domain == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            DomainViewModel model = new DomainViewModel { Id = domain.Id, Name = domain.Name };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteConfirm(Domain dto)
        {
            if (dto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                await domainService.DeleteAsync(dto);
            } catch(DomainException ex)
            {
                return Json(new { Error = ex.Message } );
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
                domainService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
 }