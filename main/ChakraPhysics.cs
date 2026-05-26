using System;
using System.Windows.Media;

namespace SuperChakra.main
{
    public class ChakraPhysics
    {

        // DATENQUELLEN

        private static readonly double[] frequenz = { 396.0, 417.0, 528.0, 639.0, 741.0, 852.0, 963.0 };
        private static readonly double[] zeit = { 0.00253, 0.0024, 0.00189, 0.00156, 0.00135, 0.00117, 0.00104 };

        private static readonly double[] stromstaerke = { 0.1741691505196227, 0.1834053078022192, 0.2322247602027425, 0.2810437435911907, 0.3259039329440554, 0.3747217402342070, 0.4235387907556417 };
        private static readonly double[] temperatur = { 37.0, 37.19, 38.16, 39.14, 40.04, 41.02, 42.0 };
        private static readonly double[] radius = { 1.00, 1.03, 1.17, 1.32, 1.46, 1.6, 1.75 };
        private readonly double leitwert = 0.85;

        private static readonly double[] wellenlaenge = { 0.9981235277677965, 1.0291766586730617, 1.1724386724386723, 1.3241844227759720, 1.4594891787874242, 1.6050720276072389, 1.7473440370636634 };
        private static readonly double[] energie = { 2.6239237793999998E-31, 2.7630712525499997E-31, 3.4985650392000001E-31, 4.2340588258499995E-31, 4.9099179811500002E-31, 5.6454117677999997E-31, 6.3809055445000006E-31 };
        private static readonly double[] volumendichte = { 0.0000015281023128, 0.0000012584101137, 0.0000005324482620, 0.0000002526031685, 0.0000001398042455, 0.0000000797853521, 0.0000000481796813 };
        private static readonly double[] schub = { 654406.4435917433, 794653.4989766699, 1878116.7510032036, 3958778.5296359900, 7152858.5968521786, 12533628.9566842410, 20755637.4850009828 };
        private static readonly double[] echo = { 655636.7277056958, 795289.2217758512, 1874210.2681611170, 3946268.7894823406, 7155362.0973610775, 12494022.6891811192, 20787186.0539781823 };
        private static readonly double[] volumen = { 1655.648302287110, 1907.168397544007, 3549.640659396055, 6175.694506232144, 9656.359105750441, 14664.345879320562, 21585.862984401021 };
        private static readonly double[] dichte = { 0.0873015873, 0.0919312169, 0.1164021164, 0.1408730158, 0.1633597883, 0.1878306878, 0.2123015873 };
        private static readonly double[] rotation = { 2483.4724534306, 2696.5336943312, 3889.5909044445, 5316.5414137673, 6795.1485544312, 8592.3901636643, 10572.6675841964 };
        private static readonly double[] speed = { 395.2569169960474, 429.1666666666667, 619.0476190476190, 846.1538461538461, 1081.481481481481, 1367.521367521367, 1682.692307692307 };
        private static readonly double[] beat = { 21.0, 111.0, 111.0, 102.0, 111.0, 111.0, 111.0 };

        private static readonly double[] drehimpuls = { 358962.9084402269, 501570.92267301853, 2199991.0621697474, 8059176.742952899, 22848791.63971744, 60587523.183959946, 148382721.5572319 };
        private static readonly double[] huellvolumen = { 3.1356975422104374, 3.4301589883885035, 5.0421032093693965, 7.24846773, 9.773642819585467, 12.908755175458243, 16.811419769782727 };
        private static readonly double[] phasenverschiebung = { 0, 0.00678584, 0.018095573684676403, 0.00678584, 0.022053980428201392, 0.022053980428201392, 0.029405307237599487 };


        // FELDER

        // Felder Meta

        public ChakraMeta Meta { get; }

        public int Index => Meta.Index;

        public string Name => Meta.Name;
        public Color Farbe => Meta.Farbe;

        // Felder
        
        public double Frequenz { get; set; }
        public double Zeit { get; set; }

        public double Stromstaerke { get; set; }
        public double Temperatur { get; set; }
        public double Radius { get; set; }
        public double Leitwert { get; set; }

        public double Wellenlaenge { get; set; }
        public double Energie { get; set; }
        public double Volumendichte { get; set; }
        public double Schub { get; set; }
        public double Echo { get; set; }
        public double Volumen { get; set; }
        public double Dichte { get; set; }
        public double Rotation { get; set; }
        public double Speed { get; set; }
        public double Beat { get; set; }

        public double Drehimpuls { get; set; }
        public double Huellvolumen { get; set; }
        public double Phasenverschiebung { get; set; }

        public string InformationsGehalt { get; set; }

        public int RawBit => (int)Math.Floor(Radius / Wellenlaenge);
        public int HalbWelleBit => (int)Math.Floor(Radius / (Wellenlaenge / 2.0));
        public int ViertelWelleBit => (int)Math.Floor(Radius / (Wellenlaenge / 4.0));

        // KONSTRUKTOR

        public ChakraPhysics(int index) 
        {
            Meta = new ChakraMeta(index);

            Frequenz = frequenz[index];
            Zeit = zeit[index];

            Stromstaerke = stromstaerke[index];
            Temperatur = temperatur[index];
            Radius = radius[index];

            Wellenlaenge = wellenlaenge[index];
            Energie = energie[index];
            Volumendichte = volumendichte[index];
            Schub = schub[index];
            Echo = echo[index];
            Volumen = volumen[index];
            Dichte = dichte[index];
            Rotation = rotation[index];
            Speed = speed[index];
            Beat = beat[index];

            Drehimpuls = drehimpuls[index];
            Huellvolumen = huellvolumen[index];
            Phasenverschiebung = phasenverschiebung[index];
        }

        // METHODEN

        // Hilfsmethoden

        public double PhasenlageCalc() 
        {
            return 2 * Math.PI * Frequenz * Zeit;
        }

        public double GetFrequenceByIndex(int index) 
        {
            return frequenz[Math.Clamp(index, 0, 6)];
        }

        public double GetRadiusByIndex(int index)
        {
            return frequenz[Math.Clamp(index, 0, 6)];
        }

        // Methoden

        public double DrehimpulsCalc() 
        {
            double masse = Volumen * Dichte;
            return masse * Math.Pow(Radius, 2) * Rotation;
        }

        public double HuellvolumenCalc() 
        {
            return Math.PI * Math.Pow(Radius, 2) * Wellenlaenge;
        }

        public double PhasenverschiebungCalc(ChakraPhysics other) 
        {
            return Math.Abs(PhasenlageCalc() - other.PhasenlageCalc());
        }
    }
}

