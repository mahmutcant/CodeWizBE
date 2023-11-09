using CodeWizBE;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<AppDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("MssqlDb")));
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddMvc().AddControllersAsServices();
builder.Services.AddCors(options =>
options.AddDefaultPolicy(policy =>
policy.WithOrigins("https://0.0.0.0:7072", "http://0.0.0.0:5072").AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseAuthentication();
/* Configure */


string localIP = LocalIPAddress();
app.Urls.Add("http://" + "127.0.0.1" + ":5072");
app.Urls.Add("https://" + "127.0.0.1" + ":7072");
app.Urls.Add("http://" + "0.0.0.0" + ":5072");
app.Urls.Add("https://" + "0.0.0.0" + ":7072");

app.UseCors();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
static string LocalIPAddress()
{
    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
    {
        socket.Connect("8.8.8.8", 65530);
        IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
        if (endPoint != null)
        {
            return endPoint.Address.ToString();
        }
        else
        {
            return "127.0.0.1";
        }
    }
}