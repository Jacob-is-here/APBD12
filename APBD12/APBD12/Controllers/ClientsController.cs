using System.ComponentModel.DataAnnotations;
using APBD_example_test1_2025.Exceptions;
using APBD12.Models;
using APBD12.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD12.DTOs;


[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{

    private readonly IDbService _dbService;

    public ClientsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    
    [HttpDelete("{idClient:int}")]
    public async Task<IActionResult> Delete(int idClient, CancellationToken cancellationToken)
    {
        try
        {
            _dbService.DeleteClient(idClient, cancellationToken);
            return NoContent();
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