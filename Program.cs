using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>();

var cfg = builder.Configuration;
ProductRepository.InitMyCfg(cfg);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Oi ^^");

app.MapPost("/user", () => new { Nome = "Samuel", Age = 26 });

app.MapGet("/AddHeader", (HttpResponse response) =>
{
    response.Headers.Append("Teste", "Samuca");
    return new { Nome = "Samuca" };
});

app.MapPost("/products", (Product product) =>
{
    ProductRepository.AddProduct(product);
    return $"{Results.Created()}";
});

// api.com/getproduct/startDate={date}&endDate={date}
app.MapGet("/products", ([FromQuery] string startDate, [FromQuery] string endDate) =>
{
    return startDate + " " + endDate;
});

// api.com/getproduct/{code}
app.MapGet("products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);

    if (product is Product)
        return Results.Ok(product);

    return Results.BadRequest();
});

app.MapGet("getproductbyheader", (HttpRequest rqt) =>
{
    return rqt.Headers["product-code"].ToString();
});

app.MapPut("/products", (Product product) =>
{
    var savedProduct = ProductRepository.GetBy(product.Code);
    savedProduct.Nome = product.Nome;
    return Results.Ok();
});

app.MapDelete("/products", (string code) =>
{
    ProductRepository.DeleteProductById(code);
    return Results.Ok();
});

if (app.Environment.IsDevelopment())
    app.MapGet("configuration", (IConfiguration cfg) =>
    {
        return Results.Ok($"Cfg connection: {cfg["Database:Connection"]} - Cfg port: {cfg["database:port"]}");
    });

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = [];

    public static void InitMyCfg(IConfiguration cfg)
    {
        var products = cfg.GetSection("Products").Get<List<Product>>();
        
        if (products is not null)
            Products = products;
    }     

    public static void AddProduct(Product product)
    {
        Products.Add(product);
    }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }


    public static void DeleteProduct(Product product)
    {
        Products.Remove(product);
    }

    public static void DeleteProductById(string code)
    {
        var savedProduct = GetBy(code);
        Products.Remove(savedProduct);
    }
}

public class Product{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Nome { get; set; }
    public float Price { get; set; }

    public string Description { get; set; }
}

public class ApplicationDbContext : DbContext
{

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Substitua 'NomeDoBancoDeDados' pelo nome do seu banco de dados
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Products;Username=postgres;Password=p0stgr3s");
        }
    }
}