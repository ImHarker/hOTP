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
using System.Windows.Controls.Primitives;
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
		private bool isDeleteMode = false;

		public MainWindow() {
			InitializeComponent();

			DataContext = this;

			// Initialize the list of cards
			TOTPList = GenerateTestData();
			listBox.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
		}

		private void DeleteMode_OnClick(object? sender, RoutedEventArgs e) {
			ToggleDeleteMode();
		}

		private void ToggleDeleteMode() {
			if (listBox.Items.Count == 0) return;
			isDeleteMode = !isDeleteMode;

			listBox.UpdateLayout();

			for (int i = 0; i < listBox.Items.Count; i++) {
				ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);
				if (listBoxItem != null) {
					SetDeleteModeForLoadedItems(listBoxItem);
				}
			}
		}
		private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e) {
			if (listBox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) {
				foreach (var item in listBox.Items) {
					ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item);
					if (listBoxItem != null) {
						SetDeleteModeForLoadedItems(listBoxItem);
					}
				}
			}
		}

		private void SetDeleteModeForLoadedItems(ListBoxItem listBoxItem) {
			if (listBoxItem.IsLoaded) {
				DeleteModeBehavior.SetIsDeleteMode(listBoxItem, isDeleteMode);
			}
		}

		private void DeleteButtonClick(object? sender, RoutedEventArgs e) {
			if (sender is FrameworkElement element && element.DataContext is TOTPwin totpItem) {
				var issuer = totpItem?.Card?.Issuer;
				var account = totpItem?.Card?.Account;

				var confirmationMessage = "\nThis action is irreversible.\nAre you sure you want to delete this account?";
				if (!string.IsNullOrEmpty(account)) {
					confirmationMessage = $"Account:\t {account}\n{confirmationMessage}";
				}
				if (!string.IsNullOrEmpty(issuer)) {
					confirmationMessage = $"Issuer:\t {issuer}\n{confirmationMessage}";
				}


				var result = MessageBox.Show(confirmationMessage.TrimStart(), "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

				if (result == MessageBoxResult.Yes) {
					TOTPList.Remove(totpItem);
				}
			}
			if (TOTPList.Count == 0) isDeleteMode = false;
		}




		
		private ObservableCollection<TOTPwin> GenerateTestData() {
			var testData = new ObservableCollection<TOTPwin>();
			Random random = new Random();

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
