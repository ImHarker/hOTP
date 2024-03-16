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

namespace hOTPwin {
	/// <summary>
	/// Interaction logic for PasswordInput.xaml
	/// </summary>
	public partial class PasswordInput : Window {

		public string? Password { get; private set; }
		public PasswordInput() {
			InitializeComponent();
		}

		private void Submit_OnClick(object sender, RoutedEventArgs e) {
			if (String.IsNullOrEmpty(pwd.Password)) {
				MessageBox.Show("Password cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			Password = pwd.Password;
			this.DialogResult = true;
		}

		private void Pwd_OnKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				Submit_OnClick(sender, e);
			}
		}
	}
}
