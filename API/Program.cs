using System.Net.WebSockets;
using System.Net;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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

            // app.UseAuthorization();

            WebSocketOptions webSocketOptions = new()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120), // default
            };
            webSocketOptions.AllowedOrigins.Add("http://localhost:44438");
            webSocketOptions.AllowedOrigins.Add("https://localhost:7201");
            webSocketOptions.AllowedOrigins.Add("http://localhost:5201");
            app.UseWebSockets(webSocketOptions);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}