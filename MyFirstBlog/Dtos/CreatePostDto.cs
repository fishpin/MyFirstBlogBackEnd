namespace MyFirstBlog.Dtos;

public record CreatePostDto
{
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
}
