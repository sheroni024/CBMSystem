using CBMSystem.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using System.Text.Json;
using CBMSystem.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CBMSystem.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace CBMSystem.Repository.Service
{
    public class ReportRepository : IReport
    {
        private readonly CbmsContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        public ReportRepository(CbmsContext context, IHttpContextAccessor _httpContextAccessor, IWebHostEnvironment _env)
        {
            this.context = context;
            this._httpContextAccessor = _httpContextAccessor;
            this._env = _env;
        }


        public ReportViewModel GetReportByTypeAndId(string type, int id)
        {
            switch (type.ToLower())
            {
                case "customer":
                    var customer = context.CustomerAccounts.FirstOrDefault(c => c.CustomerId == id);
                    if (customer == null) return null;

                    return new ReportViewModel
                    {
                        Id = id,
                        Type = "customer",
                        Title = "Customer Report",
                        Date = DateTime.Now,
                        Description = $"Details for Customer ID: {id}",
                        XCustomerAccount = customer
                    };

                case "loan":
                    var loan = context.Loans.FirstOrDefault(l => l.LoanId == id);
                    if (loan == null) return null;

                    return new ReportViewModel
                    {
                        Id = id,
                        Type = "loan",
                        Title = "Loan Report",
                        Date = DateTime.Now,
                        Description = $"Details for Loan ID: {id}",
                        XLoan = loan
                    };

                case "transaction":
                    var transaction = context.Transactions.FirstOrDefault(t => t.TransactionId == id);
                    if (transaction == null) return null;

                    return new ReportViewModel
                    {
                        Id = id,
                        Type = "transaction",
                        Title = "Transaction Report",
                        Date = DateTime.Now,
                        Description = $"Details for Transaction ID: {id}",
                        XTransaction = transaction
                    };

                case "demat":
                    var demat = context.DematAccounts.FirstOrDefault(d => d.DematId == id);
                    if (demat == null) return null;

                    return new ReportViewModel
                    {
                        Id = id,
                        Type = "demat",
                        Title = "Demat Report",
                        Date = DateTime.Now,
                        Description = $"Details for Demat ID: {id}",
                        XDematAccount = demat
                    };

                case "investment":
                    var investment = context.Investments.FirstOrDefault(i => i.InvestmentId == id);
                    if (investment == null) return null;

                    return new ReportViewModel
                    {
                        Id = id,
                        Type = "investment",
                        Title = "Investment Report",
                        Date = DateTime.Now,
                        Description = $"Details for Investment ID: {id}",
                        XInvestment = investment
                    };

                default:
                    return null;
            }
        }


        //public ReportViewModel GetReportByTypeAndId(string type, int id)
        //{
        //    switch (type.ToLower())
        //    {
        //        case "customer":
        //            var customer = context.CustomerAccounts.FirstOrDefault(c => c.CustomerId == id);
        //            return new ReportViewModel
        //            {
        //                Id = id,
        //                Type = type,
        //                Title = "Customer Report",
        //                Date = DateTime.Now,
        //                Description = $"Details for Customer ID: {id}",
        //                XCustomerAccount = customer
        //            };

        //        case "loan":
        //            var loan = context.Loans.FirstOrDefault(x => x.LoanId == id);
        //            return new ReportViewModel
        //            {
        //                Id = id,
        //                Type = type,
        //                Title = "Loan Report",
        //                Date = DateTime.Now,
        //                Description = $"Loan details for ID: {id}",
        //                XLoan = loan
        //            };

        //        case "transaction":
        //            var transaction = context.Transactions.FirstOrDefault(x => x.TransactionId == id);
        //            return new ReportViewModel
        //            {
        //                Id = id,
        //                Type = type,
        //                Title = "Loan Report",
        //                Date = DateTime.Now,
        //                Description = $"Loan details for ID: {id}",
        //                XTransaction = transaction
        //            };

        //            // Add cases for demat, investment, etc.
        //            // ...
        //    }

        //    return null;
        //}

        public (byte[] Content, string ContentType, string FileName) GenerateTextReport(string type, int id)
        {
            var report = GetReportByTypeAndId(type, id);
            if (report == null) return (null, "text/plain", "not_found.txt");

            var sb = new StringBuilder();
            sb.AppendLine($"Report Type: {report.Type}");
            sb.AppendLine($"Title: {report.Title}");
            sb.AppendLine($"Date: {report.Date:yyyy-MM-dd}");
            sb.AppendLine($"Description: {report.Description}");
            sb.AppendLine("------------------------------------------");

            switch (type.ToLower())
            {
                case "customer":
                    sb.AppendLine($"Customer ID: {report.XCustomerAccount?.CustomerId}");
                    sb.AppendLine($"Name: {report.XCustomerAccount?.FirstName}");
                    sb.AppendLine($"Email: {report.XCustomerAccount?.Email}");
                    break;

                case "loan":
                    sb.AppendLine($"Loan ID: {report.XLoan?.LoanId}");
                    sb.AppendLine($"Loan Type: {report.XLoan?.LoanType}");
                    sb.AppendLine($"Amount: {report.XLoan?.LoanAmount}");
                    break;

                case "transaction":
                    sb.AppendLine($"Transaction ID: {report.XTransaction?.TransactionId}");
                    sb.AppendLine($"Amount: {report.XTransaction?.Amount}");
                    sb.AppendLine($"Date: {report.XTransaction?.TransactionDate}");
                    break;

                default:
                    sb.AppendLine("Unsupported report type.");
                    break;
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"{type}_report_{id}.txt";

            return (bytes, "text/plain", fileName);
        }

        public IEnumerable<CustomerAccount> GetCustomers() => context.CustomerAccounts.ToList();
        public IEnumerable<Loan> GetLoans() => context.Loans.ToList();
        public IEnumerable<Transaction> GetTransactions() => context.Transactions.ToList();
        public IEnumerable<DematAccount> GetDemat() => context.DematAccounts.ToList();
        public IEnumerable<Investment> GetInvestment() => context.Investments.ToList();


        public IEnumerable<FraudReport> GetFraudReports(int? customerId = null)
        {
            // start with the full set
            var query = context.FraudReports
                                .Include(fr => fr.FraudType)
                                .Include(fr => fr.FraudActions)
                                .Include(fr => fr.Customer)
                                .AsQueryable();

            // if a specific customerId was passed in, filter by it
            if (customerId.HasValue)
                query = query.Where(fr => fr.CustomerId == customerId.Value);

            // always order by most‐recent first
            return query.OrderByDescending(fr => fr.ReportedDate)
                        .ToList();
        }


        //public List<FraudReport> GetFraudReportsByCustomerId(long customerId)
        //{
        //    return context.FraudReports
        //        .Where(fr => fr.CustomerId == customerId)
        //        .Include(fr => fr.FraudType)
        //        .Include(fr => fr.FraudActions)
        //        .Include(fr => fr.Customer)
        //        .OrderByDescending(fr => fr.ReportedDate)
        //        .ToList();
        //}


        //public IEnumerable<CustomerAccount> GetCustomers()
        //{
        //    var customers = context.CustomerAccounts.ToList();
        //    if (customers == null)
        //    {
        //        // Handle the error appropriately, maybe log it or return an empty list
        //        return new List<CustomerAccount>();  // Or handle as needed
        //    }
        //    return customers;
        //}

        //public IEnumerable<Loan> GetLoans()
        //{
        //    return context.Loans
        //        .Where(l => l.IsDelete == false || l.IsDelete == null)
        //        .Include(l => l.Status) // Include related entities if needed
        //        .ToList();
        //}

        //public IEnumerable<Transaction> GetTransactions()
        //{
        //    return context.Transactions
        //        .OrderByDescending(t => t.TransactionDate)
        //        .ToList();
        //}

        //public IEnumerable<DematAccount> GetDemat()
        //{
        //    return context.DematAccounts
        //        .Include(d => d.Customer) // Assuming demat is linked to customer
        //        .ToList();

        //    //    var dematWithCustomer = context.DematAccounts
        //    //.Join(
        //    //    context.CustomerAccounts,
        //    //    demat => demat.CustomerId,  // or AccountNumber → adjust as per actual FK
        //    //    customer => customer.CustomerId,
        //    //    (demat, customer) =>
        //    //    {
        //    //        demat.Customer = customer; // Assuming you have a property in DematAccount like `public CustomerAccount Customer { get; set; }`
        //    //        return demat;
        //    //    })
        //    //.ToList();

        //    //    return dematWithCustomer;
        //}

        //public IEnumerable<Investment> GetInvestment()
        //{
        //    return context.Investments
        //        .Include(i => i.CustomerId) // Assuming investment is linked to customer
        //        .ToList();
        //}

        //public Task<(byte[] Content, string ContentType, string FileName)> GenerateReportFileAsync(string type, int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public (byte[] Content, string ContentType, string FileName) GenerateReportFile(string type, int id)
        //{
        //    throw new NotImplementedException();
        //}



        //public async Task<(byte[] Content, string ContentType, string FileName)> GenerateReportFileAsync(string type, int id)
        //{
        //    var report = GetReportByTypeAndId(type, id);

        //    var pdf = new Rotativa.AspNetCore.ViewAsPdf("ReportDetails", report)
        //    {
        //        PageSize = Rotativa.AspNetCore.Options.Size.A4,
        //        FileName = $"{type}_report_{id}.pdf"
        //    };

        //    var fileBytes = await pdf.BuildFile(_httpContextAccessor.HttpContext);
        //    return (fileBytes, "application/pdf", pdf.FileName);
        //}

        //// If you still want a sync version
        //public (byte[] Content, string ContentType, string FileName) GenerateReportFile(string type, int id)
        //{
        //    return GenerateReportFileAsync(type, id).GetAwaiter().GetResult();
        //}


        //public string ExportCsv(IEnumerable<object> data)
        //{
        //    if (data == null || !data.Any()) return string.Empty;

        //    var sb = new StringBuilder();
        //    var properties = data.First().GetType().GetProperties();

        //    // Add header row
        //    sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        //    // Add data rows
        //    foreach (var item in data)
        //    {
        //        var values = properties.Select(p =>
        //            "\"" + (p.GetValue(item)?.ToString()?.Replace("\"", "\"\"") ?? "") + "\"");
        //        sb.AppendLine(string.Join(",", values));
        //    }

        //    return sb.ToString();
        //}

        //public string ExportText(IEnumerable<object> data)
        //{
        //    if (data == null || !data.Any()) return string.Empty;

        //    var sb = new StringBuilder();
        //    foreach (var item in data)
        //    {
        //        string line = JsonSerializer.Serialize(item, new JsonSerializerOptions
        //        {
        //            WriteIndented = false
        //        });
        //        sb.AppendLine(line);
        //    }

        //    return sb.ToString();
        //}

        //public byte[] ExportPdf(IEnumerable<object> data)
        //{
        //    if (data == null || !data.Any())
        //        return Array.Empty<byte>();

        //    using var stream = new MemoryStream();
        //    var document = new Document(PageSize.A4, 25, 25, 30, 30);
        //    PdfWriter.GetInstance(document, stream);
        //    document.Open();

        //    var font = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);

        //    foreach (var item in data)
        //    {
        //        string line = JsonSerializer.Serialize(item, new JsonSerializerOptions
        //        {
        //            WriteIndented = true
        //        });

        //        document.Add(new Paragraph(line, font));
        //        document.Add(new Paragraph("\n")); // Space between entries
        //    }

        //    document.Close();
        //    return stream.ToArray();
        //}
    }
}
