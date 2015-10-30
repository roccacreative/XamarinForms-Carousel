using System;
using System.Threading;
using Xamarin.Forms;

namespace RoccaCarousel
{
	public class TimerWrapper
	{
		private readonly TimeSpan timespan;
		private readonly Action callback;
		private readonly bool shouldRepeat;

		private CancellationTokenSource cancellation;

		public TimerWrapper(TimeSpan timespan, bool shouldRepeat, Action callback)
		{
			this.timespan = timespan;
			this.callback = callback;
			this.shouldRepeat = shouldRepeat;
			this.cancellation = new CancellationTokenSource();
		}

		public void Start()
		{


			CancellationTokenSource cts = this.cancellation; // safe copy
			Device.StartTimer(this.timespan, 
				() => {
					if (cts.IsCancellationRequested || !shouldRepeat) return false;

					this.callback.Invoke();

					if (shouldRepeat) {
						return true; // or true for periodic behavior
					} else {
						return false;
					}
				});
		}

		public void Stop()
		{
			Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
		}
	}
}

