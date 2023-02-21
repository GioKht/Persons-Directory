using System.Globalization;
using System.Resources;

namespace Persons.Directory.Application.Services;

public class ResourceManagerService : IResourceManagerService
{
    private readonly ResourceManager _resourceManager;

    public ResourceManagerService(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public string GetString(string name)
    {
        return _resourceManager.GetString(name, CultureInfo.CurrentCulture);
    }
}
