using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialLite.Core.API {
    public interface ICoordinateFilter {
        void Filter(ref Coordinate coordinate);
    }
}
