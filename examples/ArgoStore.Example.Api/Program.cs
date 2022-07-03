using ArgoStore.Example.Api.Models;

namespace ArgoStore.Example.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddArgoStore(config =>
            {
                string connectionString = builder.Configuration.GetConnectionString("db");
                
                config.ConnectionString(connectionString);

                // This will create table whenever call to new type of entity is performed
                // e.g. Query<Pet> will create table for Pet entity if not exists
                config.CreateNotConfiguredEntities(true);

                config.Entity<Person>()
                    .PrimaryKey(x => x.Id) // this line is optional, check docs for more info
                    .NonUniqueIndex(x => x.Name)
                    .NonUniqueIndex(x => new {x.Name, x.Age})
                    .UniqueIndex(x => x.EmailAddress)
                    .UniqueIndex(x => new {x.BirthYear, x.EmailAddress});
            });

            // OR
            // builder.Services.AddArgoStore(connectionString);
            // it will set CreateNotConfiguredEntities to true

            WebApplication app = builder.Build();

            SeedData(app.Services.GetRequiredService<DocumentStore>());

            app.UseAuthorization();
            app.MapControllers();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.Run();
        }

        private static void SeedData(DocumentStore store)
        {
            IDocumentSession s = store.CreateSession();
            
            s.InsertOrUpdate(new Person
                {
                    Id = Guid.Parse("b22d7560-8657-4b7b-839c-7f9cb88b68f3"),
                    Name = "Julie Markson",
                    Age = 48,
                    BirthYear = 888,
                    EmailAddress = "marsj@example.com"
                },
                new Person
                {
                    Id = Guid.Parse("61a10887-7727-4fd7-9021-b6b269504acf"),
                    Name = "Mark Markson",
                    Age = 48,
                    BirthYear = 888,
                    EmailAddress = "mare@example.com"
                });

            s.SaveChanges();
        }
    }
}