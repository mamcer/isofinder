using System;

namespace IsoFinder.IsoScanner.Views
{
    public interface IScannerStep
    {
        void Initialize(IsoScannerInfo scannerInfo);

        void Execute();

        bool ValidateStep();

        IsoScannerInfo Info { get; }

        event Action WorkFinished;
    }
}