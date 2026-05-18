USE ChakraDataDB;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChakraMetaData]') AND type in (N'U'))
BEGIN
	CREATE TABLE ChakraMetaData 
	(
		[Index] INT PRIMARY KEY,
		[Name] VARCHAR(50) NOT NULL,
		ColorHex VARCHAR(7) NOT NULL
	);
END

