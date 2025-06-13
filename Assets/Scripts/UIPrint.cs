using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPrint : MonoBehaviour
{
    Board board;
    MakeTile maker;
    Text text;
    public bool high;
    public bool block;
    void Start()
    {
        text = GetComponent<Text>();
        if (!block)
        {
            board = GameObject.FindObjectOfType<Board>();
        }
        else
        {
            maker = GameObject.FindObjectOfType<MakeTile>();
        }
    }
    void Update()
    {
        if (high)
        {
            text.text = board.HighScore.ToString("HIGH SCORE : #,##0");
        }
        else if(!block)
        {
             text.text = board.Score.ToString("SCORE : #,##0");
        }
        else
        {
            text.text = (minmaxvalue(0, 5,maker.TileCount - (maker.IsLineCount()==0?-1: maker.IsLineCount()))).ToString("남은 타일 수 : #,##0");
        }
    }
    int minmaxvalue(int min, int max, int input)
    {
        if (min > input) input = min;
        else if (max < input) input = max;
        return input;
    }
}
