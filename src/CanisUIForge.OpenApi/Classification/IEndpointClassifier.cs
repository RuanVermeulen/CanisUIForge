using CanisUIForge.Core.Enums;

namespace CanisUIForge.OpenApi.Classification;

public interface IEndpointClassifier
{
    EndpointClassification Classify(HttpMethodType method, string route, string operationId);
}
