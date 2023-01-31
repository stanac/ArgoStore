using System;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.TestsCommon.Entities.Person;

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
            Person p = new Person
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
                Type = (PersonTypes)(i % 3),
                PortList = GetPortList(i),
                PrimaryContact = GetPrimaryContact(i),
                Contacts = GetContacts(i)
            };

            p.SetCollections();
            p.CoronationDate = DateOnly.FromDateTime(p.RegistrationTime.AddDays(-7).Date);

            yield return p;
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
            return "ken sutton";
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
        DateTime dt = new DateTime(2022, 11, 1).ToLocalTime();

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

    private static List<int> GetPortList(int index)
    {
        if (index % 3 == 0) return new List<int> { 1 };
        if (index % 5 == 0) return new List<int> { 1, 3 };
        if (index % 7 == 0) return new List<int> { 1, 3, 2 };
        if (index % 11 == 0) return new List<int> { 1, 2 };
        if (index % 13 == 0) return new List<int> { 2 };
        if (index % 17 == 0) return new List<int> { 2, 3 };
        if (index % 19 == 0) return new List<int> { 3 };
        if (index % 23 == 0) return null;
        return new List<int>();
    }

    private static List<string> GetRoles(int index)
    {
        const string role1 = "admin";
        const string role2 = "user";
        const string role3 = "operator";

        if (index % 3 == 0) return new List<string> {role1};
        if (index % 5 == 0) return new List<string> {role1, role2};
        if (index % 7 == 0) return new List<string> {role1, role2, role3};
        if (index % 11 == 0) return new List<string> {role1, role3};
        if (index % 13 == 0) return new List<string> {role3};
        if (index % 17 == 0) return new List<string> {role3, role2};
        if (index % 19 == 0) return new List<string> {role2};
        if (index % 23 == 0) return null;
        return new List<string>();
    }

    private static Person.PersonContact GetPrimaryContact(int i)
    {
        return GetContacts(i)?.FirstOrDefault();
    }

    private static List<Person.PersonContact> GetContacts(int i)
    {
        if (i % 3 == 0)
            return new List<Person.PersonContact>
            {
                new()
                {
                    ContactType = i,
                    ContactInfos = new List<Person.ContactInfo>()
                }
            };

        if (i % 5 == 0)
            return new List<Person.PersonContact>
            {
                new()
                {
                    ContactType = i,
                    ContactInfos = null
                }
            };

        if (i % 7 == 0) return new List<Person.PersonContact>
        {
            new()
            {
                ContactType = -4,
                ContactInfos = new List<Person.ContactInfo>
                {
                    new ()
                    {
                        Active = true,
                        Details = null
                    }
                }
            }
        };

        if (i % 11 == 0)
            return new List<Person.PersonContact>
            {
                new()
                {
                    ContactType = -8,
                    ContactInfos = new List<Person.ContactInfo>
                    {
                        new()
                        {
                            Active = true,
                            Details = Array.Empty<string>()
                        }
                    }
                }
            };

        if (i % 13 == 0)
            return new List<Person.PersonContact>
            {
                new()
                {
                    ContactType = -9,
                    ContactInfos = new List<Person.ContactInfo>
                    {
                        new()
                        {
                            Active = true,
                            Details = new[] { "a", "abc", "123" }
                        }
                    }
                }
            };

        if (i % 17 == 0)
            return new List<Person.PersonContact>
            {
                new()
                {
                    ContactType = -9,
                    ContactInfos = new List<Person.ContactInfo>
                    {
                        new()
                        {
                            Active = true,
                            Details = new[] { "a", "abc", "123" }
                        },
                        new()
                        {
                            Active = false,
                            Details = new[] { "99", "98" }
                        }
                    }
                }

            };

        if (i % 19 == 0)
            return new List<Person.PersonContact>
            {
                new()
                {
                    ContactType = -9,
                    ContactInfos = new List<Person.ContactInfo>
                    {
                        new()
                        {
                            Active = true,
                            Details = new[] { "a", "abc", "123" }
                        },
                        new()
                        {
                            Active = false,
                            Details = new[] { "99", "98", "s1", "s2" }
                        }
                    }
                }
            };

        if (i % 23 == 0) return null;

        return new List<Person.PersonContact>();
    }

}