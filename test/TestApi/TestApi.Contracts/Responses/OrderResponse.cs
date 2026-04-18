namespace TestApi.Contracts.Responses;

public class OrderResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public System.DateTime OrderDate { get; set; }
}
