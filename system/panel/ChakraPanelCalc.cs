using System;
using SuperChakra.main;

namespace SuperChakra.system.panel
{
    public class ChakraPanelCalcPhysics 
    {
        // FELDER

        public int Index { get; set; }
        public string Name { get; set; }

        public double Frequenz { get; set; }
        public double Radius { get; set; }

        public string TangenzGeschwindigkeit { get; set; }
        public string InformationsGehalt { get; set; }
        public string ResonanzQuotient { get; set; }
        public int RawBit { get; set; }
        public int HalbWelleBit { get; set; }
        public int ViertelWelleBit { get; set; }


        // STANDARD-KONSTRUKTOR

        public ChakraPanelCalcPhysics() { }

        // KONSTRUKTOR

        public ChakraPanelCalcPhysics(ChakraPhysics source) 
        {
            this.Index = source.Index + 1;
            this.Name = source.Name;            

            double echteFrequenz = source.Frequenz;
            double echterRadius = source.Radius;
            double tangenzGeschwindigkeit = 2.0 * Math.PI * echteFrequenz * echterRadius;

            if (tangenzGeschwindigkeit == 0)
                tangenzGeschwindigkeit = 2.0 * Math.PI * source.GetFrequenceByIndex(source.Index) * source.GetRadiusByIndex(source.Index);

            this.TangenzGeschwindigkeit = $"{tangenzGeschwindigkeit:F2} m/s".Replace('.', ',');

            double lambda = source.Wellenlaenge;

            if (lambda > 0) 
            {
                double infoRaw = source.Radius / lambda;
                this.InformationsGehalt = $"{infoRaw:F5}".Replace('.', ',');
                this.RawBit = (int)Math.Floor(infoRaw);

                double umfang = 2.0 * Math.PI * source.Radius;
                double wellenUmlauf = umfang / (lambda * 2.0 * Math.PI);
                this.ResonanzQuotient = $"{wellenUmlauf:F5}".Replace('.', ',');

                this.RawBit = source.RawBit;

                double halbWelleRaw = source.Radius / (source.Wellenlaenge / 2.0);
                this.HalbWelleBit = (int)Math.Floor(halbWelleRaw);

                double viertelWelleRaw = source.Radius / (source.Wellenlaenge / 4.0);
                this.ViertelWelleBit = (int)Math.Floor(viertelWelleRaw);

            }
            else 
            {
                this.InformationsGehalt = "0,00000";                
                this.ResonanzQuotient = "0,00000";
                this.RawBit = 0;
                this.HalbWelleBit = 0;
                this.ViertelWelleBit = 0;
            }           


        }

        // METHODEN

    }

    public class ChakraPanelCalc
    {
        // FELDER

        // Binary

        public string ID { get; set; }
        public string Bits { get; set; }
        public string BitsHex { get; set; }
        public string ByteSequence { get; set; }
        public string ByteSequenceHalbWelle { get; set; }
        public string ByteSequenceViertelWelle { get; set; }


        public int Weight { get; set; }
        public string Priority { get; set; }
        public string Entropy { get; set; }
        public string Decimal { get; set; }
        public string CRC8 { get; set; }
        public int RunLength { get; set; }
        public string BitDensity { get; set; }
        public int Inversions { get; set; }
        public string NibbleXOR { get; set; }
        public string GrayCode { get; set; }
        public int Level { get; set; }
        public string CoreSequence01 { get; set; }
        public string CoreSequence02 { get; set; }
        public string SNR { get; set; }
        public string Parity { get; set; }
        public string Asymmetry { get; set; }
        public string Autocorrelation { get; set; }
        public int Distance { get; set; }
        public string Redundancy { get; set; }
        public string Resilience { get; set; }
        public string PulseWidth { get; set; }
        public string DutyCycle { get; set; }
        public string JitterFactor { get; set; }


        // STANDARD-KONSTRUKTOR

        public ChakraPanelCalc() 
        { 
            
        }

        // KONSTRUKTOR

