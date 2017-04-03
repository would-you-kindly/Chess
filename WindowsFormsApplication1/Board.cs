using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WindowsFormsApplication1
{
    public class Board
    {
        private static Piece?[,] defaultBoard =
        {
            {Piece.BlackRook, Piece.BlackKnight, Piece.BlackBishop, Piece.BlackQueen, Piece.BlackKing, Piece.BlackBishop, Piece.BlackKnight, Piece.BlackRook},                       
            {Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn},
            {null, null, null, null, null, null, null, null},
            {null, null, null, null, null, null, null, null},
            {null, null, null, null, null, null, null, null},
            {null, null, null, null, null, null, null, null},
            {Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn},
            {Piece.WhiteRook, Piece.WhiteKnight, Piece.WhiteBishop, Piece.WhiteQueen, Piece.WhiteKing, Piece.WhiteBishop, Piece.WhiteKnight, Piece.WhiteRook},
        };
        
        private Piece?[,] board; 

        private bool move_of_white;

        private string history;

        public Board()
        {
            Reset();
        }

        public void LoadFromFile(string path)
        {
            using (Stream stream = File.OpenRead(path))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                for (int x = 0; x < 8; x++)
                {
                    string line = streamReader.ReadLine();
                    string[] splitLine = line.Split(new[] { ' ' });
                    for (int y = 0; y < 8; y++)
                    {
                        int value = int.Parse(splitLine[y]);
                        board[x, y] = value != -1 ? (Piece?)(Piece)value : null;
                    }
                }

                move_of_white = streamReader.ReadLine() == "W";
                string tempHistory = streamReader.ReadToEnd().Trim();
                history = string.IsNullOrEmpty(tempHistory) ? tempHistory : tempHistory + Environment.NewLine;
            }
        }

        public void SaveToFile(string path)
        {
            using (Stream stream = File.Create(path))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Piece? piece = board[x, y];
                        writer.Write((piece != null ? (int)piece.Value : -1).ToString());
                        writer.Write(' ');
                    }

                    writer.WriteLine();
                }
                    
                writer.WriteLine(move_of_white ? "W" : "B");
                writer.WriteLine(history);
            }
        }

        public Piece? this [int x, int y]
        {
            get
            {
                return board[x, y];
            }
            set
            {                     
                board[x, y] = value;
            }
        }
        
        public PieceColor? GetPieceColor(int x, int y)
        {
            return GetPieceColor(this[x, y]);
        }

        public void ChangeMove()
        {
            move_of_white = !move_of_white;
        }

        public bool IsMoveOfWhite
        {
            get
            {
                return move_of_white;
            }
        }

        public bool IsMoveOfBlack
        {
            get
            {
                return !move_of_white;
            }
        }

        public string History
        {
            get
            {
                return history;
            }
        }

        public bool[,] GetPossibleMoves(int x, int y)
        {
            return new bool[8, 8];
        }

        public void Reset()
        {
            board = (Piece?[,])defaultBoard.Clone();
            move_of_white = true;
            history = string.Empty;
        }

        public void AddToHistory(string text)
        {
            history += text;
        }

        public static PieceColor? GetPieceColor(Piece? figure)
        {
            if (figure == null)
            {
                return null;
            }

            return GetPieceColor(figure.Value);
        }

        public static PieceColor GetPieceColor(Piece figure)
        {
            switch (figure)
            {
                case Piece.WhitePawn:
                case Piece.WhiteRook:
                case Piece.WhiteKnight:
                case Piece.WhiteBishop:
                case Piece.WhiteQueen:
                case Piece.WhiteKing:
                    return PieceColor.White;
                case Piece.BlackPawn:
                case Piece.BlackRook:
                case Piece.BlackKnight:
                case Piece.BlackBishop:
                case Piece.BlackQueen:
                case Piece.BlackKing:
                    return PieceColor.Black;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
