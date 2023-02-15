using System.ComponentModel.DataAnnotations;

namespace Persons.Directory.Application.Domain;

public class Entity
{
    [Key]
    public int Id { get; protected set; }
}
