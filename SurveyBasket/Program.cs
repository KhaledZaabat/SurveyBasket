
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
//builder.Services.AddAppServices();

builder.Services.AddAutomaticAppServices();
//After Here you Can Override



var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


