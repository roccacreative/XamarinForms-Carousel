using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace RoccaCarousel
{
	public class Example_ExternalControl: ContentPage
	{
		//ManualCarouselView _carousel;

		public Example_ExternalControl ()
		{
			#region Carousel Code

			// Initialise a new Carousel layout
			var carousel = GenerateCarousel();

			// Create a Page using our usual format with buttons on the outside, controlling the nested carousel
			Grid basePage = CreatePage(Color.Default, Color.Default, carousel, carousel);

			// Finally initialise it, this sets the starting page and calculates the size, etc.
			carousel.Initialise (0);

			#endregion

			Title = "Externally Controlled Carousel";

			// Finally, assign the carousel as the page content.
			Content = basePage;
		}

		public ManualCarouselView GenerateCarousel() {
			ManualCarouselView carousel = new ManualCarouselView {
				Pages = new List<Layout> (),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			AddPagesToCarousel(carousel);

			carousel.Initialise (0);

			return carousel;
		}

		public void AddPagesToCarousel(ManualCarouselView carousel) {
			// Add content pages to the carousel (in this instance, the buttons are nested within the carousel)
			carousel.Pages.Add (CreatePage (Color.Maroon, Color.White, new Label() { Text = "Page 1\n" + ExampleStrings.ILikeDogs, TextColor = Color.White }, null));
			carousel.Pages.Add (CreatePage (Color.Navy, Color.White, new Label() { Text = "Page 2\n" + ExampleStrings.WaterMovesFast, TextColor = Color.White }, null));
			carousel.Pages.Add (CreatePage (Color.White, Color.Black, new Label() { Text = "Page 3\n" + ExampleStrings.LysineContingency, TextColor = Color.Black }, null));
		}

		// Here we create basic pages for the views, only we specify the content to display in the main area
		// We also specify which CarouselView we wish to manipulate by passing it in.
		public Grid CreatePage(Color bgColor, Color textColor, View content, ManualCarouselView eventTarget) {

			Grid layout = new Grid {
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) },
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) },
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) }
				},
				RowDefinitions = new RowDefinitionCollection{
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
				},
				BackgroundColor = bgColor
			};
					
			Button goBack = new Button () {
				Text = "Back",
				TextColor = textColor,
				Command = new Command(() => {
					Device.BeginInvokeOnMainThread(() => {
						if (eventTarget != null) {
							eventTarget.AdvancePage(-1);
						}
					});
				})
			};

			Button goForward = new Button () {
				Text = "Next",
				TextColor = textColor,
				Command = new Command(() => {
					Device.BeginInvokeOnMainThread(() => {
						if (eventTarget != null) {
							eventTarget.AdvancePage(1);
						}
					});
				})
			};

			if (eventTarget != null) {
				layout.Children.Add (goBack, 0, 1, 3, 4);
				layout.Children.Add (goForward, 2, 3, 3, 4);
			}
			layout.Children.Add (content, 0, 3, 0, 3);

			return layout;
		}
	}
}

