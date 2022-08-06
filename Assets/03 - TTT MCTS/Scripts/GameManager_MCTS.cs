using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTT_MCTS
{
    //https://www.faqcode4u.com/faq/364516/monte-carlo-tree-search-implementation-for-tic-tac-toe
    //https://www.youtube.com/watch?v=q3TO5pH1i9c&list=PLLfIBXQeu3aanwI5pYz6QyzYtnBEgcsZ8&index=12&ab_channel=CodeMonkeyKing
    //https://tech.io/snippet/JOG9cSV
    //https://github.com/rfrerebe/MCTS/blob/master/docs/Monte-Carlo%20Tree%20Search%20A%20New%20Framework%20for%20Game%20AI.pdf
    //https://www.youtube.com/watch?v=gvlO_-Fdk9w&ab_channel=DennisRoof
    //https://github.com/maksimKorzh/tictactoe-mtcs/tree/master/src/tictactoe
    //https://towardsdatascience.com/monte-carlo-tree-search-an-introduction-503d8c04e168

    enum WINSTATE
    {
        loose = -1,
        tie = 0,
        win = 1,
        notfinished
    }

    public class GameManager_MCTS : MonoBehaviour
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

        MCTS mcts;
        #endregion

        #region UNITY METHODS
        private void Awake()
        {
            boardBtnArray = new Button[3, 3];
            boardTextArray = new Text[3, 3];

            for (int i = 0; i < board.childCount; i++)
            {
                boardBtnArray[i / 3, i % 3] = board.GetChild(i).GetComponent<Button>();
                boardTextArray[i / 3, i % 3] = board.GetChild(i).GetChild(0).GetComponent<Text>();
            }

            ResetBoard();

            mcts = new MCTS();
        }
        #endregion

        #region BOARD METHODS
        //YOUR BTN CLICK
        public void CellClick(int index)
        {
            (int r, int c) cellPos = (index / 3, index % 3);

            boardTextArray[cellPos.r, cellPos.c].text = yourSymbol;
            DisableAllCells();


            //CHECK FOR WIN
            if (CheckForWin(CopyBoard(boardTextArray), true) == WINSTATE.notfinished)
            {
                StartCoroutine(BotMove());
            }
        }

        public void ResetBoard()
        {
            //ENABLE BTNS,CLEAR TEXTS
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
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

        


        private WINSTATE CheckForWin(string[,] textArray, bool realMove)
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
            if (textArray[0, 0] != "" && textArray[0, 0] == textArray[1, 1] && textArray[1, 1] == textArray[2, 2])
            {

                if (realMove)
                {
                    ChageTextColorForWin((0, 0), (1, 1), (2, 2));
                    Debug.Log(textArray[0, 0] == AISymbol ? "COMPUTER WINS" : "YOU WIN");
                    stateText.text = textArray[0, 0] == AISymbol ? "COMPUTER WIN" : "YOU WINS";
                }
                return textArray[0, 0] == AISymbol ? WINSTATE.win : WINSTATE.loose;
            }


            if (textArray[0, 2] != "" && textArray[0, 2] == textArray[1, 1] && textArray[1, 1] == textArray[2, 0])
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
                        return WINSTATE.notfinished;
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

        private void ChageTextColorForWin(params (int r, int c)[] posArray)
        {
            foreach (var pos in posArray)
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


            if (CheckForWin(CopyBoard(boardTextArray), true) == WINSTATE.notfinished)
            {
                EnableEmptyCells();
            }


        }

        private void GetBotBestMove()
        {

            (int r, int c) bestMove = (-1, -1);
            int bestScore = int.MinValue;


            string[,] boardCopy = CopyBoard(boardTextArray);
            bestMove = mcts.GetBestMove(boardCopy,(-1,-1));
        
            //for (int i = 0; i < 9; i++)
            //{
            //    int r = i / 3;
            //    int c = i % 3;

            //    if (boardTextArray[r, c].text == "")
            //    {

            //        string[,] boardCopy = CopyBoard(boardTextArray);
            //        boardCopy[r, c] = AISymbol;


            //        int score = AlphaBetaPrune(boardCopy, 9, false, int.MinValue, int.MaxValue);
            //        Debug.Log(score);

            //        if (score > bestScore)
            //        {
            //            bestScore = score;
            //            bestMove = (r, c);
            //        }

            //    }
            //}

            boardTextArray[bestMove.r, bestMove.c].text = AISymbol;
        }

        

        private string[,] CopyBoard(Text[,] orignalBoard)
        {
            string[,] boardCopy = new string[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    boardCopy[i, j] = orignalBoard[i, j].text;
                }
            }

            return boardCopy;
        }
        #endregion
    }

    class TreeNode
    {
        public string[,] board;
        public (int r, int c) move;
        public TreeNode parent;
        public bool AITurn;
        public bool isTerminal;
        public bool isFullyExpanded;
        public int visits;
        public int score;
        public List<TreeNode> children;

        private static readonly string yourSymbol = "X";
        private static readonly string AISymbol = "O";


        public TreeNode(string[,] currentBoard,(int r,int c) move, TreeNode parentNode,bool AITurn=true)
        {
            WINSTATE _currentState = Utils.CheckForWin(currentBoard);


            this.board = currentBoard;
            this.move = move;
            this.parent = parentNode;
            this.AITurn = AITurn;
            //if(parent != null)
            //{
            //    currentBoard[move.r, move.c] = AIMove ? AISymbol : yourSymbol;
            //}
            

            this.isTerminal = _currentState == WINSTATE.notfinished ? false : true;
            this.isFullyExpanded = this.isTerminal;
            this.visits = 0;
            this.score = 0;
            children = new List<TreeNode>();
        }
    }

    class MCTS
    {
        private static readonly string yourSymbol = "X";
        private static readonly string AISymbol = "O";

        public (int r,int c) GetBestMove(string[,] boardState,(int r,int c) move)
        {
           

            //Setup root and initial variables
            TreeNode root = new TreeNode(boardState,move,null);

            

            //four phases: selection,expand,roll-out(simulation),backpropagation
            //-----------------------------------------------------------------------------------------------------
            for (int iteration = 0; iteration < 300000; iteration++)
            {
                TreeNode currentNode = Selection(root);
                int value = Rollout(currentNode);
                BackPropagate(currentNode, value);
            }
            
            return BestChildUCB(root, 0,true).move;
        }

        public TreeNode Selection(TreeNode currentNode)
        {
            

            while (!currentNode.isTerminal)
            {
                var validMoves = Utils.GetValidMoves(currentNode.board);
                
                if (validMoves.Count > currentNode.children.Count)
                    return Expand(currentNode);
                else
                    currentNode = BestChildUCB(currentNode, 5000f,false);

                
            }
            return currentNode;
        }

        //#2. Expand a node by creating a new move and returning the node
        public TreeNode Expand(TreeNode currentNode)
        {

            var validMoves = Utils.GetValidMoves(currentNode.board);

            for (int i = 0; i < validMoves.Count; i++)
            {
                //We already have evaluated this move
                if (currentNode.children.Exists(a => a.move == validMoves[i]))
                    continue;

                string[,] boardCopy = (string[,])currentNode.board.Clone();
                bool AITurn = currentNode.AITurn;
                boardCopy[validMoves[i].r, validMoves[i].c] = AITurn ? AISymbol : yourSymbol;
                // int playerActing = Opponent(current.PlayerTookAction);
                TreeNode node = new TreeNode(boardCopy, validMoves[i], currentNode, !AITurn);
                currentNode.children.Add(node);
                
                return node;
            }
            throw new Exception("Error");
        }

        //#3. Roll-out. Simulate a game with a given policy and return the value
        public int Rollout(TreeNode currentNode)
        {
            System.Random r = new System.Random();

            string[,] boardCopy = (string[,])currentNode.board.Clone();

            bool _AIMove = currentNode.AITurn ? true : false;

            int count = 9;

            //Do the policy until a winner is found for the first (change?) node added
            while (Utils.CheckForWin(boardCopy) == WINSTATE.notfinished)
            {
                //Random
                var moves = Utils.GetValidMoves(boardCopy);
                int x = r.Next(0, moves.Count);
                (int r,int c) move = moves[x];
                boardCopy[move.r, move.c] = _AIMove ? AISymbol : yourSymbol;
                _AIMove = !_AIMove;
                count--;
            }

            var state = Utils.CheckForWin(boardCopy);

            if(state == WINSTATE.win)
            {
                return 2 * count;
            }
            else if(state == WINSTATE.tie)
            {
                return 1 * count;
            }
            else
            {
                return -1 * count;
            }

            
        }


        public void BackPropagate(TreeNode currentNode, int value)
        {
            do
            {
                currentNode.visits++;
                currentNode.score += value;
                currentNode = currentNode.parent;
            }
            while (currentNode != null);
        }

        public TreeNode BestChildUCB(TreeNode current, float explorationConstant,bool check)
        {
            TreeNode _bestChild = null;
            float _bestScore = float.MinValue;

            foreach (TreeNode child in current.children)
            {
                //double UCB1Value = ((double)child.score / (double)child.visits) + explorationConstant * Math.Sqrt((2.0 * Math.Log((double)current.visits)) / (double)child.visits);
                float firstPart = (float)child.score/ (float)child.visits ;
                float secondPart = (float)(explorationConstant * Math.Sqrt((float)Math.Log(current.visits) /(float)child.visits ));


                float UCB1Value = firstPart + secondPart;

                if(check)
                {
                    Debug.Log($"{child.score} {child.visits} {UCB1Value}");
                }

                


                if (UCB1Value > _bestScore)
                {
                    _bestChild = child;
                    _bestScore = UCB1Value;
                }
            }
            return _bestChild;
        }
    }


    class Utils
    {
        private static readonly string yourSymbol = "X";
        private static readonly string AISymbol = "O";

        public static WINSTATE CheckForWin(string[,] gameBoard)
        {

            //CHECK FOR ROWS
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[i, 0] != "" && gameBoard[i, 0] == gameBoard[i, 1] && gameBoard[i, 1] == gameBoard[i, 2])
                {
                    return gameBoard[i, 0] == AISymbol ? WINSTATE.win : WINSTATE.loose;
                }
            }


            //CHECK FOR COLUMNS
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[0, i] != "" && gameBoard[0, i] == gameBoard[1, i] && gameBoard[1, i] == gameBoard[2, i])
                {
                    return gameBoard[0, i] == AISymbol ? WINSTATE.win : WINSTATE.loose;
                }

            }


            //DIAGONALS
            if (gameBoard[0, 0] != "" && gameBoard[0, 0] == gameBoard[1, 1] && gameBoard[1, 1] == gameBoard[2, 2])
            {
                return gameBoard[0, 0] == AISymbol ? WINSTATE.win : WINSTATE.loose;
            }


            if (gameBoard[0, 2] != "" && gameBoard[0, 2] == gameBoard[1, 1] && gameBoard[1, 1] == gameBoard[2, 0])
            {
                return gameBoard[0, 2] == AISymbol ? WINSTATE.win : WINSTATE.loose;
            }

            //TIE
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (gameBoard[i, j] == "")
                    {
                        return WINSTATE.notfinished;
                    }
                }
            }

            return WINSTATE.tie;
        }

        public static List<(int r,int c)> GetValidMoves(string[,] gameBoard)
        {
            List<(int r, int c)> validMoveList = new List<(int r, int c)>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(gameBoard[i,j]=="")
                    {
                        validMoveList.Add((i,j));
                    }
                }
            }

            return validMoveList;
        }
    }


}




