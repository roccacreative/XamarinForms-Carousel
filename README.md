# Xamarin.Forms ManualCarouselView
## Introduction
MCV is a view for Xamarin.Forms which replicates an animated carousel view, but isn't controlled by gestures. It can be controlled from anywhere with dedicated methods. It also supports events to suggest when a page is appearing, and disappearing for updates. Pages can be any class which inherits from Layout.

The main advantages of this library are that:
* This View is not a Page layout, it is a View layout, so it is not limited to full-page layouts. An  example of this, would be the creation of a LiveTile-like interface with interactive and updating tiles.
* It does not require the use of custom-renderers, working perfectly on iOS and Android *(It has not been tested on Windows Phone)*

## Getting Started
To get started, the **ManualCarouselView.cs** file is the only file that is needed to utilise the view. 

```C#
ManualCarouselView _carousel = new ManualCarouselView {
	Pages = new List<Layout> ()
};
```

Then populate the Pages list with layout data to be displayed in the view.
```C#
_carousel.Pages.Add (new Label());
_carousel.Pages.Add (new BoxView());
_carousel.Pages.Add (_myCustomLayout);
```

Once the pages have been added, the *Initialise* method must be called to set the starting page and initialise the internal layout objects. *A background colour can be specified as an optional second parameter.*
```C#
_carousel.Initialise(0);
```
The starting page should be a positive number that represents the page in the order that they were added to the Pages array list.

Finally, add the carousel view to the overall Page layout or the layout of another view or whatever.
```C#
Content = _carousel;
```

## Working with the Carousel
Changing pages with the carousel is easy.

With **AdvancePage**, the page can easily be advanced in either a positive or negative direction to to forward or backwards. 
```C#
_carousel.AdvancePage(1); // Go Forward one page
_carousel.AdvancePage(-2); // Go Back two pages
```
If necessary, the page can actually be set using **SetPage** where it is possible to designate the desired page as well as whether it should enter from the right or the left.
```C#
_carousel.SetPage(pageNum, 1); // Go to page and slide in from the right-hand side.
_carousel.SetPage(pageNum, -1); // Go to page and slide in from the left-hand side.
```

It is also possible to access the current page for whatever reason by viewing the *CurrentPage* property
```C#
_carousel.CurrentPage;
```

## Page Changing Notifications
To hook into the page events, Page layouts should implement the IManualCarouselPage interface to grant the Carousel the ability to call methods at key page change moments. This could be useful for all sorts of things, such as updating the page's data before the page is revealed. Or perhaps just after it has been revealed.

```C#
public class MyLayout : ContentView, IManualCarouselPage
{
#region IManualCarouselPage implementation
    	public void OnPageAppearing () {}
    	public void OnPageDisappearing () {}
    	public void OnPageAppeared () {}
    	public void OnPageDisappeared () {}
#endregion
}
```

## Known Issues
* Creating a carousel with only a single page can cause problems in certain circumstances, such as if nesting a carousel view within another carousel.