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

namespace WBR
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary> 
    public partial class SaveWindow : Window
    {
        private bool FocusedOnce = false;
        private List<List<byte>> ByteList;
        public SaveWindow(int vid, int pid, List<List<byte>> byteList)
        {
            InitializeComponent();
            ByteList = byteList;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text) || !FocusedOnce) return;
            DevicePresets.Add(NameInput.Text, ByteList);
            this.Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NameGotFocus(object sender, RoutedEventArgs e)
        {
            if (!FocusedOnce)
                NameInput.Text = "";
            FocusedOnce = true;
        }
    }
}
