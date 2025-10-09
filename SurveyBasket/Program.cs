using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddAppServices();

builder.Services.AddApplicationServices(builder.Configuration);
//After Here you Can Override





var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await RoleSeeder.SeedRolesAsync(services);
//} Later We add roles based

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
