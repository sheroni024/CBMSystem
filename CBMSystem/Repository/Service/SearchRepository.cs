using CBMSystem.Models;
using CBMSystem.Repository.Interface;

namespace CBMSystem.Repository.Service
{
    public class SearchRepository : ISearch
    {
        private readonly CbmsContext _context;
        public SearchRepository(CbmsContext context)
        {
            _context = context;
        }

        private readonly Dictionary<string, string> _searchMap = new()
    {
        // Key => search keyword, Value => URL/action
        { "addcustomer", "/CustomerAccounts/Index" },
        { "customer", "/CustomerAccounts/Index" },
        { "createcustomer", "/CustomerAccounts/Index" },
        { "editcustomer", "/CustomerAccounts/Index" },
        { "deletecustomer", "/CustomerAccounts/Index" },
        { "adddemat", "/CustomerAccounts/Index" },
        { "addinvestment", "/CustomerAccounts/Index" },

        { "loan", "/Loan/Index" },
        { "addloan", "/Loan/Index" },
        { "loanapplication", "/Loan/Index" },

        { "transaction", "/Transaction/Index" },
        { "maketransaction", "/Transaction/Index" },
        { "addtransaction", "/Transaction/Index" },
        { "deletetransaction", "/Transaction/Index" },
        { "viewtransactions", "/Transaction/Index" },

        { "dashboard", "/Home/Index" },

        { "customerreport", "/Reports/Index" },
        { "loanreport", "/Reports/Index" },
        { "transactionreport", "/Reports/Index" },
        { "dematreport", "/Reports/Index" },
        { "investmentreport", "/Reports/Index" },

        { "profile", "/Setting/Index" },
        { "changepassword", "/Setting/Index" },
    };

        public string? GetRedirectUrlForSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return null;

            string normalized = searchTerm.Trim().ToLower();

            foreach (var entry in _searchMap)
            {
                if (normalized.Contains(entry.Key))
                {
                    return entry.Value;
                }
            }

            return null;
        }

        public IEnumerable<string> GetSuggestions(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<string>();

            return _searchMap.Keys
                .Where(k => k.Contains(searchTerm.ToLower()))
                .Distinct()
                .Take(5);
        }

    }
}
