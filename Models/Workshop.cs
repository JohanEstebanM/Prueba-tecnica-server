using System.Collections.Generic;

namespace server.Models
{
    public class Workshop
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Location { get; set; } = null!;
    public bool Availability { get; set; }
}

}