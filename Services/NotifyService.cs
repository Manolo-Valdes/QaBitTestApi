using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QaBitTestApi.Db;
using QaBitTestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QaBitTestApi.Services
{
    /// <summary>
    /// Background Service that sends notifications to users
    /// about reserved Products exists in stock
    /// </summary>
    public class NotifyService : BackgroundService
    {
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(IServiceScopeFactory scopeFactory, ILogger<NotifyService> logger)
        {
            ScopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Loading Scheduled Tasks..");
                await ExecuteOnceAsync(stoppingToken);
                _logger.LogInformation("Waiting 24 hours until next run..");
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }

        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            using (var serviceScope = ScopeFactory.CreateScope())
            {
                ApiDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();
                var suscriptions = await dbContext.ClientSuscriptions.ToListAsync(cancellationToken);
                var reservedProducts = new Dictionary<int, int>();
                foreach(var s in suscriptions)
                {
                    var product = dbContext.Products.Find(s.RequestedProductID);
                    int reserved = 0;
                    if (reservedProducts.ContainsKey(s.RequestedProductID))
                    {
                        reserved = reservedProducts[s.RequestedProductID];
                    }
                    else
                    {
                        reservedProducts.Add(s.RequestedProductID, 0);
                    }
                    if (s.RequestedProductAmount < product.Stock - reserved)
                    {
                        if (await SendNotificationAsync(s, cancellationToken))
                        {
                            reserved += s.RequestedProductAmount;
                            reservedProducts[s.RequestedProductID] = reserved;
                            //Unsuscribing executed notification
                            dbContext.ClientSuscriptions.Remove(s);
                        }
                    }
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task<bool> SendNotificationAsync(ClientSuscription suscription,CancellationToken cancellationToken)
        {
            //Enviar email usando los datos de la clase ClientSuscription
            //Omitido para simplificar el ejemplo.
            //Se implementa facilmente usando el paquete MailKit
            return await Task.FromResult<bool>(true);
        }

    }
}
