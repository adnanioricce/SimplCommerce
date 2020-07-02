using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Infrastructure.Tests.Data
{
    public static class FakeRepositoryHelpers
    {
        public static Mock<DbSet<T>> GetDbSet<T>(IQueryable<T> TestData) where T : class
        {       
            return TestData.BuildMockDbSet();
        }
        public static IQueryable<T> BuildQueryableMock<T>(this IQueryable<T> testData) where T : class,new()
        {
            return testData.BuildMock<T>().Object;
        }       
    }
}
