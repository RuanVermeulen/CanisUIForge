using System.Reflection;
using CanisUIForge.Contracts.Registry;

namespace CanisUIForge.Contracts.Mapping;

public class SchemaTypeMapper : ISchemaTypeMapper
{
    public void MapTypes(Assembly assembly, ITypeRegistry typeRegistry)
    {
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        if (typeRegistry is null)
        {
            throw new ArgumentNullException(nameof(typeRegistry));
        }

        Type[] exportedTypes;

        try
        {
            exportedTypes = assembly.GetExportedTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            exportedTypes = exception.Types
                .Where(type => type is not null)
                .Cast<Type>()
                .ToArray();
        }

        foreach (Type type in exportedTypes)
        {
            if (!type.IsClass && !type.IsValueType)
            {
                continue;
            }

            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            typeRegistry.Register(type.Name, type);
        }
    }
}
