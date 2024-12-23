using System.ComponentModel.DataAnnotations;

namespace GIS.Country.Service.DTOs
{
    public record LoginUserDTO([Required] string name, [Required] string email);

    public record LoginTokenDTO([Required] string accessToken);
}
