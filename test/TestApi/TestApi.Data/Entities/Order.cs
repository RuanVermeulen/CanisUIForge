namespace TestApi.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public System.DateTime OrderDate { get; set; }

    public Customer? Customer { get; set; }
    public Product? Product { get; set; }
}
