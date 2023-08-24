using System;
using System.Collections.Generic;

namespace UPFleet.Models;

public partial class PeachtreeExportedArchive
{
    public string? Owner { get; set; }

    public string? SalesOrder { get; set; }

    public DateTime? Date { get; set; }

    public string? Closed { get; set; }

    public string? CustomerPo { get; set; }

    public string? SalesRepresentativeId { get; set; }

    public string? Account { get; set; }

    public string? SalesTaxId { get; set; }

    public double? NumDistributions { get; set; }

    public double? Transfer { get; set; }

    public string? Qnty { get; set; }

    public string? Invitem { get; set; }

    public string? Description { get; set; }

    public string? Glaccount { get; set; }

    public double? TaxType { get; set; }

    public double? Amount { get; set; }

    public string? SalesTaxAuth { get; set; }

    public int Id { get; set; }
}