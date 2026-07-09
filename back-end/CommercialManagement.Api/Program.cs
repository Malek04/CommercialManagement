using CommercialManagement.Api.Mappers;
using CommercialManagement.Core.IRepositories;
using CommercialManagement.Infrastructure.Context;
using CommercialManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderLineRepository, OrderLineRepository>();

builder.Services.AddControllers();


// CORS configuration for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Database Connection
builder.Services.AddDbContext<CommercialManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Mapping
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MappingProfile)
);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


// Enable CORS (must be before Authorization)
app.UseCors("AllowAngular");


app.UseAuthorization();

app.MapControllers();

app.Run();