namespace Assignment.Infrastructure;

public class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        this._context = context;
    }

    public (Response Response, int TagId) Create(TagCreateDTO tagDTO)
    {
        var entity = _context.Tags.FirstOrDefault(tag => tag.Name == tagDTO.Name);
        Response response;

        if (entity == null)
        {
            entity = new Tag(tagDTO.Name);

            _context.Tags.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }


        return (response, entity.Id);

    }

    public Response Delete(int tagId, bool force = false)
    {
        var tag = _context.Tags.FirstOrDefault(tag => tag.Id == tagId);
        Response response;
        if (tag == null)
        {
            response = Response.NotFound;
        }
        else if (tag.WorkItems != null && tag.WorkItems.Any())
        {
            if (force)
            {
                response = Response.Deleted;
                _context.Tags.Remove(tag); //deletes from fatabase
            }
            else
            {
                response = Response.Conflict;
            }
        }
        else
        {
            response = Response.Deleted;
            _context.Tags.Remove(tag); //deletes from fatabase
        }
        return response;
    }

    public TagDTO Find(int tagId)
    {
        var tag = from t in _context.Tags
                  where t.Id == tagId
                  select new TagDTO(t.Id, t.Name);

        return tag.First();
    }

    public IReadOnlyCollection<TagDTO> Read()
    {
        var tags = from t in _context.Tags
                   orderby t.Name
                   select new TagDTO(t.Id, t.Name);

        return tags.ToArray();
    }

    public Response Update(TagUpdateDTO tag)
    {
        var entity = _context.Tags.Find(tag.Id);

        Response response;

        if (entity == null)
        {
            response = Response.NotFound;
        }
        else if (_context.Tags.FirstOrDefault(t => t.Id != tag.Id && t.Name == tag.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Name = tag.Name;
            _context.SaveChanges();
            response = Response.Updated;
        }

        return response;
    }
}
