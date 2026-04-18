namespace CanisUIForge.Generation.Unification;

public interface IMetadataUnifier
{
    List<ResolvedResource> Unify(ApiDefinition apiDefinition, ITypeRegistry typeRegistry);
}
