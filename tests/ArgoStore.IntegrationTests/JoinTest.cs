using System;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace ArgoStore.IntegrationTests
{
    public class JoinTest : IntegrationTestsBase
    {
        public JoinTest(ITestOutputHelper output) : base(output)
        {
            
        }

        [SkippableFact]
        public void Join_ThrowsException()
        {
            using IDocumentSession s = GetNewDocumentSession();

            IEnumerable<string> toJoin = new List<string>();

            Action a = () => s.Query<PersonStringPk>()
                .Join(toJoin, c => c.Name, c => c, (c1, c2) => new {c1, c2})
                .ToList();

            a.Should().Throw<NotSupportedException>();
        }


        [SkippableFact]
        public void GroupJoin_ThrowsException()
        {
            using IDocumentSession s = GetNewDocumentSession();

            IEnumerable<string> toJoin = new List<string>();

            Action a = () => s.Query<PersonStringPk>()
                .GroupJoin(toJoin, c => c.Name, c => c, (c1, c2) => new { c1, c2 })
                .ToList();

            a.Should().Throw<NotSupportedException>();
        }
    }
}
