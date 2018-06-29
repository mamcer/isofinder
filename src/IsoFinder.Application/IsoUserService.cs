using System;
using System.Linq;
using IsoFinder.Core;
using IsoFinder.Data;

namespace IsoFinder.Application
{
    public class IsoUserService
    {
        private IUnitOfWork _unitOfWork;

        public IsoUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IsoUser GetByName(string name)
        {
            return _unitOfWork.IsoUserRepository.Search(m => m.Name == name).FirstOrDefault();
        }

        public IsoUser Insert(string name)
        {
            var user = _unitOfWork.IsoUserRepository.Search(m => m.Name == name).FirstOrDefault();
            if (user != null)
            {
                return user;
            }

            var isoUser = new IsoUser 
            { 
                Name = name,
                Created = DateTime.Now
            };

            user = _unitOfWork.IsoUserRepository.Insert(isoUser);
            _unitOfWork.Save();
            
            return user;
        }
    }
}