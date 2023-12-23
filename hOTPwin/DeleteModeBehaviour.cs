using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hOTPwin
{
	public static class DeleteModeBehavior {
		public static readonly DependencyProperty IsDeleteModeProperty =
			DependencyProperty.RegisterAttached("IsDeleteMode", typeof(bool), typeof(DeleteModeBehavior), new PropertyMetadata(false));

		public static bool GetIsDeleteMode(DependencyObject obj) {
			return (bool)obj.GetValue(IsDeleteModeProperty);
		}

		public static void SetIsDeleteMode(DependencyObject obj, bool value) {
			obj.SetValue(IsDeleteModeProperty, value);
		}
	}

}
