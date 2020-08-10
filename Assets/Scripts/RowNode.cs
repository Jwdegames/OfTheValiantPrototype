using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowNode<T>
{
    private int row;
    private RowNode<T> nextRow;
    private ColNode<T> nextCol;

    public RowNode(int r, RowNode<T> nR, ColNode<T> nC)
    {
        row = r;
        nextRow = nR;
        nextCol = nC;
    }

    public void setRow(int r)
    {
        row = r;
    }

    public int getRow()
    {
        return row;
    }

    public void setNextRow(RowNode<T> nR)
    {
        nextRow = nR;
    }

    public RowNode<T> getNextRow()
    {
        return nextRow;
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
