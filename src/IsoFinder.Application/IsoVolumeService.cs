using System.Collections.Generic;
using System.Linq;
using IsoFinder.Core;
using IsoFinder.Data;

namespace IsoFinder.Application
{
    public class IsoVolumeService : IIsoVolumeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IsoVolumeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<IsoVolume> GetAll()
        {
            return _unitOfWork.IsoVolumeRepository.GetAll();
        }

        public IsoVolume GetById(int id)
        {
            return _unitOfWork.IsoVolumeRepository.Search(m => m.Id == id, null, "RootFolder").FirstOrDefault();
        }
    }
}