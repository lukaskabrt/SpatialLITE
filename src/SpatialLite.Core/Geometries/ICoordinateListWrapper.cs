using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    public interface ICoordinateListWrapper<out T> : ICoordinateList where T : IPoint {
    }
}
