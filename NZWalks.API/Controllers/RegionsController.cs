using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //GET : https://localhost:portnumber/api/regions
        [HttpGet]
        public IActionResult GetAll()
        {
            //Get data from db - domain models
            var regions = dbContext.Regions.ToList();

            // Map domain to DTOs
            var regionsDto = new List<RegionDto>();
            foreach (var region in regions)
            {
                regionsDto.Add(new RegionDto { Id = region.Id, Name = region.Name, Code = region.Code, RegionImageUrl = region.RegionImageUrl });

            }

            //Return DTO.
            return Ok(regionsDto);
        }

        // GET single region by id
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute]Guid id) 
        {
            //Get region model from database
            var region = dbContext.Regions.FirstOrDefault(r => r.Id == id);
            //Autre possibilité. Find prend la clé primaire uniquement.
            //var region = dbContext.Regions.Find(id);
            if(region  == null)
            {
                return NotFound();
            }
            //Map to dto
            var regionDto = new RegionDto { Id = region.Id, Name = region.Name, Code = region.Code, RegionImageUrl = region.RegionImageUrl };
            return Ok(regionDto);
        }
    }
}
