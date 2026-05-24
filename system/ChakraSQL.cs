using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Printing;
using System.Windows.Documents;
using System.Reflection;
using System.Data;

namespace SuperChakra.system
{
    public class ChakraSQL
    {
        // DATENQUELLEN

        private readonly string connectionString = @"Server=.\SUPERCHAKRA;Integrated Security=true;Initial Catalog=ChakraDataDB;Encrypt=False;TrustServerCertificate=True;";
        private readonly string masterConnectionString = @"Server=.\SUPERCHAKRA;Integrated Security=true;Initial Catalog=master;Encrypt=False;TrustServerCertificate=True;";        

        // FELDER

        // Datenbank

        public string ConnectionString => connectionString;
        public string MasterConnectionString => masterConnectionString;

        public string QueryInitDB { get; private set; }
        public string QueryDropDB { get; private set; }

        // Tabellen

        public string QueryInitTablesMeta { get; private set; }
        public string QueryInitTablesPhysics { get; private set; }
        public string QueryInitTablesBinary { get; private set; }
        public string QueryInitTablesEntity { get; private set; }

        public string QueryDropTablesMeta { get; private set; }
        public string QueryDropTablesPhysics { get; private set; }
        public string QueryDropTablesBinary { get; private set; }
        public string QueryDropTablesEntity { get; private set; }

        public string QuerySeedMeta {  get; private set; }
        public string QuerySeedPhysics { get; private set; }
        public string QuerySeedBinary { get; private set; }
        public string QuerySeedEntity { get; private set; }

        // Tabellen - Operationen

        public string QueryPopLastRowMeta { get; private set; }
        public string QueryPopLastRowPhysics { get; private set; }
        public string QueryPopLastRowBinary { get; private set; }

        // Software Ausgabe

        // Standard

        public string QueryLoadMeta { get; private set; }
        public string QueryLoadPhysics { get; private set; }
        public string QueryLoadBinary { get; private set; }
        public string QueryLoadEntity { get; private set; }

        // Beziehungen

        public string QueryLoadRelations { get; private set; }


        // KONSTRUKTOR

        public ChakraSQL(ChakraApp sql) 
        {
            // Datenbank

            QueryInitDB = LoadInternalScript("DBconf.db.createChakraDataDB.sql");
            QueryDropDB = LoadInternalScript("DBconf.db.dropChakraDataDB.sql");

            // Tabellen

            QueryInitTablesMeta = LoadInternalScript("DBconf.tables.createTablesChakraMeta.sql");
            QueryInitTablesPhysics = LoadInternalScript("DBconf.tables.createTablesChakraPhysics.sql");
            QueryInitTablesBinary = LoadInternalScript("DBconf.tables.createTablesChakraBinary.sql");

            QueryDropTablesMeta = LoadInternalScript("DBconf.tables.dropChakraMetaTables.sql");
            QueryDropTablesPhysics = LoadInternalScript("DBconf.tables.dropChakraPhysicsTables.sql");
            QueryDropTablesBinary = LoadInternalScript("DBconf.tables.dropChakraBinaryTables.sql");

            QuerySeedMeta = LoadInternalScript("DBconf.seeds.seedChakraMeta.sql");
            QuerySeedPhysics = LoadInternalScript("DBconf.seeds.seedChakraPhysics.sql");
            QuerySeedBinary = LoadInternalScript("DBconf.seeds.seedChakraBinary.sql");

            // Operationen

            QueryPopLastRowMeta = LoadInternalScript("DBconf.operations.popLastRowChakraMeta.sql");
            QueryPopLastRowPhysics = LoadInternalScript("DBconf.operations.popLastRowChakraPhysics.sql");
            QueryPopLastRowBinary = LoadInternalScript("DBconf.operations.popLastRowChakraBinary.sql");

            // Ausgabe

            QueryLoadMeta = LoadInternalScript("output.loadChakraMeta.sql");
            QueryLoadPhysics = LoadInternalScript("output.loadChakraPhysics.sql");
            QueryLoadBinary = LoadInternalScript("output.loadChakraBinary.sql");
        }

        // METHODEN

        // Hilfsmethoden

        private string LoadInternalScript(string relativeResourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullManifestPath = $"SuperChakra.system.sql.{relativeResourcePath}";

            using (Stream stream = assembly.GetManifestResourceStream(fullManifestPath))
            {
                if (stream == null)
                {
                    Debug.WriteLine($"=== ❌ FEHLER: Skript nicht im RAM gefunden: {fullManifestPath} ===");
                    return string.Empty;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string sqlText = reader.ReadToEnd();
                    Debug.WriteLine($"=== ✅ EMBEDDED: {relativeResourcePath} geladen ({sqlText.Length} Zeichen) ===");
                    return sqlText;
                }
            }
        }