        public ChakraPanelCalc(string id, ChakraBinary binary) 
        { 
            ID = id;

            byte val01 = binary.Sequence_01;
            byte val02 = ChakraBinary.CalcOriginalBytesComplement(val01);

            if (id == "Sequence_01") 
            {                
                Bits = Convert.ToString(val01, 2).PadLeft(8, '0');
                BitsHex = $"0x{val01:X2}";
                ByteSequence = Bits;

                Weight = CalculateWeight(val01);
                double entropyRaw = CalculateEntropyRaw(Bits);
                Entropy = entropyRaw.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
                Priority = $"{(1.0 - entropyRaw) * 100:F1} %";

                Decimal = val01.ToString();
                CRC8 = $"0x{CalculateCRC8(new byte[] { val01 }):X2}";
                RunLength = CalculateRunLength(Bits);
                BitDensity = $"{((double)Weight / Bits.Length) * 100:F1} %";

                Inversions = CalculateInversions(Bits);
                NibbleXOR = $"0x{(byte)((val01 >> 4) ^ (val01 & 0x0F)):X1}";
                GrayCode = Convert.ToString((byte)(val01 ^ (val01 >> 1)), 2).PadLeft(8, '0');
                CoreSequence01 = val01.ToString();
                CoreSequence02 = "-";
            }
            else if (id == "Sequence_02") 
            {
                Bits = Convert.ToString(val02, 2).PadLeft(8, '0');
                BitsHex = $"0x{val02:X2}";
                ByteSequence = Convert.ToString(val02, 2).PadLeft(8, '0');

                Weight = CalculateWeight(val02);
                double entropyRaw = CalculateEntropyRaw(Bits);
                Entropy = entropyRaw.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
                Priority = $"{(1.0 - entropyRaw) * 100:F1} %";

                Decimal = val02.ToString();
                CRC8 = $"0x{CalculateCRC8(new byte[] { val02 }):X2}";
                RunLength = CalculateRunLength(Bits);
                BitDensity = $"{((double)Weight / Bits.Length) * 100:F1} %";

                Inversions = CalculateInversions(Bits);
                NibbleXOR = $"0x{(byte)((val02 >> 4) ^ (val02 & 0x0F)):X1}";
                GrayCode = Convert.ToString((byte)(val02 ^ (val02 >> 1)), 2).PadLeft(8, '0');
                CoreSequence01 = "-";
                CoreSequence02 = val02.ToString();
            }
            else if (id == "CombinedSequence_01_02") 
            {
                ushort part1_original = val01;
                ushort part2_complement = val02;
                ushort combined = (ushort)((part2_complement << 8) | part1_original);
                Bits = Convert.ToString(combined, 2).PadLeft(16, '0');
                BitsHex = $"0x{combined:X4}";
                ByteSequence = Bits;

                Weight = CalculateWeight(combined);
                double entropyRaw = CalculateEntropyRaw(Bits);
                Entropy = entropyRaw.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
                Priority = $"{(1.0 - entropyRaw) * 100:F1} %";

                Decimal = combined.ToString();

                byte[] bytes = new byte[] { (byte)(combined >> 8), (byte)(combined & 0xFF) };
                CRC8 = $"0x{CalculateCRC8(bytes):X2}";
                RunLength = CalculateRunLength(Bits);
                BitDensity = $"{((double)Weight / Bits.Length) * 100:F1} %";

                Inversions = CalculateInversions(Bits);
                NibbleXOR = $"0x{(byte)((combined >> 8) ^ (combined & 0xFF)):X2}";
                GrayCode = Convert.ToString((ushort)(combined ^ (combined >> 1)), 2).PadLeft(16, '0');
                CoreSequence01 = val01.ToString();
                CoreSequence02 = val02.ToString();
            }
            else if (id == "CombinedSequence_03") 
            {
                ushort part1_original = val01;
                ushort part2_complement = val02;
                ushort combinedBase = (ushort)((part2_complement << 8) | part1_original);
                ushort combinedComp = (ushort)(~combinedBase & 0x7F7F);

                Bits = Convert.ToString(combinedComp, 2).PadLeft(16, '0');
                BitsHex = $"0x{combinedComp:X4}";
                ByteSequence = Bits;

                Weight = CalculateWeight(combinedComp);
                double entropyRaw = CalculateEntropyRaw(Bits);
                Entropy = entropyRaw.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
                Priority = $"{(1.0 - entropyRaw) * 100:F1} %";
                Decimal = combinedComp.ToString();

                byte[] bytes = new byte[] { (byte)(combinedComp >> 8), (byte)(combinedComp & 0xFF) };
                CRC8 = $"0x{CalculateCRC8(bytes):X2}";
                RunLength = CalculateRunLength(Bits);
                BitDensity = $"{((double)Weight / Bits.Length) * 100:F1} %";

                Inversions = CalculateInversions(Bits);
                NibbleXOR = $"0x{(byte)((combinedComp >> 8) ^ (combinedComp & 0xFF)):X2}";
                GrayCode = Convert.ToString((ushort)(combinedComp ^ (combinedComp >> 1)), 2).PadLeft(16, '0');
                CoreSequence01 = val01.ToString();
                CoreSequence02 = val02.ToString();
            }

            this.Level = this.Bits.Length / 8;
            this.SNR = CalculateSNR(this.Bits, this.Weight);

            this.Parity = (this.Weight % 2 == 0) ? "EVEN" : "ODD";
            this.Asymmetry = CalculateAsymmetry(this.Bits);
            this.Autocorrelation = CalculateAutocorrelation(this.Bits);

            string base8BitPattern = Convert.ToString(val01, 2).PadLeft(8, '0');
            this.Distance = CalculateDistance(this.Bits, base8BitPattern);

            int zerosCount = this.Bits.Length - this.Weight;
            this.Redundancy = $"{((double)zerosCount / this.Bits.Length) * 100:F1} %";

            double entropyRawVal = CalculateEntropyRaw(this.Bits);
            double baseResilience = entropyRawVal * 100;
            if (this.Parity == "EVEN") baseResilience += 5.0;
            this.Resilience = $"{Math.Min(baseResilience, 100.0):F0} / 100";

            this.PulseWidth = CalculatePulseWidth(this.Bits);
            this.DutyCycle = this.BitDensity;
            this.JitterFactor = CalculateJitterFactor(this.Bits);

        }

