namespace Hotel.Api.Availability.Dtos;

public sealed class GetAvailabilityRequest
{
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int? MinCapacity { get; set; }
}