        public void ExecuteSQLScript(string sqlScript)
        {
            if (string.IsNullOrWhiteSpace(sqlScript)) return;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlScript, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        Debug.WriteLine("SQL-Auftrag ausgeführt.");
                    }
                    catch (Exception ex) { Debug.WriteLine("Error Script Exec: " + ex.Message); }
                }
            }
        }

        public DataTable GetDataTable(string query)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        try
                        {
                            connection.Open();
                            adapter.Fill(dataTable);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"SQL-Fehler in GetDataTable: {ex.Message}");
                            throw;
                        }
                    }
                }
            }

            return dataTable;
        }

        // Datenbank - Methoden

        // Datenbank erstellen

        public void CreateDatabaseFile()
        {
            using (SqlConnection conn = new SqlConnection(MasterConnectionString))
            {
                SqlConnection.ClearAllPools();

                string sqlToRun = string.IsNullOrWhiteSpace(QueryInitDB)
                    ? @"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ChakraDataDB') BEGIN EXEC('CREATE DATABASE ChakraDataDB'); END"
                    : QueryInitDB;

                using (SqlCommand cmd = new SqlCommand(sqlToRun, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        Debug.WriteLine("=== SQL Express/Dev: DB wurde erfolgreich angelegt ===");
                    }
                    catch (Exception ex) { Debug.WriteLine("Error Create DB: " + ex.Message); }
                }
            }
        }

        // Datenbank löschen

        public void DropDatabaseFile()
        {
            using (SqlConnection conn = new SqlConnection(MasterConnectionString))
            {
                SqlConnection.ClearAllPools();

                string forceDropSql = @"
                    IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ChakraDataDB')
                    BEGIN
                        ALTER DATABASE ChakraDataDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;                               
                        DROP DATABASE ChakraDataDB;
                    END";

                string sqlToRun = string.IsNullOrWhiteSpace(QueryDropDB) ? forceDropSql : QueryDropDB;

                using (SqlCommand cmd = new SqlCommand(sqlToRun, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        Debug.WriteLine("=== SQL: DB und physische Dateien wurden erfolgreich vernichtet ===");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error Drop DB: " + ex.Message);
                    }
                }
            }
        }

        // Daten laden

        public List<Dictionary<string, object>> GetDashBoardData(string query)
        {
            var rows = new List<Dictionary<string, object>>();
            if (string.IsNullOrWhiteSpace(query)) return rows;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string colName = reader.GetName(i);
                                    row[colName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                rows.Add(row);
                            }
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine("Error Get Data: " + ex.Message); }
                }
            }
            return rows;
        }

        // Datenbank - Operationen

        // Letzte Zeile löschen

        public void DeleteLastRow(string tableName)
        {
            string scriptToExecute = tableName?.ToLower() switch
            {
                "meta" => QueryPopLastRowMeta,
                "physics" => QueryPopLastRowPhysics,
                "binary" => QueryPopLastRowBinary,
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(scriptToExecute)) return;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(scriptToExecute, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { Debug.WriteLine("Error Pop Last Row: " + ex.Message); }
            }
        }


        // Load Vectors

        public List<ChakraSQLVectors> LoadSQLVectors() 
        { 
            var list = new List<ChakraSQLVectors>();
            string query = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                float positionX = (float)Convert.ToDouble(reader["PositionX"]);
                                float positionY = (float)Convert.ToDouble(reader["PositionY"]);

                                float rgbR = (float)Convert.ToDouble(reader["ColorRGB_R"]);
                                float rgbG = (float)Convert.ToDouble(reader["ColorRGB_G"]);
                                float rgbB = (float)Convert.ToDouble(reader["ColorRGB_B"]);

                                float rgb4R = (float)Convert.ToDouble(reader["ColorRGB_B"]);
                                float rgb4G = (float)Convert.ToDouble(reader["ColorRGB_G"]);
                                float rgb4B = (float)Convert.ToDouble(reader["ColorRGB_B"]);
                                float rgb4T = (float)Convert.ToDouble(reader["ColorRGB_T"]);

                                byte[] rawBytes = (byte[])reader["RawDataV"];

                                int rawDataCount = rawBytes.Length / sizeof(ulong);
                                ulong[] rawDataArray = new ulong[Vector<ulong>.Count];
                                Vector<ulong> rawDataVektor = new Vector<ulong>(new ReadOnlySpan<ulong>(rawDataArray));

                                for (int i = 0; i < Math.Min(rawDataCount, rawDataArray.Length); i++)
                                {
                                    rawDataArray[i] = BitConverter.ToUInt64(rawBytes, i * sizeof(ulong));
                                }

                                list.Add(new ChakraSQLVectors
                                {

                                    Index = Convert.ToInt32(reader["Index"]),

                                    Name = reader["Name"].ToString(),
                                    ColorHex = reader["ColorHex"].ToString(),

                                    Position = new Vector2(positionX, positionY),
                                    ColorRGB = new Vector3(rgbR, rgbG, rgbB),
                                    ColorRGBT = new Vector4(rgb4R, rgbG, rgb4B, rgb4T),

                                    RawDataV = new ChakraSQLVectorT(rawDataVektor)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return list;
        }
        
    }

    public class ChakraSQLVectors
    {
        // FELDER

        public int Index { get; set; }

        public string Name { get; set; }
        public string ColorHex { get; set; }

        public Vector2 Position { get; set; }
        public Vector3 ColorRGB { get; set; }
        public Vector4 ColorRGBT { get; set; }

        public ChakraSQLVectorT RawDataV { get; set; }
    }


    // STRUKTUREN

    public struct ChakraSQLVectorT 
    {
        // FELDER

        public Vector<ulong> RawDataV { get; set; }

        // KONSTRUKTOR

        public ChakraSQLVectorT(Vector<ulong> rawDataV) 
        { 
            RawDataV = rawDataV;
        }
    }

}

