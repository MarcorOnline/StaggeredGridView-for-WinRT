# StaggeredGridView-for-WinRT
A staggered grid view for Windows apps which supports multiple columns with rows of varying sizes.

Supported platform:
- Windows 8
- Windows 8.1
- Windows Phone 8.1
- Windows UWP (Windows 10 and Windows 10 Mobile)

Phone preview:
![Preview of how it looks on PC](StaggeredGridView-PhoneSample.png?raw=true "Preview of how it looks on PC")

PC preview:
![Preview of how it looks on PC](StaggeredGridView-PCSample.png?raw=true "Preview of how it looks on PC")


Instructions:
- ItemsSource items must implement "IStaggeredGridViewItem" interface to provide "Width" and "Height" of each items
- If you want to support incremental loading the "ItemsSource" must implement "ISupportIncrementalLoading" (you can use the "PaginableItemsSource" in the sample)
- If you want to support pull to refresh (only on touch devices) subscribe to "PullToRefreshRequested" event and when completed call "CompletePullToRefresh" method

Properties and functionalities:
- ItemsSource
- ItemTemplate
- Header
- Footer
- ColumnsNumber
- ColumnsSpacing
- RowsSpacing

Pull to refresh properties:
- IsPullToRefreshEnabled	(it's automatically disabled in no-touch devices)
- RefreshHeaderHeight
- RefreshText
- PullText
- ArrowColor

![Preview of pull to refresh](PullToRefreshSample.png?raw=true "Preview of pull to refresh")