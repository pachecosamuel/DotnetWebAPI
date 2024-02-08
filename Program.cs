using Microsoft.AspNetCore.Mvc;

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

app.MapPost("/SaveProduct", (Product product) => {
    ProductRepository.AddProduct(product);
    return product;
});

// api.com/getproduct/startDate={date}&endDate={date}
app.MapGet("/getproduct", ([FromQuery]string startDate, [FromQuery] string endDate) => {
    return startDate + " " + endDate;
});

// api.com/getproduct/{code}
app.MapGet("getproduct/{code}", ([FromRoute]string code) => {
    var product = ProductRepository.GetBy(code);
    return product;
});

app.MapGet("getproductbyheader", (HttpRequest rqt) => {
    return rqt.Headers["product-code"].ToString();
});

app.Run();

public static class ProductRepository {
    public static List<Product> Products { get; set; }

    public static void AddProduct(Product product){
        if(Products == null)
            Products = [];

        Products.Add(product);
    }

    public static Product GetBy(string code){
        return Products.FirstOrDefault(p => p.Code == code);
    }
}

public class Product {
    public string Code { get; set; }
    public string Nome { get; set; }
    public float  Price { get; set; }
}