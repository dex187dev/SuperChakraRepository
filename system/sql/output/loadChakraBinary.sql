USE ChakraDataDB;

SELECT
    meta.[Index]                  AS [Index],
    meta.[Name]                   AS [Name],
    meta.[ColorHex]               AS [ColorHex],
    physics.[Zeit]                AS [Zeit],

    [binary].[ID]                 AS [ID],
    [binary].[OriginalBinaryString] AS [OriginalBinaryString],
    [binary].[Signalwert]         AS [Signalwert],
    
    CONVERT(VARCHAR(100), [binary].[OriginalBytes], 1) AS [OriginalBytesHex],

    [binary].[ByteSequenceHex]    AS [ByteSequenceHex],
    [binary].[isActive]           AS [isActive],
    [binary].[TTL]                AS [TTL],

    [binary].[OriginalBinaryString] AS [ByteSequence],
    [binary].[OriginalBinaryString] AS [FullBytes],
    [binary].[OriginalBinaryString] AS [FullByteSequence],
    [binary].[ByteSequenceHex]    AS [FullByteSequenceHex],

    [binary].[Level]              AS [Level],
    [binary].[Prioritaet]         AS [Priority],
    [binary].[Weight]             AS [Weight]

FROM ChakraMetaData AS meta
JOIN ChakraBinaryData AS [binary] ON meta.[Index] = [binary].[Index]
JOIN ChakraPhysicsData AS physics ON meta.[Index] = physics.[Index]
ORDER BY meta.[Index] ASC;

