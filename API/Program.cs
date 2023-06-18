//CSIP6833 (2023)
//Nkuna N.S (2016134773)
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

//configure the Http Request Pipline

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
