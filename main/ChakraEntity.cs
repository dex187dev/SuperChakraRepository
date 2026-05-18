using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace SuperChakra.main
{
    public class ChakraEntity
    {
        // DATENQUELLEN
        
        private static readonly ulong chakraSequence64BitHex = 0x006C2C2C2C2D7653;
        private static readonly string[] name = { "Wurzel", "Becken", "Nabel", "Herz", "Kehle", "Stirn", "Kopf" };


        // FELDER

        // Felder - Entity        

        public ulong ChakraSequence64BitHex => chakraSequence64BitHex;

        public int Index { get; }

        public bool isActive { get; }

        public int TTL { get; private set; }
        public int MaxTTL { get; set; }

        public double Weight { get; }

        // Felder - Meta

        public string Name { get; set; }


        // Felder - Berechnungen

        public double Level 
        {
            get 
            { 
                if (MaxTTL <= 0)
                    return 0;
                return (double)TTL / MaxTTL;
            }
        }

        public double Prioritaet
        {
            get
            {
                if (isActive)
                    return 1.0 * Weight;

                return 0.0;
            }
        }

        // KONSTRUKTOR

        public ChakraEntity(int index, int maxTTL = 5000) 
        {             
            MaxTTL = maxTTL;

            if (isActive)
                TTL = MaxTTL;

            if (index < 0 || index > 7)
                throw new ArgumentOutOfRangeException(nameof(index), "Index not 0 - 7");
            Index = index;

            Name = name[index];
        }

        // Felder - Brechnungen

        public byte ChakraRawValueCalc => (byte)(ChakraSequence64BitHex >> Index * 8 & 0xFF);


        // Felder - Methoden

        public double ChakraRawValue
        {
            get
            {
                return (byte)(ChakraSequence64BitHex >> Index * 8 & 0xFF);
            }
        }

        public double ChakraContribution 
        { 
            get 
            {
                int chakraBits = BitOperations.PopCount(ChakraRawValueCalc);
                int totalBits = BitOperations.PopCount(ChakraSequence64BitHex);
                if (totalBits == 0)
                    return 0;
                return (double)chakraBits / totalBits;
            }
        }

        public List<bool> BitStates 
        {
            get 
            { 
                return Enumerable.Range(0, 8).Select(bit => (ChakraRawValueCalc & 1 << bit) != 0).ToList();
            }
        }


        // METHODEN

        // METHODEN - HILFE

        public void Tick()
        {
            if (TTL > 0)
                TTL--;
        }

        // METHODEN - BERECHNUNG

        public byte ChakraResonanz() 
        { 
            byte systemKey = (byte)(ChakraSequence64BitHex & 0xFF);
            return (byte)(ChakraRawValueCalc & systemKey);
        }

        public double ChakraRelevanz() 
        {
            int individualBits = BitOperations.PopCount(ChakraRawValueCalc);
            int totalSystemBits = BitOperations.PopCount(ChakraSequence64BitHex);

            if (totalSystemBits == 0)
                return 0;

            double relevance = (double)individualBits / totalSystemBits;
            return relevance;
        }
    }
}

