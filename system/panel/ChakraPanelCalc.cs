using System;
using SuperChakra.main;

namespace SuperChakra.system.panel
{
    public class ChakraPanelCalc
    {
        // FELDER

        // Physics

        // Binary

        public string ID { get; set; }
        public string Bits { get; set; }
        public string BitsHex { get; set; }
        public string ByteSequence { get; set; }

        // STANDARD-KONSTRUKTOR

        public ChakraPanelCalc() 
        { 
            
        }

        // KONSTRUKTOR

        public ChakraPanelCalc(string id, ChakraBinary binary) 
        { 
            ID = id;

            if (id == "Sequence_01") 
            { 
                Bits = Convert.ToString(binary.Sequence_01, 2).PadLeft(8, '0');
                BitsHex = $"0x{binary.Sequence_01:X2}";
                ByteSequence = Convert.ToString(binary.Sequence_01, 2).PadLeft(8, '0');
            }

            else if (id == "Sequence_02") 
            {
                Bits = Convert.ToString(binary.Sequence_02, 2).PadLeft(8, '0');
                BitsHex = $"0x{binary.Sequence_02:X2}";
                ByteSequence = Convert.ToString(binary.Sequence_01, 2).PadLeft(8, '0');
            }

        }

        // METHODEN



    }
}

