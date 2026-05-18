IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ChakraDataDB')
BEGIN
	EXEC('CREATE DATABASE ChakraDataDB');
END
