namespace APBD12.DTOs;

public class WycieczkaDTO
{
    public int PageNum { get; set; }
    public int PageSize { get; set; } = 10;
    public int AllPages { get; set; }
    public List<TripDTO> Trips { get; set; }
}

public class TripDTO
{
    public String Name { get; set; }
    public String Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<String> Countries { get; set; }
    public List<CLientDTO> Clients { get; set; }
}


public class CLientDTO
{
    public String FirstName { get; set; }
    public String LastName { get; set; }
}