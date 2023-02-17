using AspNetCoreExample.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace AspNetCoreExample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        string dbConnectionString = builder.Configuration.GetConnectionString("db")
            ?? throw new InvalidOperationException("`ConnectionStrings:db` not set");

        builder.Services.AddArgoStore(c =>
        {
            c.ConnectionString(dbConnectionString);
            c.RegisterDocument<Person>(p =>
            {
                p.PrimaryKey(x => x.Id);
                p.TableName("Persons");
                p.UniqueIndex(x => x.Name);
                p.UniqueIndex(x => new {x.Name, x.CookiesCount});
                p.NonUniqueIndex(x => x.Name);
                p.NonUniqueIndex(x => new { x.Name, x.CookiesCount });
            });
        });

        WebApplication app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}
