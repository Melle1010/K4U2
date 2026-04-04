using System.ComponentModel.DataAnnotations;

namespace Content_API.DTOs
{
    public sealed record UpdateMessageRequest(
        [Required(ErrorMessage = "Titel är obligatorisk")]
        [StringLength(8000, MinimumLength = 1)]
        string Text
    );
}
