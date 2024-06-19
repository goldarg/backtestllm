using api.DataAccess;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AccessTokenHandler>();
// builder.Services.AddHttpClient("TokenHandler")

builder.Services.AddHttpClient("CrmHttpClient", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://www.zohoapis.com/");

    httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", AccessTokenHandler.Instance.AccessToken);
});

builder.Services.AddDbContext<RdaDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IRdaUnitOfWork, RdaUnitOfWork>();
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x=>x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();