DECLARE @MaxIdx INT = (SELECT MAX([Index]) FROM dbo.ChakraMetaData);

IF @MaxIdx IS NOT NULL
BEGIN
	DELETE FROM dbo.ChakraPhysicsData WHERE [Index] = @MaxIdx;
END
