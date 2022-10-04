using Assignment.Infrastructure;
using Assignment.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment.Infrastructure.Tests;
public sealed class UserRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        var task1 = new WorkItem("Task1");
        context.Items.Add(task1);
        context.Users.Add(new User("Bob", "something@"));
        context.Users.Add(new User("Frederick", "anotherthing@"));
        context.SaveChanges();

        _context = context;
        _repository = new UserRepository(_context);
    }

    [Fact]
    public void Create_given_UserCreateDTO_Returns_Response_Created_And_UserID()
    {
        var (response, userID) = _repository.Create(new UserCreateDTO("Ib", "IbErSej@itu.dk"));
        response.Should().Be(Response.Created);
        userID.Should().Be(3);
    }

    [Fact]
    public void Delete_given_UserID_Returns_Response_Deleted()
    {
        var response = _repository.Delete(1);
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public void Delete_given_Nonexisting_User_Id_returns_NotFound()
    {
        var response = _repository.Delete(4, true);
        response.Should().Be(Response.NotFound);
    }
    [Fact]
    public void Delete_given_Existing_User_Id_And_Force_False_returns_Conflict()
    {
        var response = _repository.Delete(2, false);
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Read_given_UserID_Returns_UserDTO()
    {
        //var response = _repository.Read(2);
        //response.Should().Be(new UserDTO(2, "Frederick", "anotherthing@"));
    }

    [Fact]
    public void ReadAll_Returns_IReadOnlyCollection()
    {
        //var response = _repository.ReadAll();
        //response.Should().BeEquivalentTo(new List<UserDTO>() { new UserDTO(1, "Bob", "something@"), new UserDTO(2, "Frederick", "anotherthing@") });
    }

    [Fact]
    public void Update_given_UserUpdateDTO_Returns_State_Updated()
    {
        var updateUserDTO = new UserUpdateDTO(2, "FrederickUpdated", "mailUpdated@");
        var response = _repository.Update(updateUserDTO);
        response.Should().Be(Response.Updated);
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}

