using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InvoiceDb>(opt => opt.UseSqlite());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/invoices", async (InvoiceDb db) => {
    var invoices = await db.Invoices.OrderByDescending(e => e.CreatedAt).ToListAsync();

    var updated = false;
    foreach (var invoice in invoices) {
        updated = updated || invoice.TryUpdateStatus();
    }
    if (updated) await db.SaveChangesAsync();

    return invoices;
})
.WithName("ListInvoices")
.WithOpenApi();

app.MapPost("/api/invoices", async (InvoiceDb db, CreateInvoiceDto dto) =>
{
    var invoice = new Invoice() {
        CUF = Guid.NewGuid().ToString(),
        NIT = dto.NIT,
        CustomerName = dto.CustomerName,
        LineItems = dto.LineItems.Select(e => new InvoiceLineItem() {
            Description = e.Description,
            Quantity = e.Quantity,
            UnitPrice = e.UnitPrice,
        }).ToList(),
        TotalAmount = dto.LineItems.Select((e) => e.Subtotal).Sum(),
        CreatedAt = DateTime.UtcNow,
        Status = InvoiceStatus.Pending,
    };

    db.Invoices.Add(invoice);
    await db.SaveChangesAsync();

    return Results.Accepted($"/api/invoices/{invoice.CUF}");
})
.WithName("CreateInvoice")
.WithOpenApi();

app.MapGet("/api/invoices/{cuf}", async (string cuf, InvoiceDb db) => {
    var invoice = await db.Invoices.FirstOrDefaultAsync(i => i.CUF == cuf);
    if (invoice == null) return Results.NotFound();

    var updated = invoice.TryUpdateStatus();
    if (updated) await db.SaveChangesAsync();

    return Results.Ok(invoice);
})
.WithName("GetInvoice")
.WithOpenApi();

app.MapGet("/api/invoices/{cuf}/pdf", async (string cuf, InvoiceDb db) => {
    var invoice = await db.Invoices.FirstOrDefaultAsync(i => i.CUF == cuf);
    if (invoice == null || invoice.Status != InvoiceStatus.Accepted) return Results.NotFound();

    using (var stream = new MemoryStream())
    {
        var document = new Document();
        PdfWriter.GetInstance(document, stream).CloseStream = false;
        document.Open();

        document.Add(new Paragraph($"Factura #: {invoice.Id}"));
        document.Add(new Paragraph($"NIT: {invoice.NIT}"));
        document.Add(new Paragraph($"Cliente: {invoice.CustomerName}"));

        document.Add(new Paragraph("\nDetalles de la factura:"));
        foreach (var item in invoice.LineItems)
        {
            document.Add(new Paragraph($"{item.Description} - {item.Quantity} x {item.UnitPrice:C} = {item.Subtotal:C}"));
        }

        document.Add(new Paragraph($"Total: {invoice.TotalAmount:C}"));

        document.Close();

        var pdfBytes = stream.ToArray();
        return Results.File(pdfBytes, "application/pdf", $"Factura #{invoice.Id}.pdf");
    }
})
.WithName("GetInvoicePdf")
.WithOpenApi();

app.Run();
