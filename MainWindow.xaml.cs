using System.Windows;
using Microsoft.Web.WebView2.Core;
namespace TuNamespace {
    public partial class MainWindow : Window {
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;
        private ResizeMode _previousResizeMode;
        public MainWindow() {
            InitializeComponent();
            InitializeAsync();
        }
        private async void InitializeAsync() {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webView.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
            webView.CoreWebView2.Navigate("https://www.github.com");
        }
        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e) {
            if (!e.IsSuccess)
                return;
            string injectedCSS = @"
                (function() {
                    const css = `
                        /* Ocultar scrollbars visualmente sin romper layout */
                        ::-webkit-scrollbar {
                            width: 0px !important;
                            height: 0px !important;
                        }
                        * {
                            scrollbar-width: none !important; /* Firefox */
                            -ms-overflow-style: none !important; /* IE 10+ */
                        }
                        html, body {
                            margin: 0 !important;
                            padding-bottom: 39px !important; /* Espacio inferior */
                            padding-right: 7px !important;
                            box-sizing: border-box !important;
                            overflow-x: auto !important;
                            overflow-y: auto !important;
                            width: 100% !important;
                            height: 100% !important;
                        }
                    `;
                    let styleTag = document.getElementById('custom-style');
                    if (!styleTag) {
                        styleTag = document.createElement('style');
                        styleTag.id = 'custom-style';
                        document.head.appendChild(styleTag);
                    }
                    styleTag.innerHTML = css;
                })();
            ";
            await webView.ExecuteScriptAsync(injectedCSS);
        }
        private void CoreWebView2_ContainsFullScreenElementChanged(object sender, object e) {
            if (webView.CoreWebView2.ContainsFullScreenElement)
                EnterFullScreen();
            else
                ExitFullScreen();
        }
        private void EnterFullScreen() {
            _previousWindowState = this.WindowState;
            _previousWindowStyle = this.WindowStyle;
            _previousResizeMode = this.ResizeMode;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;
        }
        private void ExitFullScreen() {
            this.WindowStyle = _previousWindowStyle;
            this.ResizeMode = _previousResizeMode;
            this.WindowState = _previousWindowState;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            webView.Width = e.NewSize.Width;
            webView.Height = e.NewSize.Height;
        }
    }
}