USE ChakraDataDB;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChakraBinaryData]') AND type in (N'U'))
BEGIN
	CREATE TABLE ChakraBinaryData 
	(
		[Index] INT PRIMARY KEY FOREIGN KEY REFERENCES ChakraMetaData([Index]),
		[ID] VARCHAR(50) NOT NULL,

		OriginalBinaryString VARCHAR(255) NOT NULL,
		Signalwert INT NOT NULL,

		OriginalBytes VARBINARY(32) NOT NULL,
		ByteSequenceHex VARCHAR(50) NOT NULL,

		isActive BIT NOT NULL DEFAULT 0,
		TTL INT NOT NULL DEFAULT 1000,

		
		[Level] FLOAT NOT NULL,
		Prioritaet FLOAT NOT NULL,
		[Weight] FLOAT NOT NULL,

		BitLenght INT NOT NULL
		
	);
END

