using System;
using System.Collections.Generic;

namespace LogAnalyzer.Dal.Models.EventStore
{
    public class MessageForCollection
    {
        private Dictionary<string, string> _collectiontypes = new Dictionary<string, string>();
        private static MessageForCollection _instance;
        private static object obj = new object();
        private const string dispatcherStr = "Nuts.InterDom.Queue.Dispatcher`1[[{0}]], Nuts.InterDom.Queue, Culture=neutral, PublicKeyToken=null"; 

        private MessageForCollection()
        {
            _collectiontypes.Add("AcceptedMeterReadings", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeterReadingRejection, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("CommercialCharacteristicUpdate", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeterReadingRejection, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("DisputeResults", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.DisputeResult, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("Disputes", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeteringPoint, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("FileExchangeInfos", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.FileExchangeInfo, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("LeadExportContract", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.Transport.LeadExportContract, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("LeadExportContracts", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.Transport.LeadExportContract, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("MasterDataUpdate", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MasterDataUpdate, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("MeterReadingRejections", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeterReadingRejection, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("MeterReadings", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeteringPoint, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("MeterReadingsDev", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeteringPoint, Nuts.InterDom.Models,  Culture=neutral, PublicKeyToken=null"));
            _collectiontypes.Add("MeterReadingsSent", String.Format(dispatcherStr, "Nuts.InterDom.Model.Messages.MeterReading.MeteringPoint, Nuts.InterDom.Models, Culture=neutral, PublicKeyToken=null"));
        }

        static public MessageForCollection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (obj)
                    {
                        if (_instance == null)
                        {
                            _instance = new MessageForCollection();
                        }
                    }
                }
                return _instance;
            }
        }

        public string GetQueueType(string collectionName)
        {
            return _collectiontypes[collectionName];
        }

    }
}
