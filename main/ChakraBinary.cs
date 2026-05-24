using System;
using System.Numerics;
using System.Windows.Media;

namespace SuperChakra.main
{
    public class ChakraBinary
    {
        // DATENQUELLEN

        private static readonly double[] zeit = { 0.00253, 0.0024, 0.00189, 0.00156, 0.00135, 0.00117, 0.00104 };
        private static readonly byte sequence_01 = 0b01010011;

        // ### ORIGINAL BYTES CHAKRAS ###

        private static readonly byte[] originalBytes = { 0b01010011, 0b01110110, 0b00101101, 0b00101100, 0b00101100, 0b00101100, 0b01101100 };
        private static readonly ulong byteSequence = 0b0000000000000000000000001101001111010011110100101000100110101100;
        private static readonly ulong byteSequenceHex = 0xD3D3D289AC;

        // ### DIGITALE AUFBEREITUNG ###

        private static readonly byte[] fullBytes = { 0b01110111, 0b01111110, 0b00101101, 0b00101100, 0b00101100, 0b00101100, 0b01101100 };        
        private static readonly ulong fullByteSequence = 0b1101001111010010100000011000100000101100001011010111111001110111;        
        private static readonly ulong fullByteSequenceHex;

        // FELDER

        // Felder - Meta

        public ChakraMeta Meta { get; }
        public ChakraPhysics Physics { get; }

        public int Index => Meta.Index;
        public int DependencyIndex { get; set; } = -1;

        public string ID { get; set; }

        public double Zeit => Physics.Zeit;

        public bool isActive { get; private set; }

        public int TTL { get; private set; }
        public int MaxTTL { get; set; }

        // HILFSFELDER

        public byte Sequence_01 => sequence_01;
        public byte Sequence_02 { get; private set; }

        // ### ORIGINAL BYTES CHAKRAS ###

        public byte OriginalBytes { get; }
        public byte OriginalBytesHex { get; }

        public byte OriginalBytesComplement { get; }
        public byte OriginalBytesComplementHex { get; }

        public ulong ByteSequence { get; }
        public ulong ByteSequenceHex { get; }
        

        // ### DIGITALE AUFBEREITUNG ###
        
        public byte FullBytes { get; }        
        public ulong FullByteSequence { get; }        
        public ulong FullByteSequenceHex { get; }

        // FELDER METHODEN

        public int Signalwert => isActive ? 1 : 0;
        public double Level => (double)TTL / MaxTTL;
        public double Prioritaet => isActive ? Physics.Frequenz / 100.0 : 0;
        public double Weight => Level * Prioritaet;

        public string OriginalBinaryString => Convert.ToString(OriginalBytes, 2).PadLeft(8, '0');        


        // KONSTRUKTOR

        public ChakraBinary(int index, int maxTTl = 1000) 
        {
            Meta = new ChakraMeta(index);
            Physics = new ChakraPhysics(index);
            
            MaxTTL = maxTTl;
            TTL = MaxTTL;
            isActive = true;

            OriginalBytes = originalBytes[index];
            ByteSequence = byteSequence;
            ByteSequenceHex = byteSequenceHex;

            FullBytes = fullBytes[index];
            FullByteSequence = fullByteSequence;
        }

        // METHODEN

        // Hilfsmethoden

        public void Tick()
        {
            if (isActive && TTL > 0)
            {
                TTL--;
                if (TTL <= 0) isActive = false;
            }
        }

        // Aufbereitung

        public static byte CalcOriginalBytesComplement(byte input) 
        { 
            return (byte)(~input & 0x7F);
        }

        // Informationserweiterung

        // Erweitert

        // Regenbogen

        public static IEnumerable<Color> getRainbow() 
        {
            for (int i = 0; i < 7; i++)
            {
                yield return new ChakraMeta(i).Farbe;
            }
        }

        // Root Mix

        public static IEnumerable<Color> getRootMix()
        {
            int[] selectedIndices = { 0, 1, 4, 6 };

            foreach (int i in selectedIndices) 
            {
                yield return new ChakraMeta(i).Farbe;
            }
        }

    }    
}

