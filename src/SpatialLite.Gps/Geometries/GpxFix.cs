using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Defines type of the Gps fix
    /// </summary>
    public enum GpsFix {
        None,
        Fix2D,
        Fix3D,
        Dgps,
        Pps
    }
}
