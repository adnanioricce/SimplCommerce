using SimplCommerce.Module.Core.Areas.Core.ViewModels;
namespace SimplCommerce.UnitTests.Modules.Core.Areas
{
    public class UserApiControllerTests 
    {
        [Theory]
        [InLineData("sample@email.com","sample name")]
        [InLineData("","sample name")]
        [InLineData("sample@email.com","")]
        public async Task Given_user_When_receives_object_with_email_or_name_Should_return_five_users(string email,string name)
        {
            //Given
            var repository = new FakeRepository<User>();
            repository.Add(new User{
                Email = email,
                FullName = name
            });
            var controller = new UserApiController(repository,null);
            //When
            var response = controller.QuickSearch(new UserSearchOption{
                Name = name,
                Email = email
            });            
            //Then
            Assert.Equal(200,(int)response.StatusCode);
        }   
        
    }
}