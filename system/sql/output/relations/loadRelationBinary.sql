USE ChakraDataDB;

	SELECT

	meta.[Index]         AS [Index],
    meta.[Name]          AS [Name],
    meta.[ColorHex]      AS [ColorHex],

	physics.Zeit,

	[binary].[ID],

	[binary].OriginalBinaryString,
	[binary].Signalwert,
	[binary].OriginalBytes,
	[binary].ByteSequenceHex,

	[binary].isActive,
	[binary].TTL,

	[binary].[Level],
	[binary].Prioritaet,
	[binary].[Weight]

FROM ChakraMetaData AS meta
JOIN ChakraBinaryData AS [binary] ON meta.[Index] = [binary].[Index]
JOIN ChakraPhysicsData AS physics ON meta.[Index] = physics.[Index]
ORDER BY meta.[Index] ASC;

