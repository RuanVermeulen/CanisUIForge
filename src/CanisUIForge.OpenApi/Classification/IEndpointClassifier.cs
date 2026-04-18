namespace CanisUIForge.OpenApi.Classification;

public interface IEndpointClassifier
{
    EndpointClassification Classify(HttpMethodType method, string route, string operationId);
}
