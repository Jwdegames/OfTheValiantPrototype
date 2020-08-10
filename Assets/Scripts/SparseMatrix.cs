using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparseMatrix<T>
{
    private RowNode<T> head;
    private int maxChars = 0;
    private int maxCol = 0;
    private int maxRow = 0;
    public SparseMatrix()
    {
        head = null;
    }

    public void firstNode(T val, int row, int col)
    {
        ColNode<T> temp = new ColNode<T>(val, col, null);
        head = new RowNode<T>(row, null, temp);
    }

    public void insertNode(T val, int row, int col)
    {
        ColNode<T> c1 = null, c2 = null, c3 = null;
        RowNode<T> r1 = null, r2 = null, r3 = null;
        maxRow = Mathf.Max(row, maxRow);
        maxCol = Mathf.Max(col, maxCol);
        c1 = new ColNode<T>(val, col, null);

        if (head == null)
        {
            firstNode(val, row, col);
            return;
        }
        else if (row < head.getRow())
        {
            r1 = new RowNode<T>(row, null, c1);
            head = r1;
        }
        else
        {
            r2 = head;
            while (r2 != null && r2.getRow() < row)
            {
                r3 = r2;
                r2 = r2.getNextRow();
            }
            if (r2 != null && r2.getRow() == row)
            {
                c2 = r2.getNextCol();
                if (c2.getCol() > col)
                {
                    r2.setNextCol(c1);
                    c1.setNextCol(c2);
                }
                else
                {
                    while (c2 != null && c2.getCol() < col)
                    {
                        c3 = c2;
                        c2 = c2.getNextCol();
                    }
                    c3.setNextCol(c1);
                    c1.setNextCol(c2);
                }
            }
            else
            {
                r1 = new RowNode<T>(row, r2, c1);
                r3.setNextRow(r1);
            }
        }
    }

    public T getValue(int r, int c)
    {
        ColNode<T> c1;
        RowNode<T> r1;
        if (head == null || head.getRow() > r)
        {
            return default(T);
        }
        else
        {
            r1 = head;
            //Debug.Log("Value at (" +r+","+c+") has a head of "+r1.getRow());
            if (r1.getRow() == r)
            {
                c1 = r1.getNextCol();
                if (c1 == null || c1.getCol() > c)
                {
                    return default(T);
                }
                //Cycle through the row until we get a null node or until we are greater than or equal to the wanted column
                while (c1 != null && c1.getCol() < c)
                {
                    c1 = c1.getNextCol();
                }
                if (c1 == null || c1.getCol() != c)
                {
                    return default(T);
                }
                else
                {
                    return c1.getVal();
                }
            }
            else
            {
                while (r1 != null && r1.getRow() < r)
                {
                    r1 = r1.getNextRow();
                }
                if (r1 == null || r1.getRow() != r)
                {
                    return default(T);
                }
                else
                {
                    c1 = r1.getNextCol();
                    if (c1 == null || c1.getCol() > c)
                    {
                        return default(T);
                    }
                    //Cycle through the row until we get a null node or until we are greater than or equal to the wanted column
                    while (c1 != null && c1.getCol() < c)
                    {
                        c1 = c1.getNextCol();
                    }
                    if (c1 == null || c1.getCol() != c)
                    {
                        return default(T);
                    }
                    else
                    {
                        return c1.getVal();
                    }
                }
            }
        }
    }

    public int getMaxRow()
    {
        return maxRow;
    }

    public int getMaxCol()
    {
        return maxCol;
    }

}
