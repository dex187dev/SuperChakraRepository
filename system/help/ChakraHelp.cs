using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using SuperChakra.main;

namespace SuperChakra.system.help
{
    public class ChakraHelp
    {

        // METHODEN

        public static void ExportPhysicsToExcel(List<ChakraPhysics> data, string filePath) 
        {
            var csv = new StringBuilder();
            csv.AppendLine("Name;Drehimpuls;Huellvolumen;Phasenverschiebung");

            ChakraPhysics previous = null;

            foreach (var cp in data)
            {
                string index = cp.Index.ToString();
                string name = cp.Name;

                string d = cp.DrehimpulsCalc().ToString("R", CultureInfo.CurrentCulture);
                string h = cp.HuellvolumenCalc().ToString("R", CultureInfo.CurrentCulture);

                double pVal = (previous != null) ? cp.PhasenverschiebungCalc(previous) : 0;
                string p = pVal.ToString("R", CultureInfo.CurrentCulture);
                string e = cp.Energie.ToString("R", CultureInfo.CurrentCulture);
                csv.AppendLine($"{index};{name};{d};{h};{p};{e}");
                previous = cp;
            }
            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }

    }
}

