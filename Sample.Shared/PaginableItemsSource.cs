using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sample
{
    public class PaginableItemsSource<T> : ObservableCollection<T>, ISupportIncrementalLoading, INotifyPropertyChanged
    {
        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value, () => Loading); }
        }

        private bool _noData;
        public bool NoData
        {
            get { return _noData; }
            set { SetProperty(ref _noData, value, () => NoData); }
        }

        public bool HasMoreItems { get; protected set; }

        private Func<uint, uint, Task<IEnumerable<T>>> getItemsFunction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection">the items collection</param>
        /// <param name="getItemsFunction">download items function, the parameters are offset and count, the function must be return null in case of error, empty list when there are no more items</param>
        public PaginableItemsSource(Func<uint, uint, Task<IEnumerable<T>>> getItemsFunction)
        {
            HasMoreItems = true;
            this.getItemsFunction = getItemsFunction;
        }

        private Task<LoadMoreItemsResult> currentOperation;
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;

            currentOperation = Task.Run(async () =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    Loading = true;
                });

                //invoco su Task asincrono
                var newItems = await getItemsFunction((uint)Count, count);

                uint newItemsCount = 0;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //torno sullo UI thread
                    if (newItems != null)
                        foreach (var item in newItems)
                        {
                            newItemsCount++;
                            Add(item);
                        }

                    Loading = false;

                    if (newItems != null)
                    {
                        HasMoreItems = newItemsCount >= count;
                        NoData = Count == 0;
                    }
                    else
                        HasMoreItems = true;
                });

                uint downloadedCount = newItems == null ? 0 : newItemsCount;

                return new LoadMoreItemsResult() { Count = downloadedCount };
            });

            return currentOperation.AsAsyncOperation();
        }

        protected override void ClearItems()
        {
            if (currentOperation != null && !currentOperation.IsCompleted)
                currentOperation.Wait();
            
            HasMoreItems = true;
            NoData = false;
            base.ClearItems();
        }

        #region INotifyPropertyChanged
        public new event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<S>(ref S storage, S value, String propertyName)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                this.OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetProperty<S>(ref S storage, S value, Expression<Func<S>> expr)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                this.OnPropertyChanged(expr);
            }
        }

        public void OnPropertyChanged<S>(Expression<Func<S>> expr)
        {
            var body = ((MemberExpression)expr.Body);
            string name = body.Member.Name;
            OnPropertyChanged(name);
        }

        #endregion
    }
}