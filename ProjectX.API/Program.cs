using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Service;
using TUTDb.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AlumniDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("AlumniDb")));

//tut database
builder.Services.AddDbContext<TUTDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("TUTDb")));

builder.Services.AddControllers();
builder.Services.AddCors(option => option.AddPolicy("corspolicy", builder =>
{
    //builder.AllowAnyOrigin()
    builder.WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader();

}));

builder.Services.AddScoped<AlumnusService>();// Register AlumnusService


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

app.UseAuthorization();

app.MapControllers();

app.Run();
