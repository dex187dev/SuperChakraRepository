using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using SuperChakra.main;
using SuperChakra.system;
using SuperChakra.system.panel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Web;
using System.Text;

namespace SuperChakra
{
    public partial class MainWindow : Window
    {
        // DATENQUELLEN

        private readonly ChakraSecurity securityService = new ChakraSecurity();
        private readonly ChakraApp app = new ChakraApp();
        private readonly ChakraSQL sqlService;
        private readonly ChakraFile fileService;

        // FELDER

        private ChakraSystem system;

        // KONSTRUKTOR

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            sqlService = new ChakraSQL(app);
            fileService = new ChakraFile(app);

            this.Title = $"{app.AppName} - {app.Version}{(app.IsDebugMode ? " (DEBUG)" : "")}";

        }

        // METHODEN

        // Hilfsmethoden

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e) 
        {
            system = new ChakraSystem(webViewContainer, sqlService, app);
            await system.InitializeAsync(OnWebMessageReceived);
        }

        // Methoden

        // WebView

        private async void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var jsonString = e.WebMessageAsJson;
                using var doc = JsonDocument.Parse(jsonString);

                string command = doc.RootElement.GetProperty("command").GetString();
                string activeView = doc.RootElement.TryGetProperty("view", out var viewProp) ? viewProp.GetString() : "physics";

                if (!securityService.IsViewAllowed(activeView))
                {
                    await system.ExecuteScriptAsync("alert('Sicherheitswarnung: Ungültige Ansicht angefordert!');");
                    return;
                }

                switch (command)
                {
                    case "refresh":

                        await system.RefreshDashboardDataAsync(activeView);
                        return;

                    case "app_exit":

                        Application.Current.Shutdown();
                        return;

                    case "calc":

                        var physicsRow = new List<ChakraPanelCalcPhysics>();

                        for (int i = 0; i < 7; i++)
                        {
                            ChakraPhysics sourceObj = new ChakraPhysics(i);
                            physicsRow.Add(new ChakraPanelCalcPhysics(sourceObj));
                        }

                        ChakraBinary coreBinary = new ChakraBinary(0);
                        var calcRows = new List<ChakraPanelCalc>
                        {
                            new ChakraPanelCalc("Sequence_01", coreBinary),
                            new ChakraPanelCalc("Sequence_02", coreBinary),
                            new ChakraPanelCalc("CombinedSequence_01_02", coreBinary),
                            new ChakraPanelCalc("CombinedSequence_03", coreBinary)
                        };

                        var responseData = new { calcData = calcRows, physicsData = physicsRow };
                        var jsonPayload = JsonSerializer.Serialize(responseData);

                        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);
                        string base64Payload = Convert.ToBase64String(jsonBytes);

                        await system.ExecuteScriptAsync($"window.updateCalcPanelBase64('{base64Payload}');");
                        return;

                    case "export_excel":

                        return;

                    case "request_graphics_data":

                        ChakraPanelGraphics panelGraphics = new ChakraPanelGraphics(this.app);
                        string jsonResult = panelGraphics.GetGraphicsDataJson();
                        string escapedJson = HttpUtility.JavaScriptStringEncode(jsonResult);

                        await system.ExecuteScriptAsync($"window.renderChakraCharts('{escapedJson}');");
                        return;
                }

                var parts = command.Split('_');
                if (parts.Length >= 2)
                {
                    string category = parts[0];
                    string action = parts[1];

                    await Task.Run(() =>
                    {
                        switch (category)
                        {
                            case "db":
                                if (action == "create") sqlService.CreateDatabaseFile();
                                if (action == "drop") sqlService.DropDatabaseFile();
                                break;

                            case "tables":
                                if (action == "create")
                                {
                                    string script = activeView switch
                                    {
                                        "meta" => sqlService.QueryInitTablesMeta,
                                        "physics" => sqlService.QueryInitTablesPhysics,
                                        "binary" => sqlService.QueryInitTablesBinary,
                                        _ => ""
                                    };
                                    sqlService.ExecuteSQLScript(script);
                                }

                                if (action == "drop")
                                {
                                    string script = activeView switch
                                    {
                                        "meta" => sqlService.QueryDropTablesMeta,
                                        "physics" => sqlService.QueryDropTablesPhysics,
                                        "binary" => sqlService.QueryDropTablesBinary,
                                        _ => ""
                                    };
                                    sqlService.ExecuteSQLScript(script);
                                }
                                break;

                            case "seed":
                                if (action == "all")
                                {
                                    sqlService.ExecuteSQLScript(sqlService.QuerySeedMeta);

                                    string script = activeView switch
                                    {
                                        "physics" => sqlService.QuerySeedPhysics,
                                        "binary" => sqlService.QuerySeedBinary,
                                        _ => ""
                                    };
                                    sqlService.ExecuteSQLScript(script);
                                }
                                break;

                            case "pop":
                                if (action == "last")
                                {
                                    sqlService.DeleteLastRow(activeView);
                                }
                                break;
                        }
                    });

                    if (category == "tables" && action == "create")
                        await system.ExecuteScriptAsync($"alert('Tabelle für {activeView} wurde erfolgreich angelegt');");
                    if (category == "tables" && action == "drop")
                        await system.ExecuteScriptAsync($"alert('Tabelle für {activeView} wurde erfolgreich gelöscht');");
                    if (category == "seed" && action == "all")
                        await system.ExecuteScriptAsync($"alert('Daten für {activeView} wurden erfolgreich geladen');");
                    if (category == "pop" && action == "last")
                        await system.ExecuteScriptAsync($"alert('Die letzte Zeile für {activeView} wurde erfolgreich gelöscht.');");

                    await Task.Delay(250);
                    await system.RefreshDashboardDataAsync(activeView);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainWindow Core: {ex.Message}");
            }
        }

        private async void CalcAndShow() 
        {            
            ChakraPhysics previousChakra = null;

            var results = await Task.Run(() => {

                var list = new List<object>();

                for (int i = 0; i < 7; i++)
                {
                    ChakraPhysics cp = new ChakraPhysics(i);

                    double d = cp.DrehimpulsCalc();
                    double h = cp.HuellvolumenCalc();

                    double p = (previousChakra != null)
                        ? cp.PhasenverschiebungCalc(previousChakra)
                        : 0;

                    list.Add(new
                    {
                        Index = i,
                        Name = cp.Name,
                        Drehimpuls = d,
                        Huellvolumen = h,
                        Phasenverschiebung = p
                    });

                    previousChakra = cp;
                }

                return list;
            });
            
            string jsonResults = JsonSerializer.Serialize(results);
            //await webView.CoreWebView2.ExecuteScriptAsync($"displayAnalysisResults({jsonResults})");
        }

        private async void CalcAndSaveToExcel() 
        {
            try
            {
                string filePath = fileService.RequestSavePathFromUser("ChakraPhysics");

                if (string.IsNullOrEmpty(filePath)) return;

                await Task.Run(() =>
                {
                    var physicsList = new List<ChakraPhysics>();
                    for (int i = 0; i < 7; i++)
                    {
                        physicsList.Add(new ChakraPhysics(i));
                    }
                    
                });
                
                MessageBox.Show("Export successfull", "Excel Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Error: {ex.Message}", "Error: ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}

