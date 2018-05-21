using System;
using System.Threading;
using LogAnalyzer.Dal;
using LogAnalyzer.Model.ViewModel;
using LogAnalyzer.Web.App_Start;
using Microsoft.AspNet.SignalR;
using Ninject;

namespace LogAnalyzer.Web.Controllers
{
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

        [Inject]
        public IRepository Repository { get; set; }

        public NewLogNumberBroadcaster()
        {
            hubContext = GlobalHost.ConnectionManager.GetHubContext<NewLogsNumberHub>();
            itemsViewModel = new NumberOfNewLogItemsViewModel();

            NinjectWebCommon.Kernel.Inject(this);
            //Create log's (mongo) repository
            // repository = new MongoRepository();

            //Start the broadcast loop
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
                    //Update client/executes client lister 'updateNumberOfNewItems' in new-logs-number-service.js
                    hubContext.Clients.All.updateNumberOfNewItems(count);
                }
            }
        }

        public void UpdateModel(NumberOfNewLogItemsViewModel clientItemsViewModel)
        {
            itemsViewModel = clientItemsViewModel;
        }
    }
}