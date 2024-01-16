using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using hOTPcommon;

namespace hOTPwin {
	/// <summary>
	/// Interaction logic for ManualImportWindow.xaml
	/// </summary>
	public partial class ManualImportWindow : Window {

		public string? Account { get; set; }
		public string? Issuer { get; set; }
		public string? SecretKey { get; set; }

		public HashAlgorithm Algorithm { get; set; } = HashAlgorithm.SHA1;
		public Period Period { get; set; } = Period.Thirty;
		public Digits Digits { get; set; } = Digits.Six;
		
		public TOTPwin? TOTP { get; set; }
		public ManualImportWindow() {
			InitializeComponent();
			DataContext = this;
		}

		private void ImportButton_OnClick(object sender, RoutedEventArgs e) {
			TOTP = new TOTPwin(Algorithm, SecretKey, Period, Digits, Issuer, Account);
			this.DialogResult = true;
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
			this.Close();
		}
	}
}
