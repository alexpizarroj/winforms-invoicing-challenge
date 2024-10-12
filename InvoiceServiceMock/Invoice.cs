using System.Text.Json.Serialization;

public class Invoice
{
    [JsonIgnore]
    public int Id { get; set; }    
    public string CUF { get; set; } = string.Empty;
    public string NIT { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<InvoiceLineItem> LineItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public InvoiceStatus Status { get; set; }

    public bool TryUpdateStatus() {
        // Cambiar estado a Accepted si han pasado más de 5 segundos
        if (Status == InvoiceStatus.Pending && (DateTime.UtcNow - CreatedAt).TotalSeconds > 5)
        {
            Status = InvoiceStatus.Accepted;
            return true;
        }
        else
        {
            return false;
        }
    }
}
