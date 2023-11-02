using System.Diagnostics;
using System.Text.Json;
using ArgoStore.Benchmarks.Models;

namespace ArgoStore.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        bool codeGen = args.Any(x => "codeGen".Equals(x, StringComparison.OrdinalIgnoreCase));

        Person[] testData = Person.GetTestData();

        string[] jsons = testData.Select(x => JsonSerializer.Serialize(x)).ToArray();

        Stopwatch sw = Stopwatch.StartNew();

        JsonSerializerOptions options = new JsonSerializerOptions();
        var argoTypeInfoResolver = new ArgoTypeInfoResolver();
        options.TypeInfoResolver = argoTypeInfoResolver;

        if (codeGen)
        {
            Console.WriteLine("Using codeGen");
            argoTypeInfoResolver.Register(PersonSerializationContext.Default.Person);
        }
        else
        {
            Console.WriteLine("Not using codeGen");
        }

        for (int j = 0; j < 1000; j++)
        {
            for (int i = 0; i < jsons.Length; i++)
            {
                JsonSerializer.Deserialize<Person>(jsons[i], options);
            }
        }

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }
}