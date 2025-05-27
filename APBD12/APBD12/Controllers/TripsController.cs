using System.ComponentModel.DataAnnotations;
using APBD_example_test1_2025.Exceptions;
using APBD12.DTOs;
using APBD12.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APBD12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private IDbService _dbService;

    public TripsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, int pageSize = 10)
    {
        var wycieczki = await _dbService.GetTrips(page, pageSize);
        return Ok(wycieczki);
    }

    [HttpPost("{idTrip:int}/clients")]
    public async Task<IActionResult> newClient([FromBody] NewClientDTO clientDto, [FromRoute] int idTrip, CancellationToken cancellationToken)
    {
        try
        {
            await _dbService.CreateClient(idTrip, clientDto, cancellationToken);
            return Created();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ConflictException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    
    
    
}