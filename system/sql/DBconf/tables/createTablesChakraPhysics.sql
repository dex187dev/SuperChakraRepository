USE ChakraDataDB;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChakraPhysicsData]') AND type in (N'U'))
BEGIN
	CREATE TABLE ChakraPhysicsData 
	(
		[Index] INT PRIMARY KEY FOREIGN KEY REFERENCES ChakraMetaData([Index]),
		
		Frequenz FLOAT NOT NULL,
		Zeit FLOAT NOT NULL,

		Temperatur FLOAT NOT NULL,
		Radius FLOAT NOT NULL,
		Stromstaerke FLOAT NOT NULL,
		Leitwert FLOAT NOT NULL,

		Wellenlaenge FLOAT NOT NULL,
		Energie FLOAT NOT NULL,
		Volumendichte FLOAT NOT NULL,
		Schub FLOAT NOT NULL,
		Echo FLOAT NOT NULL,
		Volumen FLOAT NOT NULL,
		Dichte FLOAT NOT NULL,
		Rotation FLOAT NOT NULL,
		Speed FLOAT NOT NULL,
		Beat FLOAT NOT NULL,
		Drehimpuls FLOAT NOT NULL,
		Huellvolumen FLOAT NOT NULL,
		Phasenverschiebung FLOAT NOT NULL

	);
END

