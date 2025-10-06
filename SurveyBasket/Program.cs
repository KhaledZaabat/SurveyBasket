
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddAppServices();




var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


