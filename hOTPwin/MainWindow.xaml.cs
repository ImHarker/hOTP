using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using hOTPcommon;

namespace hOTPwin {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public ObservableCollection<TOTPwin> TOTPList { get; set; }
		private static Random random = new Random();

		public MainWindow() {
			InitializeComponent();

			DataContext = this;

			// Initialize the list of cards
			TOTPList = GenerateTestData();
			

		}





		private ObservableCollection<TOTPwin> GenerateTestData() {
			var testData = new ObservableCollection<TOTPwin>();

			for (int i = 1; i <= 10; i++) {
				testData.Add(new TOTPwin(HashAlgorithm.SHA256, null, random.Next(2) == 0 ? Period.ThirtySeconds : Period.SixtySeconds, random.Next(2) == 0 ? Digits.Six : Digits.Eight, GenerateRandomString(), GenerateRandomString()) );
			}

			return testData;
		}

		private string GenerateRandomString() {
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			int length = new Random().Next(5, 17); // Generate a random length between 5 and 16 characters
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[new Random().Next(s.Length)]).ToArray());
		}

	}


}
