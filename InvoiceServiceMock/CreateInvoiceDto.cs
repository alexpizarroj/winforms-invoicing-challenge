public class CreateInvoiceDto
{
    public string NIT { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateInvoiceLineItemDto> LineItems { get; set; } = new();
}
