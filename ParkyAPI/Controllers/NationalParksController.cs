using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;
        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get List of National Parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetNationalParks() {
            var objList = _npRepo.GetNationalParks();
            var objDto = new List<NationalParkDto>();
            foreach (var obj in objList) {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get Indivisual National Park
        /// </summary>
        /// <param name="npId"></param>
        /// <returns></returns>
        [HttpGet("{npId:int}", Name= "GetNationalPark")]
        public IActionResult GetNationalPark(int npId)
        {
            var obj = _npRepo.GetNationalPark(npId);
            if (obj == null) {
                return NotFound();
            }
            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto) {
            if (nationalParkDto == null) {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExists(nationalParkDto.Name)) {
                ModelState.AddModelError("", "National Park Exist!!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var nationalParkobj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(nationalParkobj)) {
                ModelState.AddModelError("", $"Something went wrong while saving the record {nationalParkobj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark",new { npId = nationalParkobj .Id}, nationalParkobj);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkId != nationalParkDto.Id || nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            var nationalParkobj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(nationalParkobj))
            {
                ModelState.AddModelError("", $"Something went wrong while Updating the record {nationalParkobj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }
            var nationalParkobj = _npRepo.GetNationalPark(nationalParkId);
            if (!_npRepo.DelateNationalPark(nationalParkobj))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting the record {nationalParkobj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
