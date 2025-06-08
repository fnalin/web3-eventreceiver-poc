using Fansoft.EventReceiver.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.EventReceiver.Api.Data.Repositories;

public class DigitalTwinRepository (EventReceiverDataContext ctx) : IDigitalTwinRepository
{
    public async Task<DigitalTwin> GetAddressByNameAsync(string name) =>
        await ctx.DigitalTwins.FirstOrDefaultAsync(x=>x.KeyName == name) ??
        throw new InvalidOperationException("Digital twin not found");
}

public interface IDigitalTwinRepository
{
    Task<DigitalTwin> GetAddressByNameAsync(string name);
}