using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Holds a list of track points which are logically connected in order.
    /// </summary>
    /// <remarks>
    /// To represent a single GPS track where GPS reception was lost, or the GPS receiver was turned off, start a new Track Segment for each continuous span of track data.
    /// </remarks>
    public class GpxTrackSegment : GpsTrackBase<GpxPoint> {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the empty GpxTrackSegment
        /// </summary>
        public GpxTrackSegment() {
        }

        /// <summary>
        /// Creates a new instance of GpxTrackSegment with given points
        /// </summary>
        /// <param name="points">The points of the track segment</param>
        public GpxTrackSegment(IEnumerable<GpxPoint> points)
            : base(points) {
        }

        #endregion

    }

}
