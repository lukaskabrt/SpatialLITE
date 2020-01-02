using SpatialLite.Core.API;
using SpatialLite.Core.Collections.QuadTree;
using System.Collections.Generic;

namespace SpatialLite.Core.Collections {
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    class QuadTreeNode<T> where T : IGeometry {
        private const int maxObjectsPerNode = 10;

        private List<QuadTreeObject<T>> objects = null;
        private Envelope rect; // The area this QuadTree represents

        private QuadTreeNode<T> parent = null; // The parent of this quad

        private QuadTreeNode<T> childTL = null; // Top Left Child
        private QuadTreeNode<T> childTR = null; // Top Right Child
        private QuadTreeNode<T> childBL = null; // Bottom Left Child
        private QuadTreeNode<T> childBR = null; // Bottom Right Child

        /// <summary>
        /// The area this QuadTree represents.
        /// </summary>
        public Envelope QuadRect {
            get { return rect; }
        }

        /// <summary>
        /// The top left child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> TopLeftChild {
            get { return childTL; }
        }

        /// <summary>
        /// The top right child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> TopRightChild {
            get { return childTR; }
        }

        /// <summary>
        /// The bottom left child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> BottomLeftChild {
            get { return childBL; }
        }

        /// <summary>
        /// The bottom right child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> BottomRightChild {
            get { return childBR; }
        }

        /// <summary>
        /// This QuadTree's parent
        /// </summary>
        public QuadTreeNode<T> Parent {
            get { return parent; }
        }

        /// <summary>
        /// The objects contained in this QuadTree at it's level (ie, excludes children)
        /// </summary>
        //public List<T> Objects { get { return m_objects; } }
        internal List<QuadTreeObject<T>> Objects {
            get { return objects; }
        }

        /// <summary>
        /// How many total objects are contained within this QuadTree (ie, includes children)
        /// </summary>
        public int Count {
            get { return ObjectCount(); }
        }

        /// <summary>
        /// Returns true if this is a empty leaf node
        /// </summary>
        public bool IsEmptyLeaf {
            get { return Count == 0 && childTL == null; }
        }

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        public QuadTreeNode(Envelope rect) {
            this.rect = rect;
        }


        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        public QuadTreeNode(int x, int y, int width, int height) {
            rect = new Envelope(x, x + width, y, y + height);
        }

        private QuadTreeNode(QuadTreeNode<T> parent, Envelope rect)
            : this(rect) {
            this.parent = parent;
        }

        /// <summary>
        /// Add an item to the object list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(QuadTreeObject<T> item) {
            if (objects == null) {
                //m_objects = new List<T>();
                objects = new List<QuadTreeObject<T>>();
            }

            item.Owner = this;
            objects.Add(item);
        }


        /// <summary>
        /// Remove an item from the object list.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        private void Remove(QuadTreeObject<T> item) {
            if (objects != null) {
                int removeIndex = objects.IndexOf(item);
                if (removeIndex >= 0) {
                    objects[removeIndex] = objects[objects.Count - 1];
                    objects.RemoveAt(objects.Count - 1);
                }
            }
        }


        /// <summary>
        /// Get the total for all objects in this QuadTree, including children.
        /// </summary>
        /// <returns>The number of objects contained within this QuadTree and its children.</returns>
        private int ObjectCount() {
            int count = 0;

            // Add the objects at this level
            if (objects != null) {
                count += objects.Count;
            }

            // Add the objects that are contained in the children
            if (childTL != null) {
                count += childTL.ObjectCount();
                count += childTR.ObjectCount();
                count += childBL.ObjectCount();
                count += childBR.ObjectCount();
            }

            return count;
        }


        /// <summary>
        /// Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        private void Subdivide() {
            // We've reached capacity, subdivide...
            Coordinate size = new Coordinate(rect.Width / 2, rect.Height / 2);
            Coordinate mid = new Coordinate(rect.MinX + size.X, rect.MinY + size.Y);

            childTL = new QuadTreeNode<T>(this, new Envelope(rect.MinX, rect.MinX + size.X, mid.Y, mid.Y + size.Y));
            childTR = new QuadTreeNode<T>(this, new Envelope(mid.X, mid.X + size.X, mid.Y, mid.Y + size.Y));
            childBL = new QuadTreeNode<T>(this, new Envelope(rect.MinX, rect.MinX + size.X, rect.MinY, rect.MinY + size.Y));
            childBR = new QuadTreeNode<T>(this, new Envelope(mid.X, mid.X + size.X, rect.MinY, rect.MinY + size.Y));

            // If they're completely contained by the quad, bump objects down
            for (int i = 0; i < objects.Count; i++) {
                QuadTreeNode<T> destTree = GetDestinationTree(objects[i]);

                if (destTree != this) {
                    // Insert to the appropriate tree, remove the object, and back up one in the loop
                    destTree.Insert(objects[i]);
                    Remove(objects[i]);
                    i--;
                }
            }
        }


        /// <summary>
        /// Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private QuadTreeNode<T> GetDestinationTree(QuadTreeObject<T> item) {
            // If a child can't contain an object, it will live in this Quad
            QuadTreeNode<T> destTree = this;

            if (childTL.QuadRect.Covers(item.Data.GetEnvelope())) {
                destTree = childTL;
            } else if (childTR.QuadRect.Covers(item.Data.GetEnvelope())) {
                destTree = childTR;
            } else if (childBL.QuadRect.Covers(item.Data.GetEnvelope())) {
                destTree = childBL;
            } else if (childBR.QuadRect.Covers(item.Data.GetEnvelope())) {
                destTree = childBR;
            }

            return destTree;
        }


        private void Relocate(QuadTreeObject<T> item) {
            // Are we still inside our parent?
            if (QuadRect.Covers(item.Data.GetEnvelope())) {
                // Good, have we moved inside any of our children?
                if (childTL != null) {
                    QuadTreeNode<T> dest = GetDestinationTree(item);
                    if (item.Owner != dest) {
                        // Delete the item from this quad and add it to our child
                        // Note: Do NOT clean during this call, it can potentially delete our destination quad
                        QuadTreeNode<T> formerOwner = item.Owner;
                        Delete(item, false);
                        dest.Insert(item);

                        // Clean up ourselves
                        formerOwner.CleanUpwards();
                    }
                }
            } else {
                // We don't fit here anymore, move up, if we can
                if (parent != null) {
                    parent.Relocate(item);
                }
            }
        }


        private void CleanUpwards() {
            if (childTL != null) {
                // If all the children are empty leaves, delete all the children
                if (childTL.IsEmptyLeaf &&
                    childTR.IsEmptyLeaf &&
                    childBL.IsEmptyLeaf &&
                    childBR.IsEmptyLeaf) {
                    childTL = null;
                    childTR = null;
                    childBL = null;
                    childBR = null;

                    if (parent != null && Count == 0) {
                        parent.CleanUpwards();
                    }
                }
            } else {
                // I could be one of 4 empty leaves, tell my parent to clean up
                if (parent != null && Count == 0) {
                    parent.CleanUpwards();
                }
            }
        }

        /// <summary>
        /// Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        internal void Clear() {
            // Clear out the children, if we have any
            if (childTL != null) {
                childTL.Clear();
                childTR.Clear();
                childBL.Clear();
                childBR.Clear();
            }

            // Clear any objects at this level
            if (objects != null) {
                objects.Clear();
                objects = null;
            }

            // Set the children to null
            childTL = null;
            childTR = null;
            childBL = null;
            childBR = null;
        }


        /// <summary>
        /// Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children, it's children will be removed as well.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="clean">Whether or not to clean the tree</param>
        internal void Delete(QuadTreeObject<T> item, bool clean) {
            if (item.Owner != null) {
                if (item.Owner == this) {
                    Remove(item);
                    if (clean) {
                        CleanUpwards();
                    }
                } else {
                    item.Owner.Delete(item, clean);
                }
            }
        }



        /// <summary>
        /// Insert an item into this QuadTree object.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        internal void Insert(QuadTreeObject<T> item) {
            // If this quad doesn't contain the items rectangle, do nothing, unless we are the root
            if (!rect.Covers(item.Data.GetEnvelope())) {
                System.Diagnostics.Debug.Assert(parent == null, "We are not the root, and this object doesn't fit here. How did we get here?");
                if (parent == null) {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    Add(item);
                } else {
                    return;
                }
            }

            if (objects == null ||
                (childTL == null && objects.Count + 1 <= maxObjectsPerNode)) {
                // If there's room to add the object, just add it
                Add(item);
            } else {
                // No quads, create them and bump objects down where appropriate
                if (childTL == null) {
                    Subdivide();
                }

                // Find out which tree this object should go in and add it there
                QuadTreeNode<T> destTree = GetDestinationTree(item);
                if (destTree == this) {
                    Add(item);
                } else {
                    destTree.Insert(item);
                }
            }
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        internal List<T> GetObjects(Envelope searchRect) {
            List<T> results = new List<T>();
            GetObjects(searchRect, ref results);
            return results;
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        internal void GetObjects(Envelope searchRect, ref List<T> results) {
            // We can't do anything if the results list doesn't exist
            if (results != null) {
                if (searchRect.Covers(this.rect)) {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    GetAllObjects(ref results);
                } else if (searchRect.Intersects(this.rect)) {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (objects != null) {
                        for (int i = 0; i < objects.Count; i++) {
                            if (searchRect.Intersects(objects[i].Data.GetEnvelope())) {
                                results.Add(objects[i].Data);
                            }
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (childTL != null) {
                        childTL.GetObjects(searchRect, ref results);
                        childTR.GetObjects(searchRect, ref results);
                        childBL.GetObjects(searchRect, ref results);
                        childBR.GetObjects(searchRect, ref results);
                    }
                }
            }
        }


        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        /// <param name="results">A reference to a list in which to store the objects.</param>
        internal void GetAllObjects(ref List<T> results) {
            // If this Quad has objects, add them
            if (objects != null) {
                foreach (QuadTreeObject<T> qto in objects) {
                    results.Add(qto.Data);
                }
            }

            // If we have children, get their objects too
            if (childTL != null) {
                childTL.GetAllObjects(ref results);
                childTR.GetAllObjects(ref results);
                childBL.GetAllObjects(ref results);
                childBR.GetAllObjects(ref results);
            }
        }


        /// <summary>
        /// Moves the QuadTree object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        internal void Move(QuadTreeObject<T> item) {
            if (item.Owner != null) {
                item.Owner.Relocate(item);
            } else {
                Relocate(item);
            }
        }
    }
}
