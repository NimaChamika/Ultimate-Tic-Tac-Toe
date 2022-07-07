using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UTTT_AlphaBeta
{
    public class GameManager_UTTT_AlphaBeta : MonoBehaviour
    {
        //PLAYER - X
        //AI - O


        #region PROPERTIES
        [SerializeField] private Transform mainBoard;

        private List<Button[,]> subBoardBtnList;
        private List<Text[,]> subBoardTextList;

        private Button[,] boardBtnArray;
        private Text[,] boardTextArray;

        [SerializeField] private Text stateText;

        private readonly int childCount = 9;

        private readonly string yourSymbol = "X";
        private readonly string AISymbol = "O";
        #endregion

        #region UNITY METHODS
        private void Awake()
        {
            InitBoard();

            ResetBoard(); 
        }
        #endregion

        #region BOARD METHODS
        private void InitBoard()
        {
            subBoardBtnList = new List<Button[,]>();
            subBoardTextList = new List<Text[,]>();

            boardBtnArray = new Button[3, 3];
            boardTextArray = new Text[3, 3];

            for (int i = 0; i < childCount; i++)
            {
                Transform _tempSubBoardT = mainBoard.GetChild(i);

                Button[,] _tempBtnArray = new Button[3, 3];
                Text[,] _tempTextArray = new Text[3, 3];

                for (int j = 0; j < childCount; j++)
                {
                    _tempBtnArray[j / 3, j % 3] = _tempSubBoardT.GetChild(j).GetComponent<Button>();
                    _tempTextArray[j / 3, j % 3] = _tempSubBoardT.GetChild(j).GetChild(0).GetComponent<Text>();
                }

                subBoardBtnList.Add(_tempBtnArray);
                subBoardTextList.Add(_tempTextArray);
            }
        }

        private void ResetBoard()
        {
            //ENABLE BTNS,CLEAR TEXTS
            for (int i = 0; i < childCount; i++)
            {
                Button[,] _tempBtnArray = subBoardBtnList[i];
                Text[,] _tempTextArray = subBoardTextList[i];

                for (int j = 0; j < childCount; j++)
                {
                    _tempTextArray[j / 3, j % 3].text = "";
                    _tempTextArray[j / 3, j % 3].color = Color.white;
                    _tempBtnArray[j / 3, j % 3].enabled = true;
                }
            }

            stateText.text = "YOUR TURN";
        }

        //YOUR BTN CLICK
        public void CellClick(string pos)
        {
            var _pos = pos.Split(' ').Select(int.Parse).ToArray();

            var _value = GetSubBoardPos((_pos[0], _pos[1]));


            subBoardTextList[_value.i][_value.r, _value.c].text = yourSymbol;
            //DisableAllCells();


            //CHECK FOR WIN
            //if (CheckForWin(CopyBoard(boardTextArray), true) == WINSTATE.notfinished)
            //{
            //    StartCoroutine(BotMove());
            //}
        }

        private static (int i, int r, int c) GetSubBoardPos((int r, int c) pos)
        {
            int listIndex = 0;
            int column = 0;
            int row = 0;

            if (pos.c <= 2)//0,1,2
            {
                listIndex = 0;
            }
            else if (pos.c <= 5)//3,4,5
            {
                listIndex = 1;
            }
            else//6,7,8
            {
                listIndex = 2;
            }

            if (pos.r <= 2)//0,1,2
            {

            }
            else if (pos.r <= 5)//3,4,5
            {
                listIndex += 3;
            }
            else//6,7,8
            {
                listIndex += 6;
            }

            column = pos.c % 3;
            row = pos.r % 3;


            return (listIndex, row, column);
        }


        private void DisableAllCells()
        {
            for (int i = 0; i < childCount; i++)
            {
                Button[,] _tempBtnArray = subBoardBtnList[i];


                for (int j = 0; j < childCount; j++)
                {
                    _tempBtnArray[j / 3, j % 3].enabled = false;
                }
            }
        }

        private void EnableEmptyCells()
        {
            for (int i = 0; i < childCount; i++)
            {
                Button[,] _tempBtnArray = subBoardBtnList[i];
                Text[,] _tempTextArray = subBoardTextList[i];

                for (int j = 0; j < childCount; j++)
                {
                    if(_tempTextArray[j / 3, j % 3].text == "") 
                    {
                        _tempBtnArray[j / 3, j % 3].enabled = true;
                    }
                    
                }
            }

            stateText.text = "YOUR TURN";
        }

        enum WINSTATE
        {
            loose = -1,
            tie = 0,
            win = 2,
            notfinished
        }


        private WINSTATE CheckForWin(string[,] textArray,bool realMove)
        {

            //CHECK FOR ROWS
            for (int i = 0; i < 3; i++)
            {
                if (textArray[i, 0] != "" && textArray[i, 0] == textArray[i, 1] && textArray[i, 1] == textArray[i, 2])
                {

                    
                    if (realMove)
                    {
                        ChageTextColorForWin((i, 0), (i, 1), (i, 2));
                        Debug.Log(textArray[i, 0] == AISymbol ? "COMPUTER WIN" : "YOU WINS");
                        stateText.text = textArray[i, 0] == AISymbol ? "COMPUTER WIN" : "YOU WINS";
                    }
                    return textArray[i, 0] == AISymbol ? WINSTATE.win : WINSTATE.loose;
                }
            }


            //CHECK FOR COLUMNS
            for (int i = 0; i < 3; i++)
            {
                if (textArray[0, i] != "" && textArray[0, i] == textArray[1, i] && textArray[1, i] == textArray[2, i])
                {
                    
                    if (realMove)
                    {
                        ChageTextColorForWin((0, i), (1, i), (2, i));
                        Debug.Log(textArray[0, i] == AISymbol ? "COMPUTER WINS" : "YOU WIN");
                        stateText.text = textArray[0, i] == AISymbol ? "COMPUTER WIN" : "YOU WINS";
                    }
                    return textArray[0, i] == AISymbol ? WINSTATE.win : WINSTATE.loose;
                }

            }


            //DIAGONALS
            if (textArray[0, 0] != "" && textArray[0, 0] == textArray[1, 1] && textArray[1,1] == textArray[2, 2])
            {
                
                if (realMove)
                {
                    ChageTextColorForWin((0, 0), (1, 1), (2, 2));
                    Debug.Log(textArray[0, 0] == AISymbol ? "COMPUTER WINS" : "YOU WIN");
                    stateText.text = textArray[0, 0] == AISymbol ? "COMPUTER WIN" : "YOU WINS";
                }
                return textArray[0, 0] == AISymbol ? WINSTATE.win : WINSTATE.loose;
            }


            if (textArray[0, 2] != "" &&  textArray[0,2] == textArray[1, 1] && textArray[1, 1] == textArray[2, 0])
            {

                if (realMove)
                {
                    ChageTextColorForWin((0, 2), (1, 1), (2, 0));
                    Debug.Log(textArray[0, 2] == AISymbol ? "COMPUTER WINS" : "YOU WIN");
                    stateText.text = textArray[0, 2] == AISymbol ? "COMPUTER WIN" : "YOU WINS";
                }
                return textArray[0, 2] == AISymbol ? WINSTATE.win : WINSTATE.loose;
            }

            //TIE
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (textArray[i, j] == "")
                    {
                        return  WINSTATE.notfinished;
                    }
                }
            }

            if (realMove)
            {
                Debug.Log("TIE");
                stateText.text = "TIE";
            }
            
            return WINSTATE.tie;
        }
        
        private void ChageTextColorForWin(params (int r,int c)[] posArray)
        {
            foreach(var pos in posArray)
            {
                boardTextArray[pos.r, pos.c].color = Color.red;
            }
        }
        #endregion

        #region BOT METHODS
        private IEnumerator BotMove()
        {
            stateText.text = "AI TURN";

            yield return new WaitForSeconds(1);

            GetBotBestMove();


            if (CheckForWin(CopyBoard(boardTextArray),true) == WINSTATE.notfinished)
            {
                EnableEmptyCells();
            }
 

        }

        private void GetBotBestMove()
        {

            (int r, int c) bestMove = (-1, -1);
            int bestScore = int.MinValue;


            for (int i = 0; i < 9; i++)
            {
                int r = i / 3;
                int c = i % 3;

                if (boardTextArray[r, c].text == "")
                {

                    string[,] boardCopy = CopyBoard(boardTextArray);
                    boardCopy[r, c] = AISymbol;

                    int score = AlphaBetaPrune(boardCopy, 9, false,int.MinValue,int.MaxValue);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = (r, c);
                    }

                }
            }

            boardTextArray[bestMove.r, bestMove.c].text = AISymbol;
        }

        private int AlphaBetaPrune(string[,] board, int depth, bool isMaximizer,int alpha,int beta)
        {
            WINSTATE winState = CheckForWin(board,false);

            //Console.Error.WriteLine(boardState);

            if (winState != WINSTATE.notfinished)
            {
                return (int)winState * depth;
            }
            else
            {
                int maxScore = int.MinValue;
                int minScore = int.MaxValue;

                for (int i = 0; i < 9; i++)
                {
                    int r = i / 3;
                    int c = i % 3;


                    if (board[r, c] == "")
                    {
                        string[,] boardCopy = (string[,])board.Clone();

                        if (isMaximizer)
                        {
                            boardCopy[r, c] = AISymbol;
                            maxScore = Math.Max(maxScore, AlphaBetaPrune(boardCopy, depth - 1, false,alpha,beta));

                            alpha = Math.Max(alpha,maxScore);
                            if(alpha >= beta)
                            {
                                break;
                            }
                        }
                        else
                        {
                            boardCopy[r, c] = yourSymbol;
                            minScore = Math.Min(minScore, AlphaBetaPrune(boardCopy, depth - 1, true,alpha,beta));

                            beta = Math.Min(beta,minScore);
                            if (alpha >= beta)
                            {
                                break;
                            }

                        }

                    }
                }

                return isMaximizer ? maxScore : minScore;
            }

        }
        
        private string[,] CopyBoard(Text[,] orignalBoard)
        {
            string[,] boardCopy = new string[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    boardCopy[i,j] = orignalBoard[i, j].text;
                }
            }

            return boardCopy;
        }
        #endregion
    }
}

 
