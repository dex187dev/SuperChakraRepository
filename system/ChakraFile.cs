using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace SuperChakra.system
{
    public class ChakraFile
    {
        // DATENQUELLEN

        // FELDER

        public string ExportTextFile { get; private set; }
        public string ExportExcelFile { get; private set; }
        public string ExportCsvFile { get; private set; }

        public string BaseExportDir {  get; private set; }

        // KONSTRUKTOR

        public ChakraFile(ChakraApp file) 
        { 
            BaseExportDir = file.ExportDirectory;
            EnsureExportDirectoryExists();
        }

        // METHODEN

        // Hilfsmethoden

        private void EnsureExportDirectoryExists() 
        { 
            if (!Directory.Exists(BaseExportDir))
                Directory.CreateDirectory(BaseExportDir);
        }

        private void UpdateExportPath(string filePath) 
        {
            string extensions = Path.GetExtension(filePath).ToLower();

            switch (extensions)
            {
                case ".txt":
                    ExportTextFile = filePath;
                    break;
                case ".xls":
                case ".xlsx":
                    ExportExcelFile = filePath;
                    break;
                case ".csv":
                    ExportCsvFile = filePath;
                    break;
            }
        }

        public string RequestSavePathFromUser(string defaultNamePrefix) 
        {
            var saveDialog = new SaveFileDialog 
            { 
                Filter = "Excel Arbeitsmappe (*.xlsx)|*.xlsx|Excel CSV (*.csv)|*.csv|Textdatei (*.txt)|*.txt",
                Title = "Export Chakra Data",
                FileName = $"{defaultNamePrefix}_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };

            if (saveDialog.ShowDialog() == true) 
            { 
                UpdateExportPath(saveDialog.FileName);
                return saveDialog.FileName;
            }
            return null;
        }
        
    }
}
