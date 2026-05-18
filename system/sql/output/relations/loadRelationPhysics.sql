USE ChakraDataDB;

SELECT
	
	meta.[Index],
	meta.[Name],
	meta.ColorHex,

	physics.Frequenz,
    physics.Zeit,

    physics.Stromstaerke,
	physics.Temperatur, 
    physics.Radius, 
    physics.Leitwert,

	physics.Wellenlaenge,	 
	physics.Energie,
    physics.Volumendichte,
    physics.Schub,
    physics.Echo,
    physics.Volumen,
    physics.Dichte,
    physics.Rotation,
    physics.Speed,
    physics.Beat

FROM ChakraMetaData AS meta
JOIN ChakraPhysicsData AS physics ON meta.[Index] = physics.[Index]
ORDER BY meta.[Index] ASC;

