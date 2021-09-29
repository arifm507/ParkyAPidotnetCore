using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace ParkyWeb.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;
        public TrailsController(ITrailRepository trailRepo, INationalParkRepository npRepo)
        {
            _trailRepo = trailRepo;
            _npRepo = npRepo;
        }
        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailApiPath) });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkApiPath);

            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i=> new SelectListItem { 
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                return View(objVM);
            }

            objVM.Trail = await _trailRepo.GetAsync(SD.TrailApiPath, id.GetValueOrDefault());

            if (objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM.Trail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {
                
                if (obj.Trail.Id == 0)
                {
                    await _trailRepo.CreateAsync(SD.TrailApiPath, obj.Trail);
                }
                else
                {
                    await _trailRepo.UpdateAsync(SD.TrailApiPath + obj.Trail.Id, obj.Trail);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }

        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepo.DeleteAsync(SD.TrailApiPath, id);

            if (status)
            {
                return Json(new { success = true, message = "Delete Successfull." });
            }

            return Json(new { success = false, message = "Delete Not Successfull." });
        }
    }
}
