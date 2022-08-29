



using P4Webshop.Models.Base;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Interface
{
    public interface IWorkerControlService
    {
        void QueueOrderItems(Func<CancellationToken, List<DeliveryModel>> workItem);
        Task<Func<CancellationToken, List<DeliveryModel>>> DequeueOrderItemsAsync(CancellationToken cancellationToken);

        Task<bool> AllBackgroundWorkers_PowerModeAsync(PowerModes mode);
        Task<bool> IndividualWorker_PowerModeAsync(PowerModes modes, ControlWorker worker);
    }
}
