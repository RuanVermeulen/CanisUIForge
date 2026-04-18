namespace TestApi.Contracts.Responses;

public class SearchCustomersResponse
{
    public System.Collections.Generic.List<CustomerResponse> Items { get; set; } = new System.Collections.Generic.List<CustomerResponse>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
