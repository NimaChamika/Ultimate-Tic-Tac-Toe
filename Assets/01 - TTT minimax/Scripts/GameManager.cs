using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TTT_MiniMax
{
    public class GameManager : MonoBehaviour
    {
        //PLAYER - X
        //AI - O


        #region PROPERTIES
        [SerializeField] private Transform board;
        private Button[,] btnArray;
        private Text[,] textArray;

        private readonly string yourSymbol = "X";
        private readonly string AISymbol = "O";
        private bool yourTurn;
        #endregion

        #region UNITY METHODS
        private void Awake()
        {
            btnArray = new Button[3, 3];
            textArray = new Text[3, 3];

            for (int i=0;i<board.childCount;i++)
            {
                btnArray[i / 3, i%3] = board.GetChild(i).GetComponent<Button>();
                textArray[i / 3, i % 3] = board.GetChild(i).GetChild(0).GetComponent<Text>();
            }

            ResetBoard();


            yourTurn = true;
        }
        #endregion

        #region BOARD METHODS
        //YOUR BTN CLICK
        public void CellClick(int index)
        {
            (int r, int c) cellPos = ( index / 3,index % 3);

            textArray[cellPos.r, cellPos.c].text = yourSymbol;
            DisableAllCells();


            //CHECK FOR WIN
            if (!CheckForWin())
            {
                yourTurn = false;

                StartCoroutine(BotMove());
            }           
        }

        private void ResetBoard()
        {
            //ENABLE BTNS,CLEAR TEXTS
            for (int i=0;i<3;i++)
            {
                for(int j=0;j<3;j++)
                {
                    textArray[i, j].text = "";
                    btnArray[i, j].enabled = true;
                }
            }
        }

        private void DisableAllCells()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    btnArray[i, j].enabled = false;
                }
            }
        }

        private void EnableEmptyCells()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (textArray[i, j].text == "")
                    {
                        btnArray[i, j].enabled = true;
                    }
                }
            }

            yourTurn = true;
        }

        private bool CheckForWin()
        {
            //CHECK FOR ROWS
            for (int i = 0; i < 3; i++)
            {
                if(textArray[i, 0].text != "" && textArray[i,0].text == textArray[i,1].text && textArray[i, 1].text  == textArray[i, 2].text)
                {
                    Debug.Log(yourTurn ? "YOU WIN" : "COMPUTER WINS");
                    ChageTextColorForWin((i,0),(i,1),(i,2));
                    return true;
                }

            }

            //CHECK FOR COLUMNS
            for (int i = 0; i < 3; i++)
            {
                if (textArray[0, i].text != "" && textArray[0, i].text == textArray[1, i].text && textArray[1, i].text == textArray[2, i].text)
                {
                    Debug.Log(yourTurn ? "YOU WIN" : "COMPUTER WINS");
                    ChageTextColorForWin((0,i), (1, i), (2, i));
                    return true;
                }

            }


            //DIAGONALS
            if (textArray[0, 0].text != "" && textArray[0, 0].text == textArray[1, 1].text && textArray[1,1].text == textArray[2, 2].text)
            {
                Debug.Log(yourTurn ? "YOU WIN" : "COMPUTER WINS");
                ChageTextColorForWin((0, 0), (1, 1), (2, 2));
                return true;
            }


            if (textArray[0, 2].text != "" &&  textArray[0,2].text == textArray[1, 1].text && textArray[1, 1].text == textArray[2, 0].text)
            {
                Debug.Log(yourTurn ? "YOU WIN" : "COMPUTER WINS");
                ChageTextColorForWin((0, 2), (1, 1), (2, 0));
                return true;
            }

            //TIE
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (textArray[i, j].text == "")
                    {
                        return false;
                    }
                }
            }

            Debug.Log("TIE");
            return true;
        }
        
        private void ChageTextColorForWin(params (int r,int c)[] posArray)
        {
            foreach(var pos in posArray)
            {
                textArray[pos.r, pos.c].color = Color.red;
            }
        }
        #endregion

        #region BOT METHODS
        private IEnumerator BotMove()
        {
            yield return new WaitForSeconds(1);

            GetBotBestMove();

            //CHECK FOR WIN
            if (!CheckForWin())
            {
                EnableEmptyCells();
            }
 

        }

        private void GetBotBestMove()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (textArray[i, j].text == "")
                    {
                        textArray[i, j].text = AISymbol;
                        return;
                    }
                }
            }
        }

        #endregion
    }
}

 
