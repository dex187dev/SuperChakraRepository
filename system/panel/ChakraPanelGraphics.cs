using SuperChakra.system;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Globalization;

namespace SuperChakra.system.panel
{
    public class ChakraPanelGraphics
    {
        // DATENQUELLEN

        // FELDER

        private readonly ChakraApp _app;

        // UNTERKLASSEN

        public class ChakraChartData 
        {
            public string Name { get; set; }
            public double Dichte { get; set; }
            public double Drehimpuls { get; set; }
            public double Energie { get; set; }
        }        

        // KONSTRUKTOR

        public ChakraPanelGraphics(ChakraApp app) 
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        // METHODEN

        public string GetGraphicsDataJson()
        {
            var chartDataList = new List<ChakraChartData>();
            ChakraSQL db = new ChakraSQL(_app);

            string query = @"
                SELECT m.Name, p.Dichte, p.Drehimpuls, p.Energie 
                FROM ChakraPhysicsData p
                INNER JOIN ChakraMetaData m ON p.[Index] = m.[Index]
                ORDER BY p.[Index] ASC";

            try
            {
                DataTable dt = db.GetDataTable(query);
                System.Diagnostics.Debug.WriteLine($"[Charts-Backend] Zeilen gefunden: {dt.Rows.Count}");

                foreach (DataRow row in dt.Rows)
                {
                    chartDataList.Add(new ChakraChartData
                    {
                        Name = row["Name"].ToString(),
                        Dichte = Convert.ToDouble(row["Dichte"], CultureInfo.InvariantCulture),
                        Drehimpuls = Convert.ToDouble(row["Drehimpuls"], CultureInfo.InvariantCulture),
                        Energie = Convert.ToDouble(row["Energie"], CultureInfo.InvariantCulture)
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Charts-Backend] SQL-FEHLER: {ex.Message}");
                return "[]";
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string finalJson = JsonSerializer.Serialize(chartDataList, options);
            System.Diagnostics.Debug.WriteLine($"[Charts-Backend] Sende an UI: {finalJson}");
            return finalJson;
        }

    }
}

