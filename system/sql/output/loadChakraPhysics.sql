USE ChakraDataDB;

SELECT

    meta.[Index]         AS [Index],
    meta.[Name]          AS [Name],
    meta.[ColorHex]      AS [ColorHex],

    physics.[Frequenz]   AS [Frequenz],
    physics.[Zeit]       AS [Zeit],
    physics.[Stromstaerke] AS [Stromstaerke],
    physics.[Temperatur] AS [Temperatur], 
    physics.[Radius]     AS [Radius], 
    physics.[Leitwert]   AS [Leitwert],
    physics.[Wellenlaenge] AS [Wellenlaenge],	 
    physics.[Energie]    AS [Energie],
    physics.[Volumendichte] AS [Volumendichte],
    physics.[Schub]      AS [Schub],
    physics.[Echo]       AS [Echo],
    physics.[Volumen]    AS [Volumen],
    physics.[Dichte]     AS [Dichte],
    physics.[Rotation]   AS [Rotation],
    physics.[Speed]      AS [Speed],
    physics.[Beat]       AS [Beat],
    physics.[Drehimpuls] AS [Drehimpuls],
    physics.[Huellvolumen] AS [Huellvolumen],
    physics.[Phasenverschiebung] AS [Phasenverschiebung]

FROM ChakraMetaData AS meta
JOIN ChakraPhysicsData AS physics ON meta.[Index] = physics.[Index]
ORDER BY meta.[Index] ASC;

