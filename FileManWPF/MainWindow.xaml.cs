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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace FileManWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        FileBuilder builder;

        public MainWindow() {
            InitializeComponent();
            builder = new FileBuilder();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            Nullable<bool> user_saves = sfd.ShowDialog();

            if (user_saves == true) {
                FileBuilder fb = new FileBuilder(',', "fileA", "fileB");
                fb.ProcessData(10000, sfd.FileName);
            }
        }

        private void btnFileASelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            Nullable<bool> user_opens = ofd.ShowDialog();

            if (user_opens == true) {
                builder.FileA = new RawFile(ofd.FileName);
                txtBlckA.Text = ofd.FileName;
            }
        }

        private void btnFileBSelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            Nullable<bool> user_opens = ofd.ShowDialog();

            if (user_opens == true) {
                builder.FileB = new RawFile(ofd.FileName);
                txtBlckB.Text = ofd.FileName;
            }
        }

        
    }
}
