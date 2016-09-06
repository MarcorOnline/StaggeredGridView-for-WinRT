using Sample.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sample
{
    public sealed partial class MainPage : Page
    {
        private List<SampleItem> _fakeInternetSource = new List<SampleItem>() {
            new SampleItem(427, 640, "427x640.jpg"),
            new SampleItem(640, 320, "640x320.jpg"),
            new SampleItem(640, 359, "640x359.jpg"),
            new SampleItem(491, 640, "491x640.jpg"),
            new SampleItem(640, 394, "640x394.jpg"),
            new SampleItem(640, 400, "640x400.jpg"),
            new SampleItem(640, 420, "640x420.jpg"),
            new SampleItem(640, 422, "640x422.jpg"),
            new SampleItem(640, 423, "640x423 (2).jpg"),
            new SampleItem(640, 425, "640x425.jpg"),
            new SampleItem(640, 426, "640x426 (2).jpg"),
            new SampleItem(640, 426, "640x426 (3).jpg"),
            new SampleItem(640, 426, "640x480.jpg"),
            new SampleItem(640, 426, "640x426 (4).jpg"),
            new SampleItem(640, 426, "640x426 (5).jpg"),
            new SampleItem(640, 426, "640x426.jpg"),
            new SampleItem(640, 426, "640x446.jpg"),
            new SampleItem(640, 426, "640x448.jpg"),
            new SampleItem(640, 426, "640x480 (2).jpg"),

            new SampleItem(427, 640, "427x640.jpg"),
            new SampleItem(640, 320, "640x320.jpg"),
            new SampleItem(640, 359, "640x359.jpg"),
            new SampleItem(640, 394, "640x394.jpg"),
            new SampleItem(491, 640, "491x640.jpg"),
            new SampleItem(640, 400, "640x400.jpg"),
            new SampleItem(640, 420, "640x420.jpg"),
            new SampleItem(640, 422, "640x422.jpg"),
            new SampleItem(640, 423, "640x423 (2).jpg"),
            new SampleItem(640, 425, "640x425.jpg"),
            new SampleItem(640, 426, "640x426 (2).jpg"),
            new SampleItem(640, 426, "640x426 (3).jpg"),
            new SampleItem(640, 426, "640x480.jpg"),
            new SampleItem(640, 426, "640x426 (4).jpg"),
            new SampleItem(640, 426, "640x426 (5).jpg"),
            new SampleItem(640, 426, "640x426.jpg"),
            new SampleItem(640, 426, "640x446.jpg"),
            new SampleItem(640, 426, "640x448.jpg"),
            new SampleItem(640, 426, "640x480 (2).jpg"),

            new SampleItem(427, 640, "427x640.jpg"),
            new SampleItem(640, 320, "640x320.jpg"),
            new SampleItem(640, 359, "640x359.jpg"),
            new SampleItem(640, 394, "640x394.jpg"),
            new SampleItem(491, 640, "491x640.jpg"),
            new SampleItem(640, 400, "640x400.jpg"),
            new SampleItem(640, 420, "640x420.jpg"),
            new SampleItem(640, 422, "640x422.jpg"),
            new SampleItem(640, 423, "640x423 (2).jpg"),
            new SampleItem(640, 425, "640x425.jpg"),
            new SampleItem(640, 426, "640x426 (2).jpg"),
            new SampleItem(640, 426, "640x426 (3).jpg"),
            new SampleItem(640, 426, "640x480.jpg"),
            new SampleItem(640, 426, "640x426 (4).jpg"),
            new SampleItem(640, 426, "640x426 (5).jpg"),
            new SampleItem(640, 426, "640x426.jpg"),
            new SampleItem(640, 426, "640x446.jpg"),
            new SampleItem(640, 426, "640x448.jpg"),
            new SampleItem(640, 426, "640x480 (2).jpg"),

            new SampleItem(427, 640, "427x640.jpg"),
            new SampleItem(640, 320, "640x320.jpg"),
            new SampleItem(640, 359, "640x359.jpg"),
            new SampleItem(640, 394, "640x394.jpg"),
            new SampleItem(491, 640, "491x640.jpg"),
            new SampleItem(640, 400, "640x400.jpg"),
            new SampleItem(640, 420, "640x420.jpg"),
            new SampleItem(640, 422, "640x422.jpg"),
            new SampleItem(640, 423, "640x423 (2).jpg"),
            new SampleItem(640, 425, "640x425.jpg"),
            new SampleItem(640, 426, "640x426 (2).jpg"),
            new SampleItem(640, 426, "640x426 (3).jpg"),
            new SampleItem(640, 426, "640x480.jpg"),
            new SampleItem(640, 426, "640x426 (4).jpg"),
            new SampleItem(640, 426, "640x426 (5).jpg"),
            new SampleItem(640, 426, "640x426.jpg"),
            new SampleItem(640, 426, "640x446.jpg"),
            new SampleItem(640, 426, "640x448.jpg"),
            new SampleItem(640, 426, "640x480 (2).jpg"),
        };

        private PaginableItemsSource<SampleItem> PaginableItemsSource;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PaginableItemsSource = new PaginableItemsSource<SampleItem>((offset, count) => {
                return DownloadItems(offset, count);
            });
            
            HomeGridView.DataContext = PaginableItemsSource;
            //NOTE: you can use also a simple ObservableCollection or another type of ItemsSource if you don't want pagination

            await PaginableItemsSource.LoadMoreItemsAsync(40);
        }

        private async Task<IEnumerable<SampleItem>> DownloadItems(uint offset, uint count)
        {
            //simulate internet delay
            await Task.Delay(2000);

            //return items
            return _fakeInternetSource.Skip((int)offset).Take((int)count);
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await new MessageDialog("Clicked: " + ((sender as FrameworkElement).DataContext as SampleItem).ImageUrl).ShowAsync();
        }

        private async void HomeGridView_PullToRefreshRequested(object sender, EventArgs e)
        {
            PaginableItemsSource.Clear();
            await PaginableItemsSource.LoadMoreItemsAsync(40);
            HomeGridView.CompletePullToRefresh();
        }
    }
}