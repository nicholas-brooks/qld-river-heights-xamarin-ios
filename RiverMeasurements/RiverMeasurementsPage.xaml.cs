using Xamarin.Forms;
using Xamarin.Forms.Maps;
using RiverMeasurements.Bom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RiverMeasurements
{
	public partial class RiverMeasurementsPage : ContentPage
	{
		const double SUNSHINE_COAST_LAT = -26.652477;
		const double SUNSHINE_COAST_LONG = 153.090242;

		private bool initialising;

		public RiverMeasurementsPage()
		{
			initialising = true;
			InitializeComponent();

			MyMap.MoveToRegion(
				MapSpan.FromCenterAndRadius(
					new Position(SUNSHINE_COAST_LAT, SUNSHINE_COAST_LONG), Distance.FromKilometers(10)));
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (initialising)
			{
				initialising = false;
				await LoadPins();
			}
		}

		private async Task LoadPins()
		{
			var data = await GetData.getMeasurementsAsync;

			foreach (var item in data)
			{
				MyMap.Pins.Add(new Pin
				{
					Label = item.Data.Name,
					Position = new Position(item.Station.Latitude, item.Station.Longitude),
					Address = GetAddress(item.Data)
				});
			}
		}

		private string GetAddress(RiverHeights.RiverHeightMeasurement measurement)
		{
			return $"{measurement.FloodClass}; {measurement.Height}; {measurement.Tendency}; {GetTakenTime(measurement)}";
		}

		private string GetTakenTime(RiverHeights.RiverHeightMeasurement measurement)
		{
			if (measurement.Taken.HasValue)
				return measurement.Taken.Value.ToString("h:mm tt ddd");
			else
				return "?";
		}
	}
}

