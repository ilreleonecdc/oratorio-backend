using oratorio_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CORS (se ti serve per il dev su IP locale)
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
  p.WithOrigins("http://localhost:4200", "http://192.168.1.97:4200")  // o http://<IP>:4200
    .AllowAnyHeader()
    .AllowAnyMethod()
  ));

builder.Services.AddHttpClient<EmailService>();
builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
