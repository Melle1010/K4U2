namespace Content_API.DTOs
{
    public sealed record MessageResponse(
        string Text,
        DateTime CreatedAt
    );
}
