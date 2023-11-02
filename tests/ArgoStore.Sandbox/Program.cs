using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArgoStore.Benchmarks.Models;
using ArgoStore.Implementations;

namespace ArgoStore.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        Person[] testData = Person.GetTestData();

        string[] jsons = testData.Select(x => JsonSerializer.Serialize(x)).ToArray();

        Stopwatch sw = Stopwatch.StartNew();

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        for (int j = 0; j < 100; j++)
        {
            for (int i = 0; i < jsons.Length; i++)
            {
                if (i == 12 && j == 3)
                {
                    ArgoStoreQueryProvider.MeasureExecutionTime = true;
                }
                else
                {
                    ArgoStoreQueryProvider.MeasureExecutionTime = false;
                }
                
                JsonSerializer.Deserialize<Person>(jsons[i]);

                if (i == 12 && j == 3)
                {
                    Console.WriteLine(ArgoStoreQueryProvider.LastActivity?.Dump());
                }
            }
        }

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }
}