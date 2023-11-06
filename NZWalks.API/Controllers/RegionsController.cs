using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        //GET : https://localhost:portnumber/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Get data from db - domain models
            var regions = await regionRepository.GetAllAsync();

            //Return DTO.
            return Ok(mapper.Map<List<RegionDto>>(regions));
        }

        // GET single region by id
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id) 
        {
            //Get region model from database
            var region = await regionRepository.GetByIdAsync(id);

            //Autre possibilité. Find prend la clé primaire uniquement.
            //var region = dbContext.Regions.Find(id);
            if(region  == null)
            {
                return NotFound();
            }
            
            return Ok(mapper.Map<RegionDto>(region));
        }

        // POST to create new region
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Map DTO to domain model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            //Use domain model to create region
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            // Map domain model back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        //Update region
        //PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //Map dto to domain model
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if(regionDomainModel == null) { return NotFound(); }

            //Convert domain model to DTO
            var regionDto = mapper.Map<RegionDto> (regionDomainModel);
            return Ok(regionDto);
        }

        // Delete region
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteAsync(id);
            if(region == null) { return NotFound(); }
            return Ok(mapper.Map<RegionDto>(region));
        }
    }
}
