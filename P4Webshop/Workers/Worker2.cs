
using Microsoft.EntityFrameworkCore;
using P4Webshop.Interface;
using P4Webshop.Models;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Workers
{
    public class Worker2 : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IWorkerControlService _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Task _executedTask = null;
        private Random _random = new Random();

        public Worker2(IWorkerControlService data, ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _queue = data;
            _logger = loggerFactory.CreateLogger<Worker2>();
            _scopeFactory = scopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executedTask = ExecuteAsync(_tokenSource.Token);

            if (_executedTask.IsCompleted)
                return _executedTask;

            return Task.CompletedTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker2 is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Do_SimulateDelivery_Work(stoppingToken);
                var workItem = await _queue.DequeueOrderItemsAsync(stoppingToken);

                try
                {
                    if (workItem != null)
                    {
                        await Do_AddDelivery_Work(workItem, stoppingToken);
                    }
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex, "Error occurred executing {WorkItem}", nameof(workItem));
                }
            }

            _logger.LogInformation("Worker2 is stopping.");
        }

        public async Task Do_AddDelivery_Work(Func<CancellationToken, List<DeliveryModel>> func, CancellationToken token)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<MyDbContext>();
                var list = func(token);

                foreach(var item in list)
                {
                    item.DeliveryBegun = true;
                    item.DeliveryStarted = DateTime.Now;
                }

                await db.Delivery.AddRangeAsync(list, token);
                await db.SaveChangesAsync(token);
                list.Clear();
            }
        }

        public async Task Do_SimulateDelivery_Work(CancellationToken token)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<MyDbContext>();
                var list = await db.Delivery.ToListAsync();

                if (!list.Any() || list == null)
                    return;

                foreach(var i in list)
                {
                    var ranInt = _random.Next(1, 5);

                    if(i.DeliveryEnded == null)
                    {
                        i.DeliveryEnded = DateTime.Now.AddDays(ranInt);
                    }
                    else if(i.DeliveryEnded < DateTime.Now)
                    {

                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executedTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _tokenSource.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executedTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
    }
}

