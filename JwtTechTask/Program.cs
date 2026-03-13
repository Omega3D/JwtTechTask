using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JwtTechTask;

public enum TransactionStatus { New = 0, Completed = 1, InProgress = 2, Unknown = 3 }

public class Payload
{
    [JsonPropertyName("data")]
    public List<UserTransactionModel> Data { get; set; } = new List<UserTransactionModel>();
}

public class UserTransactionModel
{
    [JsonPropertyName("userId")]
    public required string UserId { get; set; }
    [JsonPropertyName("transactions")]
    public required List<TransactionModel> Transactions { get; set; }

}

public class TransactionModel
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }
    [JsonPropertyName("meta")]
    public required TransactionMeta Meta { get; set; }
    [JsonPropertyName("status")]
    [JsonConverter(typeof(TransactionStatusConverter))]
    public TransactionStatus Status { get; set; }

}

public class TransactionMeta
{
    [JsonPropertyName("source")]
    public required string Source { get; set; }
    [JsonPropertyName("confirmed")]
    public bool IsConfirmed { get; set; }
}

public static class JwtService
{
    public static Payload DecodePayload(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        var payload = token.Payload.SerializeToJson();

        var result = JsonSerializer.Deserialize<Payload>(payload)
            ?? throw new InvalidOperationException("Failed to deserialize JWT payload.");

        return result;
    }
}

public static class Reporter
{
    public static void PrintReport(Payload payload)
    {
        Console.WriteLine("═════════════════════════════");

        foreach (var user in payload.Data)
        {
            decimal totalConfirmedAmount = 0;

            Console.WriteLine($"User ID: {user.UserId}");
            Console.WriteLine($"Total Transactions Count: {user.Transactions.Count}");

            var validTransactions = new List<TransactionModel>();

            foreach (var t in user.Transactions)
            {
                if (t.Status == TransactionStatus.Completed && t.Meta.IsConfirmed)
                {
                    validTransactions.Add(t);
                }
            }

            foreach (var t in validTransactions)
            {
                totalConfirmedAmount += t.Amount;
            }

            Console.WriteLine($"Total Confirmed Transactions: {validTransactions.Count} and Total amount of valid Transactions: {totalConfirmedAmount}");
            Console.WriteLine("═════════════════════════════");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string jwt = "eyJhbGciOiJub25lIn0.eyJkYXRhIjpbeyJ1c2VySWQiOiIxMjM0NSIsInRyYW5zYWN0aW9ucyI6W3siaWQiOiIxIiwiYW1vdW50Ijo1MCwiY3VycmVuY3kiOiJVQUgiLCJtZXRhIjp7InNvdXJjZSI6IkNBQiIsImNvbmZpcm1lZCI6dHJ1ZX0sInN0YXR1cyI6IkNvbXBsZXRlZCJ9LHsiaWQiOiIyIiwiYW1vdW50IjozMC41LCJjdXJyZW5jeSI6IlVBSCIsIm1ldGEiOnsic291cmNlIjoiQUNCIiwiY29uZmlybWVkIjpmYWxzZX0sInN0YXR1cyI6IkluUHJvZ3Jlc3MifSx7ImlkIjoiMyIsImFtb3VudCI6ODkuOTksImN1cnJlbmN5IjoiVUFIIiwibWV0YSI6eyJzb3VyY2UiOiJDQUIiLCJjb25maXJtZWQiOnRydWV9LCJzdGF0dXMiOiJDb21wbGV0ZWQifV19LHsidXNlcklkIjoidTEyMyIsInRyYW5zYWN0aW9ucyI6W3siaWQiOiIxIiwiYW1vdW50Ijo0NDM0LCJjdXJyZW5jeSI6IkVVUiIsIm1ldGEiOnsic291cmNlIjoiQ0FCIiwiY29uZmlybWVkIjp0cnVlfSwic3RhdHVzIjoiQ29tcGxldGVkIn0seyJpZCI6IjIiLCJhbW91bnQiOjU2LjUzLCJjdXJyZW5jeSI6IlVBSCIsIm1ldGEiOnsic291cmNlIjoiQUNCIiwiY29uZmlybWVkIjpmYWxzZX0sInN0YXR1cyI6Mn1dfV19.";

        try
        {
            var payload = JwtService.DecodePayload(jwt);

            Reporter.PrintReport(payload);
        }
        catch (FormatException ex)
        {
            Console.Error.WriteLine($"JWT Error {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"JSON Error {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected Error {ex.Message}");
        }
    }
}