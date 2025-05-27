using Microsoft.AspNetCore.Mvc;
using APBD12.DTOs;
namespace APBD12.Services;

public interface IDbService
{
    public Task<WycieczkaDTO> GetTrips(int page , int pageSize);
    Task DeleteClient(int id, CancellationToken cancellationToken);
    Task CreateClient(int idTrip, NewClientDTO clientDto, CancellationToken cancellationToken);
}