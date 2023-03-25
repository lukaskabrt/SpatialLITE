using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core;
using SpatialLite.Core.Api;
using SpatialLite.Core.Geometries;
using FluentAssertions;
using Tests.SpatialLite.Core.FluentAssertions;

namespace Tests.SpatialLite.Core.Geometries {
	public class PointTests {

		const double _xCoordinate = 3.5f;
		const double _yCoordinate = 4.2f;
		const double _zCoordinate = 10.5f;

		Coordinate _coordinate = new (_xCoordinate, _yCoordinate, _zCoordinate);
		
		[Fact]
		public void Constructor__CreatesPointWithEmptyPosition() {
			Point target = new Point();

			target.Position.IsEmpty.Should().BeTrue();
		}

		[Fact]
		public void Constructor_XY_SetsCoordinates() {
			var target = new Point(_xCoordinate, _yCoordinate);

			target.Position.ShouldHaveCoordinates(_xCoordinate, _yCoordinate, double.NaN);
		}

		[Fact]
		public void Constructor_XYZ_SetsCoordinates() {
			Point target = new Point(_xCoordinate, _yCoordinate, _zCoordinate);

			target.Position.ShouldHaveCoordinates(_xCoordinate, _yCoordinate, _zCoordinate);
		}

		[Fact]
		public void Constructor_Coordinate_SetsCoordinates() {
			var target = new Point(_coordinate);

			target.Position.Should().Equals(_coordinate);
		}

		[Fact]
		public void Is3D_ReturnsTrueFor3DPoint() {
			var target = new Point(_xCoordinate, _yCoordinate, _zCoordinate);

			target.Is3D.Should().BeTrue();
		}

		[Fact]
		public void Is3D_ReturnsFalseFor2DPoint() {
			var target = new Point(_xCoordinate, _yCoordinate);

			target.Is3D.Should().BeFalse();
		}

		[Fact]
		public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyPoint() {
			var target = new Point();
			
			var envelope = target.GetEnvelope();

			envelope.IsEmpty.Should().BeTrue();
		}

		[Fact]
		public void GetEnvelope_ReturnsEnvelopeThatCoversOnePoint() {
			var target = new Point(_coordinate);

			var envelope = target.GetEnvelope();

			envelope.ShouldHaveBounds(_xCoordinate, _xCoordinate, _yCoordinate, _yCoordinate);
		}
	}
}
