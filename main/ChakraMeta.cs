using System;
using System.Windows.Media;
using System.Collections.Generic;

namespace SuperChakra.main
{
    public class ChakraMeta
    {
        // DATENQUELLEN

        private static readonly string[] name = { "Wurzel", "Becken", "Nabel", "Herz", "Kehle", "Stirn", "Kopf" };

        private static readonly Color[] farbe = { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Indigo, Colors.Violet };
        private static readonly string[] colorHex = { "#FFFF0000", "#FFFFA500", "#FFFFFF00" , "#FF008000", "#FF0000FF", "#FF4B0082", "#FFEE82EE" };

        // FELDER

        public int Index { get; }

        public string Name { get; }

        public Color Farbe { get; }
        public string ColorHex {  get; }


        // KONSTRUKTOR

        public ChakraMeta(int index) 
        {
            if (index < 0 || index >= name.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            Index = index; 

            Name = name[index];
            Farbe = farbe[index];
            ColorHex = colorHex[index];
        }
    }
}

