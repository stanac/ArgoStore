using System;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.TestsCommon.TestData;

public static class PersonTestData
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

    public static IEnumerable<Person> GetPersonTestData()
    {
        for (int i = 0; i < _names.Length; i++)
        {
            yield return new Person
            {
                Name = GetName(_names[i]),
                EmailAddress = GetEmailAddress(_names[i]),
                BirthYear = GetBirthYear(i),
                Points = i + 3,
                NumberOfPorts = i % 5,
                OddNumberOfPorts = i % 4,
                CakeDay = GetCakeDay(i),
                EmailConfirmed = i % 2 == 0,
                Id = GetIdGuid(i),
                NickName = _names[i].Split(' ')[0],
                RegistrationTime = GetRegistrationTime(i),
                Roles = GetRoles(i),
                Type = (PersonTypes)(i % 3)
            };
        }
    }

    private static string GetName(string name)
    {
        if (name == "Shelly Hunt")
        {
            return "  Shelly Hunt  ";
        }

        if (name == "Rosalie Norman")
        {
            return "  Rosalie Norman";
        }

        if (name == "Katie Banks")
        {
            return "Katie Banks  ";
        }

        if (name == "Ramona Taylor")
        {
            return "RAMONA TAYLOR";
        }

        if (name == "Ken Sutton")
        {
            return "ken sutton".ToLower();
        }

        return name;
    }

    private static string GetEmailAddress(string name)
    {
        name = string.Join(".", name.Split(' ').Select(x => x.ToLower()));

        return $"{name}@example.com";
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
        DateTime dt = new DateTime(2022, 11, 1);

        if (index % 2 == 0)
        {
            return dt.AddDays(-index);
        }

        return null;
    }

    private static Guid GetIdGuid(int index)
    {
        string g = "00a6d8c9-5f3c-42d3-a5f3-886f910f10b5";
        string hex = index.ToString("X");
        if (hex.Length == 1) hex = "0" + hex;

        g = g.Replace("00", hex);

        return Guid.Parse(g);
    }

    private static DateTime GetRegistrationTime(int index)
    {
        DateTime dt = new DateTime(2022, 10, 1);

        return dt.AddDays(-index);
    }

    private static List<string> GetRoles(int index)
    {
        if (index % 3 == 0)
        {
            return null;
        }

        if (index % 2 == 0)
        {
            return new List<string>();
        }

        if (index % 5 == 0)
        {
            return new List<string>() {"admin", "user"};
        }

        return new List<string>() { "sails", "admin", "user" };
    }
}