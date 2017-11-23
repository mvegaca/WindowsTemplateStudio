﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

using Param_ItemNamespace.Models;

namespace Param_ItemNamespace.Extensions
{
    public static class DataRequestExtensions
    {
        // TODO WTS
        // Step 1. Use the DataTransferManager to register an event handler to DataRequested.
        // This event will be fired when the share operation starts.
        // _dataTransferManager = DataTransferManager.GetForCurrentView();
        // _dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(OnDataRequested);
        //
        // Step2. Code in OnDataRequested event handler.
        // We need to add data to DataRequestedEventArgs through SetData extension method
        // Create a ShareSourceData instance.
        // var config = new ShareSourceData("ADD HERE THE SHARE TITLE");
        // Set the data you want to share
        // i.e. config.SetWebLink(new Uri("https://www.microsoft.com/");
        // i.e. config.SetText("Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
        //
        // Step 3. Call this SetData method befor ending the event handler
        // args.Request.SetData(config);
        // You can also register an event handler to ShareCompleted to run code when the share operation finishes.
        // args.Request.Data.ShareCompleted += OnShareCompleted;
        //
        // Step 4. You can also register an event handler to ShareCompleted
        // to run code when the share operation finishes.
        // Do not forget unregister the event handler.
        // sender.ShareCompleted -= OnShareCompleted;
        public static void SetData(this DataRequest dataRequest, ShareSourceFeatureData config)
        {
            var deferral = dataRequest.GetDeferral();
            try
            {
                var requestData = dataRequest.Data;
                requestData.Properties.Title = config.Title;
                if (!string.IsNullOrEmpty(config.Description))
                {
                    requestData.Properties.Description = config.Description;
                }

                var storageItems = new List<IStorageItem>();
                foreach (var dataItem in config.Items)
                {
                    switch (dataItem.DataType)
                    {
                        case ShareSourceFeatureItemType.Text:
                            requestData.SetText(dataItem.Text);
                            break;
                        case ShareSourceFeatureItemType.WebLink:
                            requestData.SetWebLink(dataItem.WebLink);
                            break;
                        case ShareSourceFeatureItemType.ApplicationLink:
                            requestData.SetApplicationLink(dataItem.ApplicationLink);
                            break;
                        case ShareSourceFeatureItemType.Html:
                            var htmlFormat = HtmlFormatHelper.CreateHtmlFormat(dataItem.Html);
                            requestData.SetHtmlFormat(htmlFormat);
                            break;
                        case ShareSourceFeatureItemType.Image:
                            requestData.FillImage(dataItem.Image, storageItems);
                            break;
                        case ShareSourceFeatureItemType.StorageItems:
                            requestData.FillStorageItems(dataItem.StorageItems, storageItems);
                            break;
                        case ShareSourceFeatureItemType.DeferredContent:
                            requestData.FillDeferredContent(dataItem.DeferredDataFormatId, dataItem.GetDeferredDataAsyncFunc);
                            break;
                    }
                }

                if (storageItems.Any())
                {
                    requestData.SetStorageItems(storageItems);
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        private static void FillImage(this DataPackage requestData, StorageFile image, List<IStorageItem> storageItems)
        {
            storageItems.Add(image);
            var streamReference = RandomAccessStreamReference.CreateFromFile(image);
            requestData.Properties.Thumbnail = streamReference;
            requestData.SetBitmap(streamReference);
        }

        private static void FillStorageItems(this DataPackage requestData, IEnumerable<IStorageItem> sourceItems, List<IStorageItem> storageItems)
        {
            foreach (var item in sourceItems)
            {
                storageItems.Add(item);
            }
        }

        private static void FillDeferredContent(this DataPackage requestData, string deferredDataFormatId, Func<Task<object>> getDeferredDataAsyncFunc)
        {
            requestData.SetDataProvider(deferredDataFormatId, async (providerRequest) =>
            {
                var deferral = providerRequest.GetDeferral();
                try
                {
                    var deferredData = await getDeferredDataAsyncFunc();
                    providerRequest.SetData(deferredData);
                }
                finally
                {
                    deferral.Complete();
                }
            });
        }
    }
}
