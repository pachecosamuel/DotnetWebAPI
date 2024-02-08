var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", () => "Oi ^^");
app.MapPost("/user", () => new {Nome = "Samuel", Age = 26});
app.MapGet("/AddHeader", (HttpResponse response) => {
    response.Headers.Add("Teste", "Samuca");
    return new {Nome = "Samuca"};
    });

app.Run();
