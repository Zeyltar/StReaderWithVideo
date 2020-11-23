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
        private bool isFullScreen;

        public MainWindow()
        {
            InitializeComponent();
            St.Visibility = Visibility.Hidden;
            SetWindowedScreen();
            isPaused = false;
            Video.Source = new Uri(@"..\..\The.Mandalorian.S02E01.mkv", UriKind.Relative);
            Video.Play();
            startTime = DateTime.Now;
            InitialiseSubtitles(@"..\..\The.Mandalorian.S02E01.srt");
            
        }

        private async void InitialiseSubtitles(string path)
        {
            Subtitle.ReadSrtFile(path);
            await ManageSubtitle();
        }

        private async Task ManageSubtitle()
        {
            for (int i = 0; i < Subtitle.List.Count; i++)
            {
                await DisplaySubtitle(i);
                await ClearSubtitle(i);
            }
        }

        private async Task DisplaySubtitle(int index)
        {
            while (DateTime.Now - startTime < Subtitle.List[index].Start || isPaused)
            {
                await Task.Delay(10);
            }

            St.FontStyle = (Subtitle.List[index].IsItalic) ? FontStyles.Italic : FontStyles.Normal;
            St.Visibility = Visibility.Visible;
            St.Text = Subtitle.List[index].Text;
        }

        private async Task ClearSubtitle(int index)
        {
            while (DateTime.Now - startTime < Subtitle.List[index].End || isPaused)
            {
                await Task.Delay(10);
            }
            St.Visibility = Visibility.Hidden;
            St.Text = "";
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPaused)
                ResumeVideo();
            else
                PauseVideo();
        }

        private void PauseVideo()
        {
            isPaused = true;
            Video.Pause();
            pauseTime = DateTime.Now;
        }

        private void ResumeVideo()
        {
            isPaused = false;
            Video.Play();
            startTime += DateTime.Now - pauseTime;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(!e.IsRepeat)
            {
                switch (e.Key)
                {
                    case Key.F:
                        if (isFullScreen)
                            SetWindowedScreen();
                        else
                            SetFullScreen();
                        break;
                    case Key.Escape:
                        if (isFullScreen)
                            SetWindowedScreen();
                        break;
                    case Key.Space:
                        if (isPaused)
                            ResumeVideo();
                        else
                            PauseVideo();
                        break;
                }
            }
        }

        private void SetFullScreen()
        {
            isFullScreen = true;
            Visibility  = Visibility.Collapsed;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Visibility = Visibility.Visible;
            Topmost = true;
        }

        private void SetWindowedScreen()
        {
            isFullScreen = false;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.CanResize;
            Topmost = false;
        }

    }
}
