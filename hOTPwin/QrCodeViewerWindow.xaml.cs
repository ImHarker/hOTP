using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hOTPwin {
	/// <summary>
	/// Interaction logic for QrCodeViewerWindow.xaml
	/// </summary>
	public partial class QrCodeViewerWindow : Window {
		public QrCodeViewerWindow(TOTPwin totp) {
			InitializeComponent();
			qrcode.Source = BitmapToImageSource(totp.GenerateQrCode());
			issuer.Text = totp.Issuer;
			acc.Text = totp.Account;
			qrcode.Effect = new BlurEffect{KernelType = KernelType.Gaussian, RenderingBias = RenderingBias.Quality, Radius = 25};
		}


		private BitmapImage BitmapToImageSource(Bitmap bitmap) {
			using (MemoryStream memory = new MemoryStream()) {
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				return bitmapimage;
			}
		}
		private void Qrcode_OnMouseLeave(object sender, MouseEventArgs e) {
			qrcode.Effect = new BlurEffect { KernelType = KernelType.Gaussian, RenderingBias = RenderingBias.Quality, Radius = 25 };
			txt.Opacity = 0xff;
			txt1.Opacity = 0xff;
			txt2.Opacity = 0xff;

		}

		private void QrcodeOnClick(object sender, RoutedEventArgs e) {
			qrcode.Effect = null;
			txt.Opacity = 0;
			txt1.Opacity = 0;
			txt2.Opacity = 0;

		}
	}
}
