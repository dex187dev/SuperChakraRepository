DECLARE @MaxIdx INT = (SELECT MAX([Index]) FROM dbo.ChakraMetaData);

IF @MaxIdx IS NOT NULL
BEGIN
	DELETE FROM dbo.ChakraMetaData WHERE [Index] = @MaxIdx;
END
