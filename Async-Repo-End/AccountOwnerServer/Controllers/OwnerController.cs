using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountOwnerServer.Controllers
{
    [Route("api/owner")] 
    [ApiController] 
    public class OwnerController : ControllerBase 
    { 
        private ILoggerManager _logger; 
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        
        public OwnerController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper) 
        { 
            _logger = logger; 
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet] 
        public async Task<IActionResult> GetAllOwners() 
        { 
            try 
            { 
                var owners = await _repository.Owner.GetAllOwnersAsync(); 
                _logger.LogInfo($"Returned all owners from database.");

                var ownersResult = _mapper.Map<IEnumerable<OwnerDto>>(owners);
                return Ok(ownersResult); 
            } 
            catch (Exception ex) 
            { 
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}"); 
                return StatusCode(500, "Internal server error"); 
            } 
        }

        [HttpGet("{id}", Name = "OwnerById")] 
        public async Task<IActionResult> GetOwnerById(Guid id) 
        { 
            try 
            { 
                var owner = await _repository.Owner.GetOwnerByIdAsync(id); 
                if (owner == null) 
                { 
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db."); 
                    return NotFound(); 
                } 
                else 
                { 
                    _logger.LogInfo($"Returned owner with id: {id}");

                    var ownerResult = _mapper.Map<OwnerDto>(owner);
                    return Ok(ownerResult); 
                } 
            } 
            catch (Exception ex) 
            { 
                _logger.LogError($"Something went wrong inside GetOwnerById action: {ex.Message}"); 
                return StatusCode(500, "Internal server error"); 
            } 
        }

        [HttpGet("{id}/account")] 
        public async Task<IActionResult> GetOwnerWithDetails(Guid id) 
        { 
            try 
            { 
                var owner = await _repository.Owner.GetOwnerWithDetailsAsync(id); 
                if (owner == null) 
                { 
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db."); 
                    return NotFound(); 
                } 
                else 
                { 
                    _logger.LogInfo($"Returned owner with details for id: {id}");

                    var ownerResult = _mapper.Map<OwnerDto>(owner);
                    return Ok(ownerResult); 
                } 
            } 
            catch (Exception ex) 
            { 
                _logger.LogError($"Something went wrong inside GetOwnerWithDetails action: {ex.Message}"); 
                return StatusCode(500, "Internal server error"); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOwner([FromBody]OwnerForCreationDto owner)
        {
            try
            {
                if (owner == null)
                {
                    _logger.LogError("Owner object sent from client is null.");
                    return BadRequest("Owner object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid owner object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var ownerEntity = _mapper.Map<Owner>(owner);

                _repository.Owner.CreateOwner(ownerEntity);
                await _repository.SaveAsync();

                var createdOwner = _mapper.Map<OwnerDto>(ownerEntity);

                return CreatedAtRoute("OwnerById", new { id = createdOwner.Id }, createdOwner);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOwner(Guid id, [FromBody]OwnerForUpdateDto owner)
        {
            try
            {
                if (owner == null)
                {
                    _logger.LogError("Owner object sent from client is null.");
                    return BadRequest("Owner object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid owner object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var ownerEntity = await _repository.Owner.GetOwnerByIdAsync(id);
                if (ownerEntity == null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _mapper.Map(owner, ownerEntity);

                _repository.Owner.UpdateOwner(ownerEntity);
                await _repository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(Guid id)
        {
            try
            {
                var owner = await _repository.Owner.GetOwnerByIdAsync(id);
                if (owner == null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                if (_repository.Account.AccountsByOwner(id).Any()) 
                {
                    _logger.LogError($"Cannot delete owner with id: {id}. It has related accounts. Delete those accounts first"); 
                    return BadRequest("Cannot delete owner. It has related accounts. Delete those accounts first"); 
                }

                _repository.Owner.DeleteOwner(owner);
                await _repository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}