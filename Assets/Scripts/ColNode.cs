using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColNode<T>
{
    private T val;
    private int col;
    private ColNode<T> nextCol;

    public ColNode(T v, int c, ColNode<T> nC)
    {
        val = v;
        col = c;
        nextCol = nC;
    }

    public void setVal(T v)
    {
        val = v;
    }

    public T getVal()
    {
        return val;
    }

    public void setCol(int c)
    {
        col = c;
    }

    public int getCol()
    {
        return col;
    }

    public void setNextCol(ColNode<T> nC)
    {
        nextCol = nC;
    }

    public ColNode<T> getNextCol()
    {
        return nextCol;
    }

}
