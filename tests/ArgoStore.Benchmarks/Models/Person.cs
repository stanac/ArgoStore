namespace ArgoStore.Benchmarks.Models;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public int YearOfBirth { get; set; }
    public bool Active { get; set; }
    public bool IsAdmin { get; set; }
    public int CookiesCount { get; set; }
    public DateTime RegistrationDate { get; set; }

    public static Person[] GetTestData()
    {
        return TestData.GetTestData().ToArray();
    }
}

file static class TestData
{
    private static readonly string[] _names = @"Lula Floyd
Colin Blair
Harvey Hunt
Bertha Steele
Orville Murray
Emilio Diaz
Evelyn Myers
Guy Moody
Paul Evans
Freda Hunter
Kenneth Summers
Jeremy Perkins
Tara Stanley
Joe Wheeler
Ramona Taylor
Jaime Knight
Emanuel Haynes
Devin Doyle
Chad Brooks
Marcus Collier
Aubrey Roberts
Edna Simmons
Tracey Carson
Spencer Colon
Patrick Sanders
Christian Gross
Jennifer Andrews
Dwight Greene
Herman Bishop
Vickie Benson
Cristina Tucker
Eileen Cohen
Ross Malone
Eddie Valdez
Kelley Pena
Warren Reid
Wilson Strickland
Alvin King
Raymond Guzman
Ricky Hoffman
Benjamin Harrison
Becky Copeland
Glenn Mann
Leland Sharp
Rodney Hamilton
Don Dennis
Audrey Maxwell
Phil Ortiz
Frances Flores
Lamar Williams
Linda Brewer
Tommie Fields
Bonnie Carlson
Felix Gonzalez
Marta Lynch
Nathaniel Shelton
Johnnie Harvey
Emmett Mcgee
Darrel Lane
Isaac Daniels
Rudolph Kelley
Ella Jimenez
Damon Rios
Kathryn May
Carroll Paul
Fred Vega
Alfred Cannon
Doreen Patton
Duane Bailey
Shane Sanchez
Ken Sutton
Joshua Vasquez
Naomi Pearson
Wallace Barnett
Shelly Hunt
Maggie Carter
Santiago French
Antoinette Huff
Malcolm Matthews
Leonard Roy
Kristen O'brien
Rosalie Norman
Kayla Fitzgerald
Brandon Conner
Troy Jackson
Frederick Martinez
Katie Banks
Theresa Hudson
Shannon Holloway
Tracy Ross
Dennis Wells
Ray Beck
Roman Singleton
Jenny Brady
Gerardo Wright
Randy Stevenson
Horace Johnston
Lydia Klein
Candice Cole
Hugh Cross".Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();

    public static IEnumerable<Person> GetTestData()
    {
        for (int j = 0; j < 100; j++) { 
            for (int i = 0; i < _names.Length; i++)
            {
                yield return new Person
                {
                    Active = i % 2 == 0,
                    Name = _names[i],
                    EmailAddress = GetEmailAddress(_names[i], j),
                    CookiesCount = i,
                    Id = GetIdGuid(i, j),
                    IsAdmin = i % 5 > 0,
                    RegistrationDate = GetRegistrationTime(i),
                    YearOfBirth = GetRegistrationTime(i).Year
                };
            }
        }
    }

    private static string GetEmailAddress(string name, int iteration)
    {
        name = string.Join(".", name.Split(' ').Select(x => x.ToLower()));

        return $"{name}-{iteration}@example.com";
    }

    private static int? GetBirthYear(int index)
    {
        DateTime dt = new DateTime(2020, 11, 1);

        if (index % 3 == 0)
        {
            return null;
        }

        return dt.AddMonths(-index).Year;
    }

    private static DateTime? GetCakeDay(int index)
    {
        DateTime dt = new DateTime(2022, 11, 1).ToLocalTime();

        if (index % 2 == 0)
        {
            return dt.AddDays(-index);
        }

        return null;
    }

    private static Guid GetIdGuid(int index, int iteration)
    {
        string g = "00a6d8c9-5f3c-42d3-a5f3-cccc110f10b5";
        string hex1 = index.ToString("X");
        if (hex1.Length == 1) hex1 = "0" + hex1;

        string hex2 = iteration.ToString("X");

        while (hex2.Length < 4)
        {
            hex2 = "0" + hex2;
        }

        g = g.Replace("00", hex1);
        g = g.Replace("cccc", hex2);

        return Guid.Parse(g);
    }

    private static DateTime GetRegistrationTime(int index)
    {
        DateTime dt = new DateTime(2022, 10, 1);

        return dt.AddDays(-index);
    }

}