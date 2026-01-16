namespace Hotel.Infrastructure.Entities;

public class Room
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;
    public string Type { get; set; } = "Standard";
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}