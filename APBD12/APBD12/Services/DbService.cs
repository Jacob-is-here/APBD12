using APBD_example_test1_2025.Exceptions;
using APBD12.DTOs;
using APBD12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbContext = APBD12.Models.DbContext;

namespace APBD12.Services;

public class DbService : IDbService
{
    private readonly DbContext _context;
    
    public DbService(DbContext context)
    {
        _context = context;
    }

    public async Task<WycieczkaDTO> GetTrips(int page , int pageSize = 10)
    {
        var skip = (page - 1);
        var totalTrips = _context.Trips.Count()*pageSize;
        var liczbastr = pageSize / 10;
        var wycieczki = new WycieczkaDTO
            {
                PageNum = page,
                PageSize = pageSize,
                AllPages = totalTrips,
                Trips = _context.Trips.OrderBy(g => g.DateFrom)
                    .Skip(skip)
                    .Take(liczbastr).Select(t => new TripDTO
                    {
                        Name = t.Name,
                        Description = t.Description,
                        DateFrom = t.DateFrom,
                        DateTo = t.DateTo,
                        MaxPeople = t.MaxPeople,
                        Countries = t.IdCountries.Select(c => c.Name).ToList(),
                        Clients = _context.ClientTrips.Where(w =>w.IdTrip == t.IdTrip)
                            .Select(c => new CLientDTO()
                        {
                            FirstName = _context.Clients.Where(q => q.IdClient == c.IdClient).Select(m => m.FirstName).FirstOrDefault()?? string.Empty,
                            LastName = _context.Clients.Where(q => q.IdClient == c.IdClient).Select(m => m.LastName).FirstOrDefault()?? string.Empty
                        }).ToList()
                        
                    }).ToList()
            };
        return wycieczki;
    }

    public async Task DeleteClient(int id, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(s => s.IdClient == id, cancellationToken);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {id} was not found.");
        }

        var hasTrips = await _context.ClientTrips.AnyAsync(w => w.IdClient == id, cancellationToken);
        if (hasTrips )
        {
            throw new ConflictException($"Nie można usunąć klienta {id}, ponieważ ma przypisane wycieczki.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

    }

    public async Task CreateClient(int idTrip, NewClientDTO clientDto, CancellationToken cancellationToken)
    {
        var czyIstniejeTrip = _context.Trips.Count(t => t.IdTrip == idTrip);
        if (czyIstniejeTrip == 0)
        {
            throw new ConflictException($"Wycieczka o id {idTrip} nie istnieje ");
        }

        var czyZdarzy = _context.Trips.Count(s => s.DateFrom < DateTime.Now && s.IdTrip == idTrip);
        if (czyZdarzy > 0)
        {
            throw new ConflictException("Nie możesz jechać na daną wycieczkę ponieważ już się zaczeła , bądź zakończyła");
        }

        var czyPoprawnaNazwa = _context.Trips.Count(t => t.IdTrip == idTrip && t.Name == clientDto.TripName);
        if (czyPoprawnaNazwa == 0)
        {
            throw new ConflictException("Podano nie poprawno nazwa wyjazdu ");
        }
        
        var czyIstniejeKlient = _context.Clients.FirstOrDefault(s => s.Pesel == clientDto.Pesel);
        if (czyIstniejeKlient != null)
        {
            
            var alreadyAssigned = _context.ClientTrips.Any(ct => ct.IdClient == czyIstniejeKlient.IdClient && ct.IdTrip == idTrip);
            if (alreadyAssigned)
                throw new ConflictException("Klient o podanym PESEL jest już zapisany na tę wycieczkę.");

            var newClientTrip = new ClientTrip
            {
                IdClient = _context.Clients.Where(s => s.Pesel == clientDto.Pesel).Select(t => t.IdClient).FirstOrDefault(),
                IdTrip = idTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = clientDto.PaymentDate
            };
            _context.ClientTrips.Add(newClientTrip);
            await _context.SaveChangesAsync(cancellationToken);
            return;
            
        }

        
        var client = new Client()
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel
        };
        await _context.Clients.AddAsync(client, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var trip = new ClientTrip()
        {
            IdClient = _context.Clients.Where(s => s.Pesel == clientDto.Pesel).Select(t => t.IdClient).FirstOrDefault(),
            IdTrip = idTrip,
            PaymentDate = clientDto.PaymentDate,
            RegisteredAt = DateTime.Now
        };

        await _context.ClientTrips.AddAsync(trip, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);


    }
}