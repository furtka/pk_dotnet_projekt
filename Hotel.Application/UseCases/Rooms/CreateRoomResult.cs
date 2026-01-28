namespace Hotel.Application.UseCases.Rooms;

public record CreateRoomResult
{
    public int? Id { get; init; }
    public bool IsSuccess { get; init; }
    public bool IsConflict { get; init; }
    
    public static CreateRoomResult Success(int id) => new() { Id = id, IsSuccess = true };
    public static CreateRoomResult Conflict() => new() { IsConflict = true, IsSuccess = false };
}
