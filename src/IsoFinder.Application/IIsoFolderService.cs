using System.Collections.Generic;
using IsoFinder.Core;

namespace IsoFinder.Application
{
    public interface IIsoFolderService
    {
        IEnumerable<IsoFolder> GetAll();
        
        IsoFolder GetById(int id);
    }
}