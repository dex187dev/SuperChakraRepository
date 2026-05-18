USE ChakraDataDB;

SELECT

	[Index]    AS [Index],
    [Name]     AS [Name],
    [ColorHex] AS [ColorHex]

FROM ChakraMetaData
ORDER BY [Index] ASC;

