using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Union-Find（Disjoint Set）データ構造の実装
public class DisjointSet
{
    private int[] parent;

    public DisjointSet(int size)
    {
        parent = new int[size];
        for (int i = 0; i < size; i++)
            parent[i] = i;
    }

    public int Find(int x)
    {
        if (parent[x] == x)
            return x;
        return parent[x] = Find(parent[x]);
    }

    public void Union(int x, int y)
    {
        int xRoot = Find(x);
        int yRoot = Find(y);
        if (xRoot != yRoot)
            parent[yRoot] = xRoot;
    }
}
