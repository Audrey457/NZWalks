using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    // https://localhost:portnumber/api/regions
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper, 
            ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        //GET : https://localhost:portnumber/api/regions
        [HttpGet]
        [MapToApiVersion("1.0")]
        //[Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll()
        {
            //Get data from db - domain models
            var regions = await regionRepository.GetAllAsync();

            logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");

            //Return DTO.
            return Ok(mapper.Map<List<RegionDto>>(regions));
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetAllV2()
        {
            //Get data from db - domain models
            var regions = await regionRepository.GetAllAsync();

            logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");

            //Return DTO.
            return Ok(mapper.Map<List<RegionDtoV2>>(regions));
        }

        // GET single region by id
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        [MapToApiVersion("1.0")]
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
        [Authorize(Roles = "Writer")]
        [ValidateModel]
        [MapToApiVersion("1.0")]
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
        [Authorize(Roles = "Writer")]
        [Route("{id:Guid}")]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //Map dto to domain model
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null) { return NotFound(); }

            //Convert domain model to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDto);
        }

        // Delete region
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Authorize(Roles = "Writer")]
        [Route("{id:Guid}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteAsync(id);
            if(region == null) { return NotFound(); }
            return Ok(mapper.Map<RegionDto>(region));
        }
    }
}