        // METHODEN

        // Weight

        private int CalculateWeight(ulong value)
        {
            int count = 0;
            while (value > 0)
            {
                count += (int)(value & 1);
                value >>= 1;
            }
            return count;
        }

        // Entropie (Informationsgehalt)

        private double CalculateEntropyRaw(string bitString)
        {
            if (string.IsNullOrEmpty(bitString)) return 0.0;

            double zeros = 0;
            double ones = 0;

            foreach (char c in bitString)
            {
                if (c == '0') zeros++;
                else if (c == '1') ones++;
            }

            double total = bitString.Length;
            double p0 = zeros / total;
            double p1 = ones / total;

            double entropy = 0;
            if (p0 > 0) entropy -= p0 * Math.Log2(p0);
            if (p1 > 0) entropy -= p1 * Math.Log2(p1);

            return entropy;
        }

        // CRC8 - Checksumme

        private byte CalculateCRC8(byte[] data)
        {
            byte crc = 0x00;
            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x80) != 0)
                        crc = (byte)((crc << 1) ^ 0x07);
                    else
                        crc = (byte)(crc << 1);
                }
            }
            return crc;
        }

        // Run Length

        private int CalculateRunLength(string bitString)
        {
            if (string.IsNullOrEmpty(bitString)) return 0;

            int maxRun = 1;
            int currentRun = 1;

            for (int i = 1; i < bitString.Length; i++)
            {
                if (bitString[i] == bitString[i - 1])
                {
                    currentRun++;
                    if (currentRun > maxRun)
                    {
                        maxRun = currentRun;
                    }
                }
                else
                {
                    currentRun = 1;
                }
            }
            return maxRun;
        }

        // Inversion

        private int CalculateInversions(string bitString)
        {
            if (string.IsNullOrEmpty(bitString)) return 0;
            int count = 0;
            for (int i = 1; i < bitString.Length; i++)
            {
                if (bitString[i] != bitString[i - 1]) count++;
            }
            return count;
        }

        // SNR (Signalrauschen)

        private string CalculateSNR(string bitString, int onesCount)
        {
            int totalBits = bitString.Length;
            int zerosCount = totalBits - onesCount;

            if (zerosCount == 0) return "+Inf dB";
            if (onesCount == 0) return "-Inf dB";

            double snrDb = 10 * Math.Log10((double)onesCount / zerosCount);
            string sign = snrDb > 0 ? "+" : "";

            if (Math.Abs(snrDb) < 0.001) return "0.0 dB";

            return $"{sign}{snrDb:F1} dB".Replace('.', ',');
        }

        // Asymmetry (Energetische Schiefe)

        private string CalculateAsymmetry(string bitString)
        {
            int len = bitString.Length;
            int mid = len / 2;
            int leftOnes = 0; int rightOnes = 0;

            for (int i = 0; i < len; i++)
            {
                if (bitString[i] == '1')
                {
                    if (i < mid) leftOnes++;
                    else rightOnes++;
                }
            }

            if (leftOnes == rightOnes) return "BALANCED";
            return leftOnes > rightOnes ? $"+{leftOnes - rightOnes} L" : $"-{rightOnes - leftOnes} R";
        }

        // Autocorrelation (Selbst-Symmetrie / Phasen-Verschiebung)

        private string CalculateAutocorrelation(string bitString)
        {
            int matches = 0;
            int len = bitString.Length;

            for (int i = 0; i < len; i++)
            {
                int nextIndex = (i + 1) % len;
                if (bitString[i] == bitString[nextIndex]) matches++;
            }

            double score = (double)matches / len;
            return $"{score * 100:F0} %";
        }

        // Distance

        private int CalculateDistance(string currentBits, string baseBits)
        {
            if (currentBits.Length == 16)
            {
                baseBits = baseBits + baseBits;
            }

            int dist = 0;
            for (int i = 0; i < currentBits.Length; i++)
            {
                if (currentBits[i] != baseBits[i]) dist++;
            }
            return dist;
        }

        // Pulsweite (Zeitliche Aktivitätsdauer)

        private string CalculatePulseWidth(string bitString)
        {
            int totalPulseLength = 0;
            int pulseCount = 0;
            int currentPulse = 0;

            foreach (char c in bitString)
            {
                if (c == '1')
                {
                    currentPulse++;
                }
                else
                {
                    if (currentPulse > 0)
                    {
                        totalPulseLength += currentPulse;
                        pulseCount++;
                        currentPulse = 0;
                    }
                }
            }
            if (currentPulse > 0)
            {
                totalPulseLength += currentPulse;
                pulseCount++;
            }

            if (pulseCount == 0) return "0,0 Bit";
            double avgWidth = (double)totalPulseLength / pulseCount;
            return $"{avgWidth:F1} Bit".Replace('.', ',');
        }

        // Jitter Faktor (Zeitliches Signal-Flackern)

        private string CalculateJitterFactor(string bitString)
        {
            List<int> intervals = new List<int>();
            int currentInterval = 0;
            bool foundFirstOne = false;

            foreach (char c in bitString)
            {
                if (c == '1')
                {
                    if (foundFirstOne)
                    {
                        intervals.Add(currentInterval);
                    }
                    foundFirstOne = true;
                    currentInterval = 0;
                }
                else
                {
                    currentInterval++;
                }
            }

            if (intervals.Count <= 1) return "0,0 %";

            double sum = 0;
            foreach (int i in intervals) sum += i;
            double avgInterval = sum / intervals.Count;

            double absoluteDeviationSum = 0; 
            foreach (int i in intervals) 
                absoluteDeviationSum += Math.Abs(i - avgInterval); 
            
            double jitter = (absoluteDeviationSum / intervals.Count); 
            double jitterPercent = (jitter / bitString.Length) * 100; 
            
            return $"{jitterPercent:F1} %".Replace('.', ',');
        }

    }
}

