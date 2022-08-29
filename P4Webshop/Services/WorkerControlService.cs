
using P4Webshop.Interface;
using P4Webshop.Models.Base;
using P4Webshop.Models.Shopping;
using P4Webshop.Workers;
using System.Collections.Concurrent;

namespace P4Webshop.Services
{
    public class WorkerControlService : IWorkerControlService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private CancellationTokenSource _tokenSource;

        private ConcurrentQueue<Func<CancellationToken, List<DeliveryModel>>> _orderItems = new ConcurrentQueue<Func<CancellationToken, List<DeliveryModel>>>();

        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public WorkerControlService(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = loggerFactory.CreateLogger<WorkerControlService>();
            _tokenSource = new CancellationTokenSource();
        }


        public async Task<bool> AllBackgroundWorkers_PowerModeAsync(PowerModes mode)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider.GetServices<IHostedService>();

                switch (mode)
                {
                    case PowerModes.Resume:
                        _logger.LogWarning("System is restarting. Restarting workers");
                        try
                        {
                            _tokenSource = new CancellationTokenSource();
                            foreach (var service in services)
                            {
                                if (service.GetType() == typeof(Worker1) || service.GetType() == typeof(Worker2))
                                {
                                    await service.StartAsync(_tokenSource.Token);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"{ex.Message}");
                            return false;
                        }
                        return true;

                    case PowerModes.Suspend:
                        _logger.LogWarning("System is suspending. Stopping workers");
                        _tokenSource.Cancel();
                        try
                        {
                            foreach (var service in services)
                            {
                                if (service.GetType() == typeof(Worker1) || service.GetType() == typeof(Worker2))
                                {
                                    await service.StopAsync(_tokenSource.Token);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"{ex.Message}");
                            return false;
                        }

                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Stop induvidual worker ---- NEEDS TO GET FIXED -----
        /// </summary>
        /// <param name="modes"></param>
        /// <param name="worker"></param>
        /// <returns></returns>
        public async Task<bool> IndividualWorker_PowerModeAsync(PowerModes modes, ControlWorker worker)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider.GetServices<IHostedService>();
                try
                {
                    foreach (var serv in services)
                    {
                        if(serv.GetType() == worker.GetType()) //  <---- fix this, not correct, weird i tried, look at this text, dont work
                        {
                            switch (modes)
                            {
                                case PowerModes.Resume:
                                    _tokenSource = new CancellationTokenSource();
                                    await serv.StartAsync(_tokenSource.Token);
                                    break;

                                case PowerModes.Suspend:
                                    _tokenSource.Cancel();
                                    await serv.StopAsync(_tokenSource.Token);
                                    break;
                            }
                        }
                    }
                    return false;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"{ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Add to queue so QueuedHostedService can access Work list
        /// </summary>
        /// <param name="workItem"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void QueueOrderItems(Func<CancellationToken, List<DeliveryModel>> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _orderItems.Enqueue(workItem);
            _signal.Release();
        }

        /// <summary>
        /// Deqeueue work item so QueuedHostedService can process t
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Func<CancellationToken, List<DeliveryModel>>> DequeueOrderItemsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _signal.WaitAsync(cancellationToken);
                _orderItems.TryDequeue(out var workItem);


                return workItem;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, $"Workers have stopped operating");
                return null;
            }
        }

        

        

        
        
        
        
        
        

        
        
        
        
        
        
        
        
    }
}
