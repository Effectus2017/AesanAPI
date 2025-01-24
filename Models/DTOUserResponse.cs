namespace Api.Models;

public class DTOUserResponse
{
    public List<DTOUser> Data { get; set; } = new();
    public int Count { get; set; }
}