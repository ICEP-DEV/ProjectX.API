using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Service;



var builder = WebApplication.CreateBuilder(args);

// Add session services
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add services to the container.
builder.Services.AddDbContext<AlumniDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("AlumniDb")));

builder.Services.AddControllers();
builder.Services.AddCors(option => option.AddPolicy("corspolicy", builder =>
{
    //builder.AllowAnyOrigin()
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();

}));
builder.Services.AddScoped<AlumniDbContext, AlumniDbContext>();
builder.Services.AddScoped<IAlumnusService, AlumnusService>();// Register AlumnusService
builder.Services.AddScoped<IGuestService, GuestService>();// Register AlumnusService

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("corspolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession(); //use sessions

app.UseAuthorization();

app.MapControllers();

app.Run();
