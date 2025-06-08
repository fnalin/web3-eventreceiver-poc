using Fansoft.EventReceiver.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.EventReceiver.Api.Data.Repositories;

public interface IApplicationProviderRepository
{
    Task<ApplicationProvider?> GetByWalletAsync(string wallet);
}

public class ApplicationProviderRepository (EventReceiverDataContext ctx) : IApplicationProviderRepository
{
    public async Task<ApplicationProvider?> GetByWalletAsync(string wallet) =>
        await ctx.ApplicationProviders.FirstOrDefaultAsync(x => x.WalletId == wallet);

}