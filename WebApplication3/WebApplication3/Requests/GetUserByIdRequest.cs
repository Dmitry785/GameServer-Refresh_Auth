using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Requests;

public class GetUserByIdRequest
{
    public Guid Id { get; set; }
    [Required]
    public string AccessToken { get; set; }
}
