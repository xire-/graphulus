using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public interface IMassPoint
{
    float Mass { get; set; }
    Vector3 Position { get; set; }
}

internal class MassPoint : IMassPoint
{
    public float Mass { get; set; }
    public Vector3 Position { get; set; }
}


public class OctreeNode<T> where T : IMassPoint
{
    // Spatial properties
    internal Vector3 center;
    internal float half_width;

    // is this node is a leaf, contains the points in this area
    internal List<T> mass_points;

    // childs array, set to null if the node is a leaf(contains some mass point)
    internal OctreeNode<T>[] childs;

    // convenience fields
    internal float total_mass;
    internal Vector3 barycenter;

    private const int MAX_DEPTH = 10;

    public OctreeNode(Vector3 center, float half_width)
    {
        this.center = center;
        this.half_width = half_width;

        //this.childs = new OctreeNode<T>[8];
        this.childs = null;
        this.mass_points = new List<T>();

        this.total_mass = 0;
        this.barycenter = this.center;
    }

    internal void UpdateMassCenter(T obj)
    {
        // compute new barycenter with a weighted means
        barycenter = (barycenter * total_mass + obj.Position * obj.Mass) / (total_mass + obj.Mass);
        // update the total mass
        total_mass += obj.Mass;
    }

    internal void AddObject(T obj, int depth = 1)
    {
        // whatever the case update this node total mass and barycenter
        UpdateMassCenter(obj);

        // if this node has just been created or the max depth has been reached
        // just add the node
        if (childs == null || depth == MAX_DEPTH)
        {
            mass_points.Add(obj);
        }
        else
        {
            if (childs == null)
            {
                childs = new OctreeNode<T>[8];
            }
            mass_points.Add(obj);
            foreach (T item in mass_points)
            {
                // determine postiton of item in childs
                Vector3 offset = item.Position - this.center;
                Vector3 direction = new Vector3(offset.x >= 0 ? 1 : -1, offset.y >= 0 ? 1 : -1, offset.z >= 0 ? 1 : -1);
                int position = (offset.x >= 0 ? 1 : 0) + (offset.y >= 0 ? 2 : 0) + (offset.z >= 0 ? 4 : 0);
                // create the corresponding node
                childs[position] = new OctreeNode<T>(center + direction * (half_width / 2), half_width / 2);
                childs[position].AddObject(item, depth + 1);
            }
            mass_points.Clear();
        }
    }
}



public class BarnesHutOctree<T> where T : IMassPoint
{
    private Vector3 center;
    private float half_width;
    private float ratio;

    private OctreeNode<T> root;

    public BarnesHutOctree(Vector3 center, float half_width, float ratio = 0.5f)
    {
        this.center = center;
        this.half_width = half_width;
        this.ratio = ratio;

        this.root = null;
    }

    public void AddObject(T obj)
    {
        // special case for first object, create the root and add it
        if (root == null)
        {
            root = new OctreeNode<T>(center, half_width);
        }
        root.AddObject(obj);
    }

    public IEnumerable<IMassPoint> GetNearBodies(Vector3 position)
    {
        // get iterator of barnes hut bodies from a given position
        foreach (var i in IterateBodies(root, position)) yield return i;
    }

    private IEnumerable<IMassPoint> IterateBodies(OctreeNode<T> curr_node, Vector3 position)
    {
        if ((curr_node.half_width * 2) / (curr_node.barycenter - position).magnitude < ratio)
        {
            yield return new MassPoint() { Mass = curr_node.total_mass, Position = curr_node.barycenter };
        }
        else if (curr_node.mass_points.Count > 0)
        {
            foreach (var mp in curr_node.mass_points)
            {
                yield return mp;
            }
        }
        else
        {
            foreach (var child in curr_node.childs)
            {
                if (child == null) continue;
                foreach (var mp in IterateBodies(child, position))
                {
                    yield return mp;
                }
            }
        }
    }
}
