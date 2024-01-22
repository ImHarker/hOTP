using hOTPcommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hOTPwin {
	public class Card : INotifyPropertyChanged {
		public string? Issuer { get; set; }
		public string? Account { get; set; }
		private string code;
		public string Code {
			get { return code; }
			set {
				if (code != value) {
					code = value;
					OnPropertyChanged(nameof(Code));
				}
			}
		}

		private long timeRemaining;
		public long TimeRemaining {
			get { return timeRemaining; }
			set {
				if (timeRemaining != value) {
					timeRemaining = value;
					OnPropertyChanged(nameof(TimeRemaining));
				}
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
