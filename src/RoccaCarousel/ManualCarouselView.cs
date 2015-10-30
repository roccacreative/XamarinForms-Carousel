using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Forms
{
	public interface IManualCarouselPage {
		void OnPageAppearing ();
		void OnPageDisappearing ();

		void OnPageAppeared ();
		void OnPageDisappeared ();
	}

	public class ManualCarouselView : ContentView
	{
		/* This list should contain all the pages you wish to display 
		 * in your carousel, any ContentView should work fine.
		 * It should contain at least 1 page to work */
		public List<Layout> Pages;
		public int CurrentPage {
			get {
				return _currentPage;
			}
		}
		int _currentPage = 0;

		#region Setup for Carousel view
		bool _animating = false;
		bool _usingAltPage = false;
		bool _initialised = false;

		uint pageAnimationTime = 250;

		Grid _baseLayout;
		ContentView _mainPage;
		ScrollView _mainPageSV;
		ContentView _altPage;
		ScrollView _altPageSV;

		Rectangle _dimen;
		#endregion

		public ManualCarouselView ()
		{
			#region Carousel Base Layout
			_baseLayout = new Grid () {
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) }
				},
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height = new GridLength (1, GridUnitType.Star) }
				},
				ColumnSpacing = 0,
				RowSpacing = 0,
				IsClippedToBounds = true
			};
			Content = _baseLayout;
			#endregion
		}

		// Initialise so the Carousel may prepare itself with your first page, ready for viewing.
		public void Initialise(int StartPage = 0, Color? backgroundcolor = null) {
			if (Pages == null || Pages.Count == 0) {
				throw new Exception ("At least one page must be provided to the carousel layout");
			} else if (StartPage >= Pages.Count) {
				throw new Exception ("StartPage cannot exceed layout pages in array.");
			} else {
				#region Carousel Slide Initialisation
				/* It is important to call the initailise method after instanciating the ManualCarouselView
		 		* object so the first page may contain the content you wish to display. */

				_mainPage = new ContentView () {
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Content = Pages [StartPage]
				};
				_mainPageSV = new ScrollView () {
					Orientation = ScrollOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Content = _mainPage
				};
				_altPage = new ContentView () {
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Content = new ContentView ()
				};
				_altPageSV = new ScrollView () {
					Orientation = ScrollOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Content = _altPage
				};

				/* It's important that the first and second pages are added to the layout, on top of
				 * eachother so they will be sized correctly when animating in the future */
				_baseLayout.Children.Add (_altPageSV, 0, 1, 0, 1);
				_baseLayout.Children.Add (_mainPageSV, 0, 1, 0, 1);
				#endregion

				if (backgroundcolor.HasValue) {
					_mainPageSV.BackgroundColor = backgroundcolor.Value;
					_altPageSV.BackgroundColor = backgroundcolor.Value;
					BackgroundColor = backgroundcolor.Value;
				}

				// As we are showing the first page for the first time, we should trigger the "OnPageAppearing" code
				var iPgContent = Pages [StartPage] as IManualCarouselPage;
				Page_Appearing (iPgContent);
				Page_Appeared (iPgContent);

				//Content = _baseLayout;
				_initialised = true;
			}
		}

		#region Carousel Navigation
		public void AdvancePage(int step) {
			if (!_initialised) {
				throw new Exception ("ManualCarouselView has not been initialised");
			} else {
				if (!_animating) {
					// Increase the page
					_currentPage += step;

					SetPage (_currentPage, (step >= 0 ? 1 : -1));
				}
			}
		}
		public void SetPage(int page, int direction) {
			if (!_initialised) {
				throw new Exception ("ManualCarouselView has not been initialised");
			} else {
				if (!_animating) {
					// Prevent users from mashing the buttons when the app isnt ready.
					_animating = true;

					// Increase the page
					_currentPage = page;

					// Wrap pages around: (might not be necessary)
					if (_currentPage > Pages.Count - 1) {
						_currentPage = 0;
					} else if (_currentPage < 0) {
						_currentPage = Pages.Count - 1;
					}

					ScrollView mainpgsv;
					ScrollView nextpgsv;
					ContentView mainpg;
					ContentView nextpg;

					// Prepare references to the pages to prepare for animation.
					if (!_usingAltPage) {
						mainpgsv = _mainPageSV;
						mainpg = _mainPage;

						nextpgsv = _altPageSV;
						nextpg = _altPage;
					} else {
						mainpgsv = _altPageSV;
						mainpg = _altPage;

						nextpgsv = _mainPageSV;
						nextpg = _mainPage;
					}
					_usingAltPage = !_usingAltPage;

					// Prepare view dimentions each time the page changes so layout alterations are not then fixed.
					_dimen = new Rectangle (mainpgsv.X, mainpgsv.Y, mainpgsv.Width, mainpgsv.Height);

					// Prepare next page, move into position and scroll appropriately.
					var cpageModel = Pages [_currentPage];
					nextpg.Content = cpageModel;

					// Aniamtion is about to occurr, perform the 'OnAppearing' code
					var iNextPgContent = nextpg.Content as IManualCarouselPage;
					var iMainPgContent = mainpg.Content as IManualCarouselPage;
					Page_Appearing (iNextPgContent);
					Page_Disappearing (iMainPgContent);

					// Inform the animation framework that we have a batch of aniamtions to perform
					mainpgsv.BatchBegin ();
					nextpgsv.BatchBegin ();


					nextpgsv.ScrollToAsync (0, 0, false);
					if (direction >= 0) {
						nextpgsv.Layout (new Rectangle (_dimen.X + _dimen.Width, _dimen.Y, _dimen.Width, _dimen.Height));
					} else {
						nextpgsv.Layout (new Rectangle (_dimen.X - _dimen.Width, _dimen.Y, _dimen.Width, _dimen.Height));
					}

					/* This is very important. Without it, interacting with entry elements in the view would cause 
					 * the current page to swap beneath the previous page. This presumably is to do with the fact
					 * that animations dont layout the views as android normally would, so they snap back into their
					 * calculated positions when something (such as the keybaord) would cause the views to recalcualte. */
					_baseLayout.RaiseChild(nextpgsv);

					if (direction >= 0) {
						mainpgsv.LayoutTo (new Rectangle (_dimen.X - _dimen.Width, _dimen.Y, _dimen.Width, _dimen.Height), pageAnimationTime, Easing.CubicInOut);
						nextpgsv.LayoutTo (new Rectangle (_dimen.X, _dimen.Y, _dimen.Width, _dimen.Height), pageAnimationTime, Easing.CubicInOut).ContinueWith ((Task arg1) => {
							_animating = false;
							// Aniamtion is ending, alert the events
							Page_Disappeared(iMainPgContent);
							Page_Appeared(iNextPgContent);
						});
					} else {
						mainpgsv.LayoutTo (new Rectangle (_dimen.X + _dimen.Width, _dimen.Y, _dimen.Width, _dimen.Height), pageAnimationTime, Easing.CubicInOut);
						nextpgsv.LayoutTo (new Rectangle (_dimen.X, _dimen.Y, _dimen.Width, _dimen.Height), pageAnimationTime, Easing.CubicInOut).ContinueWith ((Task arg1) => {
							_animating = false;
							// Aniamtion is ending, alert the events
							Page_Disappeared(iMainPgContent);
							Page_Appeared(iNextPgContent);
						});
					}

					// Commit the animations and begin!
					mainpgsv.BatchCommit ();
					nextpgsv.BatchCommit ();
				}
			}
		}

		#endregion

		#region Navigation Events
		private void Page_Appearing(IManualCarouselPage page) {
			if (page != null) {
				page.OnPageAppearing ();
			}
		}
		private void Page_Appeared(IManualCarouselPage page) {
			if (page != null) {
				page.OnPageAppeared ();
			}
		}
		private void Page_Disappearing(IManualCarouselPage page) {
			if (page != null) {
				page.OnPageDisappearing ();
			}
		}
		private void Page_Disappeared(IManualCarouselPage page) {
			if (page != null) {
				page.OnPageDisappeared ();
			}
		}

		#endregion
	}
}


