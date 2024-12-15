using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Greeting
{

    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Greetings { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

}