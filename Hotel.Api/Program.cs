using Hotel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Hotel.Application.Domain.Repositories;
using Hotel.Application.UseCases.Guest;
using Hotel.Infrastructure.Repositories;
using Hotel.Application.UseCases.Rooms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("HotelDb"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddControllers();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<GetRoomsUseCase>();
builder.Services.AddScoped<GetRoomByIdUseCase>();
builder.Services.AddScoped<CreateRoomUseCase>();
builder.Services.AddScoped<UpdateRoomUseCase>();
builder.Services.AddScoped<DeleteRoomUseCase>();

builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<GetGuestsUseCase>();
builder.Services.AddScoped<CreateGuestUseCase>();
builder.Services.AddScoped<GetGuestByIdUseCase>();
builder.Services.AddScoped<UpdateGuestUseCase>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<GetReservationByIdUseCase>();
builder.Services.AddScoped<CreateReservationUseCase>();
builder.Services.AddScoped<RemoveReservationUseCase>();

builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
builder.Services.AddScoped<GetAvailableRoomsUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();