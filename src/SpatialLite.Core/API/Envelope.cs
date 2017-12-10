using System.Collections.Generic;

namespace SpatialLite.Core.API {
    /// <summary>
    /// Represents minimal bounding box of a <see cref="Geometry"/> object. 
    /// </summary>
    public class Envelope {
        #region Public Static Fields

        /// <summary>
        /// Empty Envelope, that has all it's bounds set to double.NaN
        /// </summary>
        public static Envelope Empty = new Envelope();

        #endregion

        #region Private Fields

        private const int XIndex = 0;
        private const int YIndex = 1;
        private const int ZIndex = 2;
        private const int MIndex = 3;

        private double[][] _bounds = new double[][] {
            new double[] { double.NaN, double.NaN },
            new double[] { double.NaN, double.NaN },
            new double[] { double.NaN, double.NaN },
            new double[] { double.NaN, double.NaN } };


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <c>Envelope</c> class that is empty and has all it's values initialized to <c>double.NaN</c>.
        /// </summary>
        public Envelope() {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Envelope</c> class with the single coordinate.
        /// </summary>
        /// <param name="coord">The coordinate used initialize <c>Envelope</c></param>
        public Envelope(Coordinate coord) {
            this.Initialize(coord.X, coord.X, coord.Y, coord.Y, coord.Z, coord.Z, coord.M, coord.M);
        }

        /// <summary>
        /// Initializes a new instance of the <c>Envelope</c> class that covers specified coordinates.
        /// </summary>
        /// <param name="coords">The coordinates to be covered.</param>
        public Envelope(IEnumerable<Coordinate> coords) {
            this.Extend(coords);
        }

        /// <summary>
        /// Initializes a new instance of the <c>Envelope</c> class as copy of specified <c>Envelope</c>.
        /// </summary>
        /// <param name="source">The <c>Envelope</c> object whose values are to be copied.</param>
        public Envelope(Envelope source) {
            this.Initialize(source.MinX, source.MaxX, source.MinY, source.MaxY, source.MinZ, source.MaxZ, source.MinM, source.MaxM);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Envelope's minimal x-coordinate.
        /// </summary>
        public double MinX {
            get { return _bounds[XIndex][0]; }
        }

        /// <summary>
        /// Gets Envelope's maximal x-coordinate.
        /// </summary>
        public double MaxX {
            get { return _bounds[XIndex][1]; }
        }

        /// <summary>
        /// Gets Envelope's minimal y-coordinate.
        /// </summary>
        public double MinY {
            get { return _bounds[YIndex][0]; }
        }

        /// <summary>
        /// Gets Envelope's maximal y-coordinate.
        /// </summary>
        public double MaxY {
            get { return _bounds[YIndex][1]; }
        }

        /// <summary>
        /// Gets Envelope's minimal z-coordinate.
        /// </summary>
        public double MinZ {
            get { return _bounds[ZIndex][0]; }
        }

        /// <summary>
        /// Gets Envelope's maximal z-coordinate.
        /// </summary>
        public double MaxZ {
            get { return _bounds[ZIndex][1]; }
        }

        /// <summary>
        /// Gets Envelope's minimal m-coordinate.
        /// </summary>
        public double MinM {
            get { return _bounds[MIndex][0]; }
        }

        /// <summary>
        /// Gets Envelope's maximal m-coordinate.
        /// </summary>
        public double MaxM {
            get { return _bounds[MIndex][1]; }
        }

        /// <summary> 
        /// Returns the difference between the maximum and minimum x values. 
        /// </summary> 
        /// <returns>max x - min x, or 0 if this is a null <c>Envelope</c>.</returns> 
        public double Width {
            get {
                if (this.IsEmpty) {
                    return 0;
                }

                return _bounds[XIndex][1] - _bounds[XIndex][0];
            }
        }


        /// <summary> 
        /// Returns the difference between the maximum and minimum y values. 
        /// </summary> 
        /// <returns>max y - min y, or 0 if this is a null <c>Envelope</c>.</returns> 
        public double Height {
            get {
                if (this.IsEmpty) {
                    return 0;
                }

                return _bounds[YIndex][1] - _bounds[YIndex][0];
            }
        }

        /// <summary>
        /// Checks if this Envelope equals the empty Envelope.
        /// </summary>
        public bool IsEmpty {
            get {
                return this.Equals(Envelope.Empty);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extends this <c>Envelope</c> to cover specified <c>Coordinate</c>.
        /// </summary>
        /// <param name="coord">The <c>Coordinate</c> to be covered by extended Envelope.</param>
        public void Extend(Coordinate coord) {
            if (double.IsNaN(_bounds[XIndex][0]) || double.IsNaN(_bounds[YIndex][0])) {
                _bounds[XIndex][0] = _bounds[XIndex][1] = coord.X;
                _bounds[YIndex][0] = _bounds[YIndex][1] = coord.Y;
                _bounds[ZIndex][0] = _bounds[ZIndex][1] = coord.Z;
                _bounds[MIndex][0] = _bounds[MIndex][1] = coord.M;
            } else {
                if (coord.X < _bounds[XIndex][0]) { _bounds[XIndex][0] = coord.X; }
                if (coord.X > _bounds[XIndex][1]) { _bounds[XIndex][1] = coord.X; }

                if (coord.Y < _bounds[YIndex][0]) { _bounds[YIndex][0] = coord.Y; }
                if (coord.Y > _bounds[YIndex][1]) { _bounds[YIndex][1] = coord.Y; }

                if (coord.Z < _bounds[ZIndex][0]) { _bounds[ZIndex][0] = coord.Z; }
                if (coord.Z > _bounds[ZIndex][1]) { _bounds[ZIndex][1] = coord.Z; }

                if (coord.M < _bounds[MIndex][0]) { _bounds[MIndex][0] = coord.M; }
                if (coord.M > _bounds[MIndex][1]) { _bounds[MIndex][1] = coord.M; }
            }
        }

        /// <summary>
        /// Extends this <c>Envelope</c> to cover specified <c>Coordinates</c>.
        /// </summary>
        /// <param name="coords">The collection of Coordinates to be covered by extended Envelope.</param>
        public void Extend(IEnumerable<Coordinate> coords) {
            foreach (var coord in coords) {
                this.Extend(coord);
            }
        }

        /// <summary>
        /// Extends this <c>Envelope</c> to cover specified <c>Envelope</c>.
        /// </summary>
        /// <param name="envelope">The <c>Envelope</c> to be covered by extended Envelope.</param>
        public void Extend(Envelope envelope) {
            if (double.IsNaN(_bounds[XIndex][0]) || double.IsNaN(_bounds[YIndex][0])) {
                _bounds[XIndex][0] = envelope.MinX;
                _bounds[XIndex][1] = envelope.MaxX;

                _bounds[YIndex][0] = envelope.MinY;
                _bounds[YIndex][1] = envelope.MaxY;

                _bounds[ZIndex][0] = envelope.MinZ;
                _bounds[ZIndex][1] = envelope.MaxZ;

                _bounds[MIndex][0] = envelope.MinM;
                _bounds[MIndex][1] = envelope.MaxM;
            } else {
                if (envelope.MinX < _bounds[XIndex][0]) { _bounds[XIndex][0] = envelope.MinX; }
                if (envelope.MaxX > _bounds[XIndex][1]) { _bounds[XIndex][1] = envelope.MaxX; }

                if (envelope.MinY < _bounds[YIndex][0]) { _bounds[YIndex][0] = envelope.MinY; }
                if (envelope.MaxY > _bounds[YIndex][1]) { _bounds[YIndex][1] = envelope.MaxY; }

                if (envelope.MinZ < _bounds[ZIndex][0]) { _bounds[ZIndex][0] = envelope.MinZ; }
                if (envelope.MaxZ > _bounds[ZIndex][1]) { _bounds[ZIndex][1] = envelope.MaxZ; }

                if (envelope.MinM < _bounds[MIndex][0]) { _bounds[MIndex][0] = envelope.MinM; }
                if (envelope.MaxM > _bounds[MIndex][1]) { _bounds[MIndex][1] = envelope.MaxM; }
            }
        }

        /// <summary>
        /// Determines whether specific <c>object</c> instance is equal to the current <c>Envelope</c>.
        /// </summary>
        /// <param name="obj">The <c>object</c> to compare with the current <c>Envelope</c></param>
        /// <returns>true if the specified  <c>object</c> is equal to the current <c>Envelope</c>; otherwise, false.</returns>
        public override bool Equals(object obj) {
            Envelope other = obj as Envelope;
            if (other == null) {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Determines whether two <c>Envelope</c> instances are equal.
        /// </summary>
        /// <param name="other">The <c>Envelope</c> to compare with the current <c>Envelope</c></param>
        /// <returns>true if the specified  <c>Envelope</c> is equal to the current <c>Envelope</c>; otherwise, false.</returns>
        public bool Equals(Envelope other) {
            return ((this.MinX == other.MinX) || (double.IsNaN(this.MinX) && double.IsNaN(other.MinX))) &&
                ((this.MinY == other.MinY) || (double.IsNaN(this.MinY) && double.IsNaN(other.MinY))) &&
                ((this.MinZ == other.MinZ) || (double.IsNaN(this.MinZ) && double.IsNaN(other.MinZ))) &&
                ((this.MinM == other.MinM) || (double.IsNaN(this.MinM) && double.IsNaN(other.MinM))) &&
                ((this.MaxX == other.MaxX) || (double.IsNaN(this.MaxX) && double.IsNaN(other.MaxX))) &&
                ((this.MaxY == other.MaxY) || (double.IsNaN(this.MaxY) && double.IsNaN(other.MaxY))) &&
                ((this.MaxZ == other.MaxZ) || (double.IsNaN(this.MaxZ) && double.IsNaN(other.MaxZ))) &&
                ((this.MaxM == other.MaxM) || (double.IsNaN(this.MaxM) && double.IsNaN(other.MaxM)));
        }

        /// <summary> 
        /// Check if the region defined by <c>other</c> 
        /// overlaps (intersects) the region of this <c>Envelope</c>. 
        /// </summary> 
        /// <param name="other"> the <c>Envelope</c> which this <c>Envelope</c> is 
        /// being checked for overlapping. 
        /// </param> 
        /// <returns> 
        /// <c>true</c> if the <c>Envelope</c>s overlap. 
        /// </returns> 
        public bool Intersects(Envelope other) {
            if (this.IsEmpty || other.IsEmpty) {
                return false;
            }

            return !(other.MinX > this.MaxX || other.MaxX < this.MinX || other.MinY > this.MaxY || other.MaxY < other.MinY);
        }

        ///<summary> 
        /// Tests if the given point lies in or on the envelope. 
        ///</summary> 
        /// <param name="x">the x-coordinate of the point which this <c>Envelope</c> is being checked for containing</param> 
        /// <param name="y">the y-coordinate of the point which this <c>Envelope</c> is being checked for containing</param> 
        /// <returns> <c>true</c> if <c>(x, y)</c> lies in the interior or on the boundary of this <c>Envelope</c>.</returns> 
        public bool Covers(double x, double y) {
            if (this.IsEmpty) {
                return false;
            }

            return x >= this.MinX &&
                x <= this.MaxX &&
                y >= this.MinY &&
                y <= this.MaxY;
        }


        ///<summary> 
        /// Tests if the given point lies in or on the envelope. 
        ///</summary> 
        /// <param name="p">the point which this <c>Envelope</c> is being checked for containing</param> 
        /// <returns><c>true</c> if the point lies in the interior or on the boundary of this <c>Envelope</c>.</returns> 
        public bool Covers(Coordinate p) {
            return Covers(p.X, p.Y);
        }


        ///<summary> 
        /// Tests if the <c>Envelope other</c> lies wholely inside this <c>Envelope</c> (inclusive of the boundary). 
        ///</summary> 
        /// <param name="other">the <c>Envelope</c> to check</param> 
        /// <returns>true if this <c>Envelope</c> covers the <c>other</c></returns> 
        public bool Covers(Envelope other) {
            if (this.IsEmpty || other.IsEmpty) {
                return false;
            }

            return other.MinX >= this.MinX &&
                other.MaxX <= this.MaxX &&
                other.MinY >= this.MinY &&
                other.MaxY <= this.MaxY;
        }


        /// <summary>
        /// Serves as a hash function for the <c>Envelope</c> class.
        /// </summary>
        /// <returns>Hash code for current Envelope object.</returns>
        public override int GetHashCode() {
            return _bounds.GetHashCode();
        }

        /// <summary>
        /// Initializes MinX, MaxX, MinY, MaxY, MinZ and MaxZ properties from the given coordinates.
        /// </summary>
        /// <param name="x1">First x-coordinate.</param>
        /// <param name="x2">Second x-coordinate.</param>
        /// <param name="y1">First y-coordinate.</param>
        /// <param name="y2">Second y-coordinate.</param>
        public void Initialize(double x1, double x2, double y1, double y2) {
            var sortedX = this.SortCoordinates(x1, x2);
            _bounds[XIndex][0] = sortedX[0];
            _bounds[XIndex][1] = sortedX[1];

            var sortedY = this.SortCoordinates(y1, y2);
            _bounds[YIndex][0] = sortedY[0];
            _bounds[YIndex][1] = sortedY[1];
        }

        /// <summary>
        /// Initializes MinX, MaxX, MinY, MaxY, MinZ and MaxZ properties from the given coordinates.
        /// </summary>
        /// <param name="x1">First x-coordinate.</param>
        /// <param name="x2">Second x-coordinate.</param>
        /// <param name="y1">First y-coordinate.</param>
        /// <param name="y2">Second y-coordinate.</param>
        /// <param name="z1">First z-coordinate.</param>
        /// <param name="z2">Second z-coordinate.</param>
        public void Initialize(double x1, double x2, double y1, double y2, double z1, double z2) {
            var sortedX = this.SortCoordinates(x1, x2);
            _bounds[XIndex][0] = sortedX[0];
            _bounds[XIndex][1] = sortedX[1];

            var sortedY = this.SortCoordinates(y1, y2);
            _bounds[YIndex][0] = sortedY[0];
            _bounds[YIndex][1] = sortedY[1];

            var sortedZ = this.SortCoordinates(z1, z2);
            _bounds[ZIndex][0] = sortedZ[0];
            _bounds[ZIndex][1] = sortedZ[1];
        }

        /// <summary>
        /// Initializes MinX, MaxX, MinY, MaxY, MinZ and MaxZ properties from the given coordinates.
        /// </summary>
        /// <param name="x1">First x-coordinate.</param>
        /// <param name="x2">Second x-coordinate.</param>
        /// <param name="y1">First y-coordinate.</param>
        /// <param name="y2">Second y-coordinate.</param>
        /// <param name="z1">First z-coordinate.</param>
        /// <param name="z2">Second z-coordinate.</param>
        /// <param name="m1">First measure value.</param>
        /// <param name="m2">Second measure value.</param>
        public void Initialize(double x1, double x2, double y1, double y2, double z1, double z2, double m1, double m2) {
            var sortedX = this.SortCoordinates(x1, x2);
            _bounds[XIndex][0] = sortedX[0];
            _bounds[XIndex][1] = sortedX[1];

            var sortedY = this.SortCoordinates(y1, y2);
            _bounds[YIndex][0] = sortedY[0];
            _bounds[YIndex][1] = sortedY[1];

            var sortedZ = this.SortCoordinates(z1, z2);
            _bounds[ZIndex][0] = sortedZ[0];
            _bounds[ZIndex][1] = sortedZ[1];

            var sortedM = this.SortCoordinates(m1, m2);
            _bounds[MIndex][0] = sortedM[0];
            _bounds[MIndex][1] = sortedM[1];
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sorts two coordinates
        /// </summary>
        /// <param name="c1">First coordinate.</param>
        /// <param name="c2">Second coordinate.</param>
        /// <returns>Array with sorted coordinates - [min, max]</returns>
        /// <remarks>If any value is <c>double.NaN</c> the other is used for min and max.</remarks>
        private double[] SortCoordinates(double c1, double c2) {
            if (double.IsNaN(c1)) {
                c1 = c2;
            }

            if (double.IsNaN(c2)) {
                c2 = c1;
            }

            if (c1 > c2) {
                return new double[] { c2, c1 };
            } else {
                return new double[] { c1, c2 };
            }
        }

        #endregion
    }
}