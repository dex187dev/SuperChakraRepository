using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace SuperChakra.system
{
    public class ChakraSystem
    {
        // DATENQUELLEN

        private readonly ContentControl _container;
        private readonly ChakraSQL _sqlService;
        private readonly ChakraApp _superChakra;
        private WebView2 _webView;

        // FELDER

        // KONSTRUKTOR

        public ChakraSystem(ContentControl container, ChakraSQL sqlService, ChakraApp app) 
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _sqlService = sqlService ?? throw new ArgumentNullException(nameof(sqlService));
            _superChakra = app ?? throw new ArgumentNullException(nameof(app));
        }

        // METHODEN

        public async Task InitializeAsync(EventHandler<CoreWebView2WebMessageReceivedEventArgs> messageReceivedHandler)
        {
            try
            {
                var dynamicWebView = new WebView2();
                dynamicWebView.HorizontalAlignment = HorizontalAlignment.Stretch;
                dynamicWebView.VerticalAlignment = VerticalAlignment.Stretch;

                _container.Content = dynamicWebView;
                _webView = dynamicWebView;

                var options = new CoreWebView2EnvironmentOptions(
                    "--disable-web-security --allow-insecure-localhost --allow-file-access-from-files"
                );

                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SuperChakra_Cache"
                );

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
                await dynamicWebView.EnsureCoreWebView2Async(env);

                if (dynamicWebView.CoreWebView2 == null) return;

                dynamicWebView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(15, 15, 15);

                var settings = dynamicWebView.CoreWebView2.Settings;
                settings.IsWebMessageEnabled = true;
                settings.IsScriptEnabled = true;
                settings.AreDefaultContextMenusEnabled = false;

                string folderPath = _superChakra.WebviewDirectory;

                if (Directory.Exists(folderPath))
                {
                    string virtualHostName = "superchakra.local";
                    dynamicWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                        virtualHostName,
                        folderPath,
                        CoreWebView2HostResourceAccessKind.Allow
                    );

                    dynamicWebView.CoreWebView2.Navigate($"http://{virtualHostName}/index.html");
                }
                else
                {
                    MessageBox.Show($"Kritisch: 'webview'-Ordner fehlt unter:\n{folderPath}", "Systemfehler");
                    return;
                }

                dynamicWebView.CoreWebView2.WebMessageReceived += messageReceivedHandler;

                dynamicWebView.NavigationCompleted += async (s, e) =>
                {
                    if (e.IsSuccess)
                    {
                        await ExecuteScriptAsync($"window.setAppVersion('v{_superChakra.Version}');");
                        await RefreshDashboardDataAsync("meta");
                    }
                    else
                    {
                        Debug.WriteLine($"Navigation fehlgeschlagen! Status: {e.WebErrorStatus}");
                    }
                };

                Debug.WriteLine("=== WEBVIEW SERVICE DYNAMISCH INITIALISIERT ===");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 Initialisierungsfehler: {ex.Message}", "Fehler");
            }
        }

        public async Task RefreshDashboardDataAsync(string view)
        {
            try
            {
                if (_webView?.CoreWebView2 == null) return;

                string queryToUse = view switch
                {
                    "meta" => _sqlService.QueryLoadMeta,
                    "binary" => _sqlService.QueryLoadBinary,
                    "entity" => _sqlService.QueryLoadEntity,
                    _ => _sqlService.QueryLoadPhysics
                };

                var data = await Task.Run(() => _sqlService.GetDashBoardData(queryToUse));

                Debug.WriteLine($"=== DEBUG: Service fand {data?.Count ?? 0} Zeilen für Ansicht '{view}' ===");

                string jsonData = JsonSerializer.Serialize(data);
                string safeJson = HttpUtility.JavaScriptStringEncode(jsonData);

                await ExecuteScriptAsync($"window.renderTable('{safeJson}');");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Refresh Service Fehler: {ex.Message}");
            }
        }

        public async Task ExecuteScriptAsync(string script)
        {
            if (_webView?.CoreWebView2 == null) return;

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (_webView?.CoreWebView2 != null)
                {
                    await _webView.CoreWebView2.ExecuteScriptAsync(script);
                }
            });
        }

    }
}

