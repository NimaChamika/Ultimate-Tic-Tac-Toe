using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TTT_AlphaBeta
{
    public class GameManager_AlphaBeta : MonoBehaviour
    {
        //PLAYER - X
        //AI - O


        #region PROPERTIES
        [SerializeField] private Transform board;
        private Button[,] boardBtnArray;
        private Text[,] boardTextArray;
        [SerializeField] private Text stateText;

        private readonly string yourSymbol = "X";
        private readonly string AISymbol = "O";
        #endregion

        #region UNITY METHODS
        private void Awake()
        {
            boardBtnArray = new Button[3, 3];
            boardTextArray = new Text[3, 3];

            for (int i=0;i<board.childCount;i++)
            {
                boardBtnArray[i / 3, i%3] = board.GetChild(i).GetComponent<Button>();
                boardTextArray[i / 3, i % 3] = board.GetChild(i).GetChild(0).GetComponent<Text>();
            }

            ResetBoard(); 
        }
        #endregion

        #region BOARD METHODS
        //YOUR BTN CLICK
        public void CellClick(int index)
        {
            (int r, int c) cellPos = ( index / 3,index % 3);

            boardTextArray[cellPos.r, cellPos.c].text = yourSymbol;
            DisableAllCells();


            //CHECK FOR WIN
            if (CheckForWin(CopyBoard(boardTextArray),true) == WINSTATE.notfinished)
            {
                StartCoroutine(BotMove());
            }           
        }

        public void ResetBoard()
        {
            //ENABLE BTNS,CLEAR TEXTS
            for (int i=0;i<3;i++)
            {
                for(int j=0;j<3;j++)
                {
                    boardTextArray[i, j].text = "";
                    boardTextArray[i, j].color = Color.white;
                    boardBtnArray[i, j].enabled = true;
                }
            }

            stateText.text = "YOUR TURN";
        }

        private void DisableAllCells()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    boardBtnArray[i, j].enabled = false;
                }
            }
        }

        private void EnableEmptyCells()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardTextArray[i, j].text == "")
                    {
                        boardBtnArray[i, j].enabled = true;
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

 
