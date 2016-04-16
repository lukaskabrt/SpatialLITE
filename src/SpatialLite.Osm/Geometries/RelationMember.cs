using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace SpatialLite.Osm.Geometries {
	/// <summary>
	/// Ecapsules member of the OSM relation and it's role in the relation.
	/// </summary>
	public class RelationMember : Geometry {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the RelationMember class with empty Role and specified Member.
		/// </summary>
		/// <param name="member">The member of this RelatioinMember.</param>
		public RelationMember(IOsmGeometry member)
			: this(member, null) {
		}

		/// <summary>
		/// Initializes a new instance of the RelationMember class with specified Role and Member.
		/// </summary>
		/// <param name="member">The member of this RelationMember.</param>
		/// <param name="role">The Role of this member in the relation.</param>
		public RelationMember(IOsmGeometry member, string role)
			: base() {
			if (member == null) {
				throw new ArgumentNullException("member");
			}

			this.Member = member;
			this.Role = role;

			if (member is Node) {
				this.MemberType = EntityType.Node;
			}
			else if (member is Way) {
				this.MemberType = EntityType.Way;
			}
			else if (member is Relation) {
				this.MemberType = EntityType.Relation;
			}
			else {
				throw new ArgumentException("Unknown member type.");
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the actual member.
		/// </summary>
		public IOsmGeometry Member { get; private set; }

		/// <summary>
		/// Gets or sets role of the member in the relation.
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// Gets the type of the Member.
		/// </summary>
		public EntityType MemberType { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Geometry"/> has Z coordinates.
		/// </summary>
		public override bool Is3D {
			get {
				return ((Geometry)this.Member).Is3D;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Geometry"/> has M values.
		/// </summary>
		public override bool IsMeasured {
			get {
				return ((Geometry)this.Member).IsMeasured;
			}
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Creates a new instance of the RelationMember class based on the data from RelationInfo object.
		/// </summary>
		/// <param name="info">The RelationMemberInfo object that contains data about member.</param>
		/// <param name="entities">The entities that can be referenced by RelationMember.</param>
		/// <param name="throwOnMissing">bool value indicating whether references to the missing entity should cause exception.</param>
		/// <returns>The RelationMember object created from RelationMemberInfo or null if referenced node is missing.</returns>
        public static RelationMember FromRelationMemberInfo(RelationMemberInfo info, IEntityCollection<IOsmGeometry> entities, bool throwOnMissing) {
			if (info.MemberType == EntityType.Unknown) {
				throw new ArgumentException("info.MemberType cannot be EntityType.Unknown");
			}

			if (entities.Contains(info.Reference, info.MemberType) == false) {
				if (throwOnMissing) {
					throw new ArgumentException(string.Format("Referenced Entity (ID = {0}, type = {1}) not found in entities collection.", info.Reference, info.MemberType), "info.Reference");
				}
				else {
					return null;
				}
			}

			RelationMember result = new RelationMember(entities[info.Reference, info.MemberType], info.Role) { MemberType = info.MemberType };
			if (result.Member.EntityType != info.MemberType) {
				throw new ArgumentException("Type of the referenced entity doesn't match type of the entity in the collection.");
			}

			return result;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Computes envelope of the <c>IGeometry</c> object. The envelope is defined as a minimal bounding box for a geometry.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="Envelope"/> object that specifies the minimal bounding box of the <c>Geometry</c> object.
		/// </returns>
		public override Envelope GetEnvelope() {
			return ((Geometry)this.Member).GetEnvelope();
		}

		/// <summary>
		/// Returns  the  closure  of  the  combinatorial  boundary  of  this  geometric  object.
		/// </summary>
		/// <returns> the  closure  of  the  combinatorial  boundary  of  the RelationMember.</returns>
		public override IGeometry GetBoundary() {
			return this.Member.GetBoundary();
		}

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public override IEnumerable<Coordinate> GetCoordinates() {
            return this.Member.GetCoordinates();
        }

        /// <summary>
        /// Applies the specific filter on this geometry
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        public override void Apply(ICoordinateFilter filter) {
            this.Member.Apply(filter);
        }

        #endregion
    }
}
