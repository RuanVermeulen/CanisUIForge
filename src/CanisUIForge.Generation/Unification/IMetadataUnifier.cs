using CanisUIForge.Contracts.Registry;
using CanisUIForge.Generation.Models;
using CanisUIForge.OpenApi.Models;

namespace CanisUIForge.Generation.Unification;

public interface IMetadataUnifier
{
    List<ResolvedResource> Unify(ApiDefinition apiDefinition, ITypeRegistry typeRegistry);
}
