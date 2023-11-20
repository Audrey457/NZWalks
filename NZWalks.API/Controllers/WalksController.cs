using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // /api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        // Create walk
        // POST : /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            // Map DTO to domain model
            var walk = mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(walk);
            //Map domain model to DTO
            return Ok(mapper.Map<WalkDto>(walk));
        }

        //GET Walks
        //GET: /api/walks?filterOn=Name&filterQuery=Track
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            var walks = await walkRepository.GetAllAsync(filterOn, filterQuery);
            //Map domain model to DTO
            return Ok(mapper.Map<List<WalkDto>>(walks));

        }

        //Get walk by id
        //GET: /api/Walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await walkRepository.GetByIdAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            //Map domain model to DTO
            return Ok(mapper.Map<WalkDto>(walk));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            var updatedWalk = await walkRepository.UpdateAsync(id, mapper.Map<Walk>(updateWalkRequestDto));
            if (updatedWalk == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDto>(updatedWalk));
        }

        // Delete a walk by id
        // DELETE: /api/Walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedWalk = await walkRepository.DeleteAsync(id);
            if (deletedWalk == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<WalkDto>(deletedWalk));
        }
    }
}
