MERGE INTO ChakraBinaryData AS Target
USING (VALUES

	(0, 'Sequence01', '01010011', 0, 0x53, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8),
    (1, 'Sequence02', '01110110', 0, 0x76, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8),
    (2, 'Sequence03', '00101101', 0, 0x2D, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8),
    (3, 'Sequence04', '00101100', 0, 0x2C, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8),
    (4, 'Sequence05', '00101100', 0, 0x2C, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8),
    (5, 'Sequence06', '00101100', 0, 0x2C, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8),
    (6, 'Sequence07', '01101100', 0, 0x6C, '0xD3D3D289AC', 0, 1000, 0.0, 0.0, 0.0, 8)

	

) AS Source ([Index], [ID], OriginalBinaryString, Signalwert, OriginalBytes, ByteSequenceHex, isActive, TTL, [Level], Prioritaet, [Weight], BitLenght)
ON Target.[Index] = Source.[Index]
WHEN NOT MATCHED THEN
	INSERT ([Index], [ID], OriginalBinaryString, Signalwert, OriginalBytes, ByteSequenceHex, isActive, TTL, [Level], Prioritaet, [Weight], BitLenght)
	VALUES (Source.[Index], Source.[ID], Source.OriginalBinaryString, Source.Signalwert, Source.OriginalBytes, Source.ByteSequenceHex, Source.isActive, Source.TTL, Source.[Level], Source.Prioritaet, Source.[Weight], Source.BitLenght);


