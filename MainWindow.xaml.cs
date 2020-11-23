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

namespace StReaderWithVideo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime startTime;
        private DateTime pauseTime;
        private bool isPaused;
        public MainWindow()
        {
            InitializeComponent();
            St.Visibility = Visibility.Hidden;
            isPaused = false;
            Video.Source = new Uri(@"..\..\The.Mandalorian.S02E01.mkv", UriKind.Relative);
            Video.Play();
            startTime = DateTime.Now;
            Console.WriteLine(startTime);
            InitialiseSubtitles(@"..\..\The.Mandalorian.S02E01.srt");
        }

        private void Pause()
        {
            isPaused = true;
            Video.Pause();
            pauseTime = DateTime.Now;
        }

        private void Resume()
        {
            isPaused = false;
            Video.Play();
            startTime += DateTime.Now - pauseTime;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(isPaused)
                Resume();
            else
                Pause();
        }

        async void InitialiseSubtitles(string path)
        {
            Subtitle.ReadSrtFile(path);
            await ManageSubtitle();
        }

        async Task ManageSubtitle()
        {
            for (int i = 0; i < Subtitle.List.Count; i++)
            {
                await DisplaySubtitle(i);
                await ClearSubtitle(i);
            }
        }

        async Task DisplaySubtitle(int index)
        {
            while (DateTime.Now - startTime < Subtitle.List[index].Start || isPaused)
            {
                await Task.Delay(10);
            }

            St.FontStyle = (Subtitle.List[index].IsItalic) ? FontStyles.Italic : FontStyles.Normal;
            
            St.Visibility = Visibility.Visible;
            St.Text = Subtitle.List[index].Text;
        }

        async Task ClearSubtitle(int index)
        {
            while (DateTime.Now - startTime < Subtitle.List[index].End || isPaused)
            {
                await Task.Delay(10);
            }
            St.Visibility = Visibility.Hidden;
            St.Text = "";
        }

    }
}
