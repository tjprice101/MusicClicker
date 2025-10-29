using Avalonia.Controls;
using System;

namespace MusicClicker
{
    public class UpgradeManager
    {
        private readonly Func<double> _getNotes;
        private readonly Action<double> _setNotes;
        private readonly Action<double> _addNps;
        private readonly Action<double> _addClick;

        public event Action? OnUpgradeChanged;

        // Ownership
        public int ChordOwned = 0;
        public int ScaleOwned = 0;
        public int OrchestraOwned = 0;
        public int SymphonyOwned = 0;
        public int AriaOwned = 0;
        public int RequiemOwned = 0;
        public int OpusOwned = 0;
        public int MagnumOpusOwned = 0;

        // Base costs
        public double ChordBaseCost = 10;
        public double ScaleBaseCost = 50;
        public double OrchestraBaseCost = 100;
        public double SymphonyBaseCost = 350;
        public double AriaBaseCost = 350;
        public double RequiemBaseCost = 750;
        public double OpusBaseCost = 1500;
        public double MagnumOpusBaseCost = 3000;

        // Optional: per-upgrade increases
        public double ChordNpsIncrease = 0.5;
        public double ChordClickIncrease = 0;

        public double ScaleNpsIncrease = 3;
        public double ScaleClickIncrease = 0;

        public double OrchestraNpsIncrease = 5;
        public double OrchestraClickIncrease = 0;

        public double SymphonyNpsIncrease = 9;
        public double SymphonyClickIncrease = 0;

        public double AriaNpsIncrease = 0;
        public double AriaClickIncrease = 1;

        public double RequiemNpsIncrease = 0;
        public double RequiemClickIncrease = 2;

        public double OpusNpsIncrease = 0;
        public double OpusClickIncrease = 3;

        public double MagnumOpusNpsIncrease = 0;
        public double MagnumOpusClickIncrease = 4;

        // Constructor using delegates/callbacks
        public UpgradeManager(
            Func<double> getNotes,
            Action<double> setNotes,
            Action<double> addNps,
            Action<double> addClick)
        {
            _getNotes = getNotes;
            _setNotes = setNotes;
            _addNps = addNps;
            _addClick = addClick;
        }

        public void RegisterButtons(MainWindow window)
        {
            window.BuyChordButton.Click += (s, e) => BuyUpgrade("Chord");
            window.BuyChordMaxButton.Click += (s, e) => BuyUpgradeMax("Chord");
            window.BuyScaleButton.Click += (s, e) => BuyUpgrade("Scale");
            window.BuyScaleMaxButton.Click += (s, e) => BuyUpgradeMax("Scale");
            window.BuyOrchestraButton.Click += (s, e) => BuyUpgrade("Orchestra");
            window.BuyOrchestraMaxButton.Click += (s, e) => BuyUpgradeMax("Orchestra");
            window.BuySymphonyButton.Click += (s, e) => BuyUpgrade("Symphony");
            window.BuySymphonyMaxButton.Click += (s, e) => BuyUpgradeMax("Symphony");
            window.BuyAriaButton.Click += (s, e) => BuyUpgrade("Aria");
            window.BuyAriaMaxButton.Click += (s, e) => BuyUpgradeMax("Aria");
            window.BuyRequiemButton.Click += (s, e) => BuyUpgrade("Requiem");
            window.BuyRequiemMaxButton.Click += (s, e) => BuyUpgradeMax("Requiem");
            window.BuyOpusButton.Click += (s, e) => BuyUpgrade("Opus");
            window.BuyOpusMaxButton.Click += (s, e) => BuyUpgradeMax("Opus");
            window.BuyMagnumOpusButton.Click += (s, e) => BuyUpgrade("MagnumOpus");
            window.BuyMagnumOpusMaxButton.Click += (s, e) => BuyUpgradeMax("MagnumOpus");
        }

        public void BuyUpgrade(string name) => PerformBuy(name, 1);

        public void BuyUpgradeMax(string name) => PerformBuy(name, int.MaxValue);

        private void PerformBuy(string name, int amount)
        {
            int owned = name switch
            {
                "Chord" => ChordOwned,
                "Scale" => ScaleOwned,
                "Orchestra" => OrchestraOwned,
                "Symphony" => SymphonyOwned,
                "Aria" => AriaOwned,
                "Requiem" => RequiemOwned,
                "Opus" => OpusOwned,
                "MagnumOpus" => MagnumOpusOwned,
                _ => throw new Exception("Invalid upgrade")
            };

            double baseCost = name switch
            {
                "Chord" => ChordBaseCost,
                "Scale" => ScaleBaseCost,
                "Orchestra" => OrchestraBaseCost,
                "Symphony" => SymphonyBaseCost,
                "Aria" => AriaBaseCost,
                "Requiem" => RequiemBaseCost,
                "Opus" => OpusBaseCost,
                "MagnumOpus" => MagnumOpusBaseCost,
                _ => 0
            };

            double npsIncrease = name switch
            {
                "Chord" => ChordNpsIncrease,
                "Scale" => ScaleNpsIncrease,
                "Orchestra" => OrchestraNpsIncrease,
                "Symphony" => SymphonyNpsIncrease,
                "Aria" => AriaNpsIncrease,
                "Requiem" => RequiemNpsIncrease,
                "Opus" => OpusNpsIncrease,
                "MagnumOpus" => MagnumOpusNpsIncrease,
                _ => 0
            };

            double clickIncrease = name switch
            {
                "Chord" => ChordClickIncrease,
                "Scale" => ScaleClickIncrease,
                "Orchestra" => OrchestraClickIncrease,
                "Symphony" => SymphonyClickIncrease,
                "Aria" => AriaClickIncrease,
                "Requiem" => RequiemClickIncrease,
                "Opus" => OpusClickIncrease,
                "MagnumOpus" => MagnumOpusClickIncrease,
                _ => 0
            };

            for (int i = 0; i < amount; i++)
            {
                double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                if (_getNotes() >= cost)
                {
                    _setNotes(_getNotes() - cost);
                    owned++;
                    _addNps(npsIncrease);
                    _addClick(clickIncrease);
                }
                else break;
            }

            // Save ownership back
            switch (name)
            {
                case "Chord": ChordOwned = owned; break;
                case "Scale": ScaleOwned = owned; break;
                case "Orchestra": OrchestraOwned = owned; break;
                case "Symphony": SymphonyOwned = owned; break;
                case "Aria": AriaOwned = owned; break;
                case "Requiem": RequiemOwned = owned; break;
                case "Opus": OpusOwned = owned; break;
                case "MagnumOpus": MagnumOpusOwned = owned; break;
            }

            OnUpgradeChanged?.Invoke();
        }
    }
}
