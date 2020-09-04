using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : MonoBehaviour
{
    private static int MIN_SIZE = 5;
    private static Random rnd = new Random();

    private int top, left, width, height;
    private Rectangle leftChild;
    private Rectangle rightChild;
    private Rectangle dungeon;

    public Rectangle(int top, int left, int height, int width)
    {
        this.top = top;
        this.left = left;
        this.width = width;
        this.height = height;

    }

    public bool split()
    {
        if (leftChild != null) //이미 분할되었다면,  할 필요 X
            return false;
        bool horizontal = rnd.nextBoolean();

    }
}
