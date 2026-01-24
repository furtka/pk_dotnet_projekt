namespace Hotel.Application.Domain.Models;

public class AvailabilityFilter
{
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int? MinCapacity { get; set; }
}
