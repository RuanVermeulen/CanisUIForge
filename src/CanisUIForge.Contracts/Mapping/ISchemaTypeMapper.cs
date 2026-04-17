using System.Reflection;
using CanisUIForge.Contracts.Registry;

namespace CanisUIForge.Contracts.Mapping;

public interface ISchemaTypeMapper
{
    void MapTypes(Assembly assembly, ITypeRegistry typeRegistry);
}
