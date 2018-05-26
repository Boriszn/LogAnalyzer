using System;
using System.Threading;
using LogAnalyzer.Dal;
using LogAnalyzer.Model.ViewModel;
using LogAnalyzer.Web.App_Start;
using Microsoft.AspNet.SignalR;
using Ninject;

namespace LogAnalyzer.Web.Controllers
{
    /// <summary>
    /// Brodcasts / watch for collection new entries count
    /// and updates 'updateNumberOfNewItems' in new-logs-number-service.js
    /// </summary>
    public class NewLogNumberBroadcaster
    {
        private static readonly Lazy<NewLogNumberBroadcaster> instance
            = new Lazy<NewLogNumberBroadcaster>(() => new NewLogNumberBroadcaster());

        public static NewLogNumberBroadcaster Instance => instance.Value;

        /// <summary>
        /// Creates time interval for Timer with 10 seconds broadcasting
        /// </summary>
        private readonly TimeSpan broadcastInterval = TimeSpan.FromSeconds(10);
        private Timer broadcastLoop;
        private readonly IHubContext hubContext;
        private NumberOfNewLogItemsViewModel itemsViewModel;

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        [Inject]
        public IRepository Repository { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewLogNumberBroadcaster"/> class.
        /// </summary>
        public NewLogNumberBroadcaster()
        {
            NinjectWebCommon.Kernel.Inject(this);

            hubContext = GlobalHost.ConnectionManager.GetHubContext<NewLogsNumberHub>();
            itemsViewModel = new NumberOfNewLogItemsViewModel();

            // Start the broadcast loop
            broadcastLoop = new Timer(
                BroadcastNumbers,
                null,
                broadcastInterval,
                broadcastInterval);
        }

        /// <summary>
        /// Update client/executes client lister 
        /// 'updateNumberOfNewItems' in new-logs-number-service.js
        /// </summary>
        public void BroadcastNumbers(object state)
        {
            if (itemsViewModel.CollectionName != null)
            {
                int count = Repository.GetNumberOfNewItems(itemsViewModel.CollectionName,
                    itemsViewModel.LoadFromId, itemsViewModel.Query, itemsViewModel.LoadFrom, itemsViewModel.LoadTo);
                
                if (count != 0)
                {
                    // Update client/executes client lister 'updateNumberOfNewItems' in new-logs-number-service.js
                    hubContext.Clients.All.updateNumberOfNewItems(count);
                }
            }
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="clientItemsViewModel">The client items view model.</param>
        public void UpdateModel(NumberOfNewLogItemsViewModel clientItemsViewModel)
        {
            itemsViewModel = clientItemsViewModel;
        }
    }
}