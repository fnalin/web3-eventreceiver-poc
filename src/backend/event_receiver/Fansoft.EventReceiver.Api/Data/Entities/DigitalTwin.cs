namespace Fansoft.EventReceiver.Api.Data.Entities;

public class DigitalTwin
{
    public int Id { get; set; }
    public string KeyName { get; set; } = null!;
    public string WalletId { get; set; } = null!;
    public string DtCallbackUrl { get; set; } = null!;
}