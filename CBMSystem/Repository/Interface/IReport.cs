using CBMSystem.Models;
using CBMSystem.ViewModels;

namespace CBMSystem.Repository.Interface
{
    public interface IReport
    {
        ReportViewModel GetReportByTypeAndId(string type, int id);

        //Task<(byte[] Content, string ContentType, string FileName)> GenerateReportFileAsync(string type, int id);

        //(byte[] Content, string ContentType, string FileName) GenerateReportFile(string type, int id);

        IEnumerable<CustomerAccount> GetCustomers();
        IEnumerable<Loan> GetLoans();
        IEnumerable<Transaction> GetTransactions();
        IEnumerable<DematAccount> GetDemat();
        IEnumerable<Investment> GetInvestment();
        IEnumerable<FraudReport> GetFraudReports(int? customerId = null);
        //List<FraudReport> GetFraudReportsByCustomerId(long customerId);
        //IEnumerable<FraudReport> GetFraudReportsByCustomerId(int customerId);


        // New method to generate TXT instead of PDF
        (byte[] Content, string ContentType, string FileName) GenerateTextReport(string type, int id);

        //IEnumerable<CustomerAccount> GetCustomers();
        //IEnumerable<Transaction> GetTransactions();
        //IEnumerable<Loan> GetLoans();

        //ReportViewModel GetReportByTypeAndId(string type, int id);

        //Task<(byte[] Content, string ContentType, string FileName)> GenerateReportFileAsync(string type, int id);
    }

}
