MERGE INTO ChakraMetaData AS Target
USING (VALUES
	(0, 'Wurzel', '#FF0000'),
	(1, 'Becken', '#FF8C00'),
	(2, 'Nabel', '#FFFF00'),
	(3, 'Herz', '#008000'),
	(4, 'Kehle', '#00BFFF'),
	(5, 'Stirn', '#4B0082'),
	(6, 'Kopf', '#800080')
) AS Source ([Index], [Name], ColorHex)
ON Target.[Index] = Source.[Index]
WHEN NOT MATCHED THEN
	INSERT ([Index], Name, ColorHex)
	VALUES (Source.[Index], Source.[Name], Source.ColorHex);

