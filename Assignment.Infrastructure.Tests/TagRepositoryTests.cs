using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment.Core;
using System.Xml.Linq;

namespace Assignment.Infrastructure.Tests;

public class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);

        var context = new KanbanContext(builder.Options);
        context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        List<User> users = new()
        {
            new User("Jens", "jens@gmail.com"),
            new User("Bo", "bo@gmail.com"),
        };

        List<Tag> tags = new()
        {
            new Tag("Smart"),
            new Tag("Green"),
        };

        List<WorkItem> workItems = new()
        {
            new WorkItem("Project"){State = State.Active},
            new WorkItem("Milestone"){State = State.New},
            new WorkItem("Task"){State = State.Removed},
        };

        context.Users.AddRange(users);
        context.Tags.AddRange(tags);
        context.Items.AddRange(workItems);

        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public void CreateGivenTag()
    {
        var (Response, TagId) = _repository.Create(new TagCreateDTO("ITU"));

        Response.Should().Be(Response.Created);

        TagId.Should().Be(3);
    }

    [Fact]
    public void DeleteTag()
    {
        var deletedTag = _repository.Delete(1);

        deletedTag.Should().Be(Response.Deleted);
    }

    [Fact]
    public void FindTag()
    {
        var findTag = _repository.Find(2);

        findTag.Id.Should().Be(2);
        findTag.Name.Should().Be("Green");
    }

    [Fact]
    public void ReadTags()
    {

        TagDTO[] array = { new TagDTO(2, "Green"), new TagDTO(1, "Smart") };
        var readTag = _repository.Read();

        readTag.Should().BeEquivalentTo(array);
    }

    [Fact]
    public void UpdateTag()
    {
        var tag = _repository.Update(new TagUpdateDTO(1, "NewName"));

        tag.Should().Be(Response.Updated);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
