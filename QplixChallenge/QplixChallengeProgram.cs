using System.Globalization;
using CsvHelper;
using QplixChallenge.Model;
using CsvHelper.Configuration;

public class QplixChallengeProgram
{
    private static string delimiter = ";";
    private static List<Investment> investments = new List<Investment>();
    private static List<Quote> quotes = new List<Quote>();
    private static List<Transaction> transactions = new List<Transaction>();
    private static string investorId = null;
    private static DateTime date = DateTime.Now;

    private static void Main(string[] args)
    {
        Console.WriteLine("Please enter date yyyy-mm-dd and investor ID separated by ; and press enter");
        var line = Console.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            var input = line.Split(delimiter);
            date = DateTime.ParseExact(input[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            investorId = input[1];
        }

        Console.WriteLine($"Value of investor id {investorId} portfolio is {CalculatePortfolioValueForDateAndInvestorId(date, investorId)}");
    }

    private static double CalculateValueForInvestment(Investment investment)
    {
        double result = 0;
        switch (investment.InvestmentType)
        {
            case "Stock":
                {
                    var shareHistory = quotes.FirstOrDefault(q => q.ISIN == investment.ISIN && q.Date == date);
                    if (shareHistory == null)
                    {
                        Console.WriteLine($"No share price history for ISIN {investment.ISIN} and date {date.ToString()}");
                        return 0;
                    }

                    var transactionsForInvestmentIdAndDate = transactions.FindAll(t => t.InvestmentId == investment.InvestmentId && t.Date == date);
                    if (transactionsForInvestmentIdAndDate == null)
                    {
                        Console.WriteLine($"No transactions for investment ID {investment.InvestmentId} and date {date.ToString()}");
                    }
                    foreach (Transaction transaction in transactionsForInvestmentIdAndDate)
                    {
                        result += transaction.Value * shareHistory.PricePerShare;
                    }
                    return result;
                }
            case "RealEstate":
                {
                    var transactionsForInvestmentIdAndDate = transactions.FindAll(t => t.InvestmentId == investment.InvestmentId && t.Date == date);
                    if (transactionsForInvestmentIdAndDate == null)
                    {
                        Console.WriteLine($"No transactions for investment ID {investment.InvestmentId} and date {date.ToString()}");
                    }
                    foreach (Transaction transaction in transactionsForInvestmentIdAndDate)
                    {
                        result += transaction.Value;
                    }
                    return result;
                }
            //Fonds
            default:
                {
                    var transactionsForInvestmentIdAndDate = transactions.FindAll(t => t.InvestmentId == investment.InvestmentId && t.Date == date);

                    foreach (Transaction transaction in transactionsForInvestmentIdAndDate)
                    {
                        if (transactionsForInvestmentIdAndDate == null)
                        {
                            Console.WriteLine($"No transactions for investment ID {investment.InvestmentId} and date {date.ToString()}");
                            return 0;
                        }
                        var shareHistory = quotes.Where(q => q.Date == date);
                        if (shareHistory == null)
                        {
                            Console.WriteLine($"No share price history for ISIN {investment.ISIN} and date {date.ToString()}");
                            return 0;
                        }
                        foreach (Quote quote in shareHistory)
                        {
                            result += quote.PricePerShare;
                        }
                        result *= transaction.Value;
                    }
                    return result;
                }
        }
    }

    public static double CalculatePortfolioValueForDateAndInvestorId(DateTime d, string investor)
    {
        //test mocking
        date = d;
        investorId = investor;

        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = delimiter };

        using (var reader = new StreamReader("Static/Investments.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            investments = csv.GetRecords<Investment>().ToList();
        }
        using (var reader = new StreamReader("Static/Quotes.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            quotes = csv.GetRecords<Quote>().ToList();
        }
        using (var reader = new StreamReader("Static/Transactions.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            transactions = csv.GetRecords<Transaction>().ToList();
        }

        double result = 0;

        var investmentsForInvestorId = investments.Where(i => i.InvestorId == investorId);

        if (!investmentsForInvestorId.Any())
        {
            throw new ArgumentException($"No investments for investor ID {investorId} and date {date.ToString()}");
        }

        foreach (var investment in investmentsForInvestorId)
        {
            result += CalculateValueForInvestment(investment);
        }

        return result;
    }
}