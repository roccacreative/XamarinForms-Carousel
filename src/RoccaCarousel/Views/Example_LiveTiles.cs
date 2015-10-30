using System;

using Xamarin.Forms;
using System.Collections.Generic;

namespace RoccaCarousel
{
	public class Example_LiveTiles : ContentPage
	{
		private Point? _dimensions;
		private Grid _baseLayout;
		private DateTime _timerStart;

		// Content variable definitions
		ManualCarouselView box1;
		ContentView 	   box2;
		ManualCarouselView box3;
		ManualCarouselView box4;
		ContentView		   box5;

		TimerWrapper timer;

		public Example_LiveTiles ()
		{
			Content = new ContentView (); // Temp, This will be overwritten shortly

			Title = "Pseudo LiveTiles";

			timer = new TimerWrapper (new TimeSpan (0, 0, 1), true, TimerElapsedEvt);
		}

		private void TimerElapsedEvt() {
			var secondsSinceStart = GetSecondsSinceTimerStart;

			if (secondsSinceStart % 5 == 0) {
				box1.AdvancePage(1);
			}  
			if (secondsSinceStart % 4 == 0) {
				box3.AdvancePage(1);
			} 
			if (secondsSinceStart % 2 == 0) {
				box4.AdvancePage(1);
			} 
		}

		private int GetSecondsSinceTimerStart {
			get {
				TimeSpan lapsedTime = DateTime.Now - _timerStart;
				return lapsedTime.Seconds;
			}
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			_timerStart = DateTime.Now;
			timer.Start ();
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			timer.Stop ();
		}


		protected override void OnSizeAllocated (double width, double height)
		{
			base.OnSizeAllocated (width, height);

			if (!_dimensions.HasValue || (_dimensions.Value.X != width && _dimensions.Value.Y != height)) {
				_dimensions = new Point (width, height);
				Content = GenerateLayout ();
			}
		}

		private View GenerateLayout() {
			_baseLayout = new Grid () {
				ColumnDefinitions = new ColumnDefinitionCollection{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				},
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height = new GridLength(_dimensions.Value.X / 2, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(_dimensions.Value.X / 2, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(_dimensions.Value.X / 2, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(_dimensions.Value.X / 2, GridUnitType.Absolute) }
				}
			};

			box1 = new ManualCarouselView{ Pages = new List<Layout>() }; // LiveTile
			box2 = new ContentView{ BackgroundColor = Color.Green }; // Boring Tile
			box3 = new ManualCarouselView{ Pages = new List<Layout>() }; // Double-Width Live-Tile
			box4 = new ManualCarouselView{ Pages = new List<Layout>() }; // Double-Height Live-Tile
			box5 = new ContentView{ BackgroundColor = Color.Blue }; // Double-Height boring tile

			SetupBox1(box1);
			SetupBox2(box2);
			SetupBox3(box3);
			SetupBox4(box4);
			SetupBox5(box5);


			_baseLayout.Children.Add (box1, 0, 1, 0, 1);
			_baseLayout.Children.Add (box2, 1, 2, 0, 1);
			_baseLayout.Children.Add (box3, 0, 2, 1, 2);
			_baseLayout.Children.Add (box4, 0, 1, 2, 4);
			_baseLayout.Children.Add (box5, 1, 2, 2, 4);

			return new ScrollView {
				Orientation = ScrollOrientation.Vertical,
				Content = _baseLayout,
			};
		}

		private void SetupBox1(ManualCarouselView box) {
			Label lb1 = new Label {
				TextColor = Color.White,
				Text = ExampleStrings.LiveTile1[0]
			};
			Label lb2 = new Label {
				TextColor = Color.White,
				Text = ExampleStrings.LiveTile1[1]
			};
			ContentView pg1 = new ContentView {
				Padding = new Thickness(5),
				BackgroundColor = Color.FromHex("#FF0000"),
				Content = lb1
			};
			ContentView pg2 = new ContentView{
				Padding = new Thickness(5),
				BackgroundColor = Color.FromHex("#840100"),
				Content = lb2
			};

			box.Pages.Add (pg1);
			box.Pages.Add (pg2);
			box.Initialise (0);
		}

		private void SetupBox2(ContentView box) {
			box.Padding = new Thickness (5);
			box.Content = new Label { Text = ExampleStrings.LiveTile2, TextColor = Color.White };
		}

		private void SetupBox3(ManualCarouselView box) {
			Label lb1 = new Label {
				TextColor = Color.Black,
				Text = ExampleStrings.LiveTile3[0]
			};
			Label lb2 = new Label {
				TextColor = Color.White,
				Text = ExampleStrings.LiveTile3[1]
			};
			ContentView pg1 = new ContentView {
				Padding = new Thickness(5),
				BackgroundColor = Color.FromHex("#92E9DC"),
				Content = lb1
			};
			ContentView pg2 = new ContentView{
				Padding = new Thickness(5),
				BackgroundColor = Color.FromHex("#399A8C"),
				Content = lb2
			};

			box.Pages.Add (pg1);
			box.Pages.Add (pg2);
			box.Initialise (0);
		}

		private void SetupBox4(ManualCarouselView box) {
			int timesTileHasChanged = 0;

			Label lb1 = new Label {
				TextColor = Color.White,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = ExampleStrings.LiveTile4[0]
			};
			Label lb2 = new Label {
				TextColor = Color.White,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = ExampleStrings.LiveTile4[0] + "\n\n" + ExampleStrings.LiveTile4[1]
			};
			CarouselPage pg1 = new CarouselPage {
				Padding = new Thickness(5),
				BackgroundColor = Color.FromHex("#FF7C2E"),
				Content = lb1,

			};
			CarouselPage pg2 = new CarouselPage{
				Padding = new Thickness(5),
				BackgroundColor = Color.FromHex("#8C3500"),
				Content = lb2
			};

			// If you extend the standard layout classes to implement the IManualCarouselPage interface
			// Then you can tie events to when the pages change.
			pg1.PageAppearing += () => {
				Device.BeginInvokeOnMainThread(() => {
					lb1.Text = String.Format("{0} {1}", ExampleStrings.LiveTile4[0], ++timesTileHasChanged);
				});
			};
			pg2.PageAppearing += () => {
				Device.BeginInvokeOnMainThread(() => {
					lb2.Text = String.Format("{0} {1}\n\n{2}", ExampleStrings.LiveTile4[0], ++timesTileHasChanged, ExampleStrings.LiveTile4[1]);
				});
			};

			
			box.Pages.Add (pg1);
			box.Pages.Add (pg2);
			box.Initialise (0);
		}

		private void SetupBox5(ContentView box) {
			box.Padding = new Thickness (5);
			box.Content = new Label { Text = ExampleStrings.LiveTile5, TextColor = Color.White };
		}
	}
}


