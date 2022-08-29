using Microsoft.EntityFrameworkCore;
using P4Webshop.Interface;
using P4Webshop.Models;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Workers
{
    /// <summary>
    /// This worker takes orders from db and makes it ready to get shipped by creating the Delivery objects, and when it gets executed in Worker2
    /// it will get the shipping simulated
    /// </summary>
    public class Worker1 : BackgroundService
    {

        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Task _executedTask = null;

        public Worker1(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<Worker1>();
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
            _logger.LogInformation("Worker1 is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Do_Work(stoppingToken);
                    await Task.Delay(10000, stoppingToken);
                }
                catch (TaskCanceledException ex)
                {
                    //_logger.LogCritical("Worker1 has")
                }
            }

            _logger.LogInformation("Worker1 is stopping.");
        }

        public async Task Do_Work(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<MyDbContext>();
                    var queue = scope.ServiceProvider.GetService<IWorkerControlService>();
                    var orders = await db.Orders
                                        .Include(x => x.Customer)
                                        .Include(x => x.DeliveryAddress)
                                        .Include(x => x.OrderdProducts)
                                        .ToListAsync(stoppingToken);


                    if (!orders.Any() || orders == null)
                       return;

               
                    queue.QueueOrderItems(stoppingToken =>
                    {
                        List<DeliveryModel> deliveryModels = new List<DeliveryModel>();
                        foreach (var item in orders)
                        {
                            var delivery = new DeliveryModel
                            {
                                Id = Guid.NewGuid(),
                                Order = item,
                                DeliveryStarted = DateTime.Now,
                                DeliveryBegun = false,
                                Address = item.DeliveryAddress.Address,
                                AddressNumber = item.DeliveryAddress.AddressNumber,
                                City = item.DeliveryAddress.City,
                                Zipcode = item.DeliveryAddress.Zipcode
                            };
                            
                            deliveryModels.Add(delivery);

                        }

                        return deliveryModels;
                    });
                }               
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
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
