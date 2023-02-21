using System.Globalization;

namespace Persons.Directory.Application.Services;

public interface IResourceManagerService
{
    string GetString(string name);
}
