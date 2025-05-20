using Microsoft.AspNetCore.Mvc;
using CBMSystem.Repository; // Use your actual service namespace
using System.Text;
using CBMSystem.Repository.Service;
using CBMSystem.Repository.Interface;
using CBMSystem.ViewModels;
using CBMSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;



namespace CBMSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ICustomers _customerService;
        private readonly ITransaction _transactionService;
        private readonly ILoan _loanService;
        private readonly IReport _reportRepository;

        public ReportsController(
            ICustomers customerService,
            ITransaction transactionService,
            ILoan loanService,
            IReport reportRepository)
        {
            _customerService = customerService;
            _transactionService = transactionService;
            _loanService = loanService;
            _reportRepository = reportRepository;
        }

        public IActionResult Index()
        {
            var model = new ReportViewModel();
            return View(model);
        }


        public IActionResult GetItems(string type)
        {
            // Check if type is null or empty
            if (string.IsNullOrEmpty(type))
            {
                // Handle the error: You could return an error page, redirect, or return an empty result
                return View("Error", new ErrorViewModel { RequestId = "Type cannot be null or empty" });
            }

            switch (type.ToLower())
            {
                case "customer":
                    return PartialView("Partials/Customer", _reportRepository.GetCustomers());
                case "transaction":
                    return PartialView("Partials/Transaction", _reportRepository.GetTransactions());
                case "loan":
                    return PartialView("Partials/Loan", _reportRepository.GetLoans());
                case "demat":
                    return PartialView("Partials/Demat", _reportRepository.GetDemat());
                case "investment":
                    return PartialView("Partials/Investment", _reportRepository.GetInvestment());
                case "fraud":
                    return PartialView("Partials/Investigation", _reportRepository.GetFraudReports());

                default:
                    return NotFound();
            }
        }

        //public IActionResult Details(string type, int id)
        //{
        //    var report = _reportRepository.GetReportByTypeAndId(type, id);
        //    if (report == null)
        //        return NotFound();

        //    return View("Details", report); // ✅ make sure report.Id is correctly set
        //}

        [HttpGet("Reports/Details")]
        public IActionResult Details(string type, int id)
        {
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest("Report type is missing.");

            var model = new ReportViewModel { Type = type };

            switch (type.ToLower())
            {
                case "customer":
                    var customer = _reportRepository.GetCustomers().FirstOrDefault(c => c.CustomerId == id);
                    if (customer != null)
                    {
                        model.Title = "Customer Details";
                        model.XCustomerAccount = customer;
                    }
                    break;

                case "loan":
                    var loan = _reportRepository.GetLoans().FirstOrDefault(l => l.LoanId == id);
                    if (loan != null)
                    {
                        model.Title = "Loan Details";
                        model.XLoan = loan;
                    }
                    break;

                case "transaction":
                    var transaction = _reportRepository.GetTransactions().FirstOrDefault(t => t.TransactionId == id);
                    if (transaction != null)
                    {
                        model.Title = "Transaction Details";
                        model.XTransaction = transaction;
                    }
                    break;

                case "demat":
                    var demat = _reportRepository.GetDemat().FirstOrDefault(d => d.DematId == id);
                    if (demat != null)
                    {
                        model.Title = "Demat Details";
                        model.XDematAccount = demat;
                    }
                    break;

                case "investment":
                    var investment = _reportRepository.GetInvestment().FirstOrDefault(i => i.InvestmentId == id);
                    if (investment != null)
                    {
                        model.Title = "Investment Details";
                        model.XInvestment = investment;
                    }
                    break;

                case "fraud":
                    var fraud = _reportRepository.GetFraudReports().FirstOrDefault(f => f.FraudReportId == id);
                    if (fraud != null)
                    {
                        model.Title = "Investigation Report Details";
                        model.XFraudReport = fraud;
                    }
                    break;

                    // Add more cases for other types like Demat, Investment, etc.
            }

            if (model.Title == null)
            {
                return NotFound("Details not found.");
            }

            return View(model);
        }

        [HttpGet("Reports/DownloadReport")]
        //public IActionResult DownloadReport(string type, int id, string format )
        //{
        //    Console.WriteLine($"Type: {type}, Id: {id}, Format: {format}");

        //    if (string.IsNullOrWhiteSpace(type))
        //        return BadRequest("Report type is missing.");

        //    var report = _reportRepository.GetReportByTypeAndId(type, id);
        //    if (report == null)
        //        return NotFound("Report not found.");

        //    if (format == "pdf")
        //    {
        //        // Here you could generate PDF content — for now, return dummy file
        //        byte[] pdfBytes = System.Text.Encoding.UTF8.GetBytes("PDF content goes here");
        //        return File(pdfBytes, "application/pdf", $"{type}_report_{id}.pdf");
        //    }
        //    else
        //    {
        //        var (content, contentType, fileName) = _reportRepository.GenerateTextReport(type, id);
        //        if (content == null)
        //            return NotFound("Unable to generate report.");

        //        return File(content, contentType, fileName);
        //    }

        //}

        public IActionResult DownloadReport(string type, int id, string format = "pdf")
        {
            var model = new ReportViewModel { Type = type };

            object dataObject = null;

            switch (type?.ToLower())
            {
                case "customer":
                    model.Title = "Customer Report";
                    dataObject = _reportRepository.GetCustomers().FirstOrDefault(c => c.CustomerId == id);
                    break;

                case "loan":
                    model.Title = "Loan Report";
                    dataObject = _reportRepository.GetLoans().FirstOrDefault(l => l.LoanId == id);
                    break;

                case "transaction":
                    model.Title = "Transaction Report";
                    dataObject = _reportRepository.GetTransactions().FirstOrDefault(t => t.TransactionId == id);
                    break;

                case "demat":
                    model.Title = "Demat Report";
                    dataObject = _reportRepository.GetDemat().FirstOrDefault(d => d.DematId == id);
                    break;

                case "investment":
                    model.Title = "Investment Report";
                    dataObject = _reportRepository.GetInvestment().FirstOrDefault(i => i.InvestmentId == id);
                    break;

                case "fraud":
                    model.Title = "Fraud Report";
                    //var allReports = _reportRepository.GetFraudReports();

                    // list for one customer
                    var fraudReportByCustomer = _reportRepository.GetFraudReports().FirstOrDefault(R => R.FraudReportId == id); // pass `id` here as the customerId
                    dataObject = fraudReportByCustomer; // Assign filtered fraud reports for the customer
                    break;
                    

                default:
                    return NotFound("Invalid type.");
            }

            if (dataObject == null)
                return NotFound("Data not found.");

            if (format.ToLower() == "pdf")
            {
                using var stream = new MemoryStream();
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var labelFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                var valueFont = FontFactory.GetFont("Arial", 12);

                doc.Add(new Paragraph(model.Title, titleFont));
                doc.Add(new Paragraph(" "));

                // Reflectively generate fields
                foreach (var prop in dataObject.GetType().GetProperties())
                {
                    var label = prop.Name;
                    var value = prop.GetValue(dataObject)?.ToString() ?? "N/A";

                    // Format date fields
                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value, out var dateVal))
                        {
                            value = dateVal.ToString("dd MMM yyyy");
                        }
                    }

                    doc.Add(new Paragraph($"{label}: {value}", valueFont));
                }

                doc.Close();

                string filename = $"{type}_{id}_Report.pdf";
                return File(stream.ToArray(), "application/pdf", filename);
            }
            else if (format == "txt")
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine(model.Title.ToUpper());
                sb.AppendLine(new string('-', 30));

                foreach (var prop in dataObject.GetType().GetProperties())
                {
                    var label = prop.Name;
                    var value = prop.GetValue(dataObject)?.ToString() ?? "N/A";

                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value, out var dateVal))
                        {
                            value = dateVal.ToString("dd MMM yyyy");
                        }
                    }

                    sb.AppendLine($"{label}: {value}");
                }

                var fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                string filename = $"{type}_{id}_Report.txt";
                return File(fileBytes, "text/plain", filename);
            }

            return BadRequest("Unsupported format.");
        }


        //public IActionResult DownloadReport(string type, int id)
        //{
        //    var report = _reportRepository.GenerateTextReport(type, id);
        //    if (report.Content == null)
        //        return NotFound("Report not found.");

        //    return File(report.Content, report.ContentType, report.FileName);
        //}


        //public IActionResult GetItems(string type)
        //{
        //    switch (type.ToLower())
        //    {
        //        case "customer":
        //            var customers = _reportRepository.GetCustomers();
        //            return PartialView("Partials/Customer", customers);
        //        case "transaction":
        //            var transactions = _reportRepository.GetTransactions();
        //            return PartialView("_TransactionListPartial", transactions);
        //        case "loan":
        //            var loans = _reportRepository.GetLoans();
        //            return PartialView("_LoanListPartial", loans);
        //        default:
        //            return BadRequest();
        //    }
        //}

        //public IActionResult Details(string type, int id)
        //{
        //    var report = _reportRepository.GetReportByTypeAndId(type, id);
        //    return View("ReportDetails", report);
        //}

        //public IActionResult Download(string type, int id)
        //{
        //    var file = _reportRepository.GenerateTextReport(type, id);
        //    return File(file.Content, file.ContentType, file.FileName);
        //}


        //public IActionResult Download(string type, int id)
        //{
        //    var file = _reportRepository.GenerateReportFile(type, id);
        //    return File(file.Content, file.ContentType, file.FileName);
        //}

        //public async Task<IActionResult> Download(string type, int id)
        //{
        //    var file = await _reportRepository.GenerateReportFileAsync(type, id);
        //    return File(file.Content, file.ContentType, file.FileName);
        //}


    }



    //[HttpGet]
    //public IActionResult Export()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public IActionResult Export(string mainType, string format)
    //{
    //    IEnumerable<object> data = mainType switch
    //    {
    //        "Customer" => _customerService.GetAllCustomers().Cast<object>(),
    //        "Transaction" => _transactionService.GetAll().Cast<object>(),
    //        "Loan" => _loanService.GetAll().Cast<object>(),
    //        _ => null
    //    };

    //    if (data == null || !data.Any()) return BadRequest("Invalid or empty data.");

    //    format = format?.ToLower();

    //    return format switch
    //    {
    //        "csv" => File(Encoding.UTF8.GetBytes(_reportRepository.ExportCsv(data)), "text/csv", "Report.csv"),
    //        "txt" => File(Encoding.UTF8.GetBytes(_reportRepository.ExportText(data)), "text/plain", "Report.txt"),
    //        //"pdf" => File(_reportRepository.ExportPdf(data), "application/pdf", "Report.pdf"),
    //        _ => BadRequest("Unsupported format.")
    //    };
    //}
}
