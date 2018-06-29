using System.Collections.Generic;
using IsoFinder.Core;

namespace IsoFinder.Application
{
    public interface IIsoVolumeService
    {
        IEnumerable<IsoVolume> GetAll();

        IsoVolume GetById(int id);
    }
}