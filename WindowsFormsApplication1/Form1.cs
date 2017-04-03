using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Chess : Form
    {
        private static char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };     //координаты горизонтальной оси на доске
        private int height = 45;                                                        //высота клетки
        private int width = 45;                                                         //ширина клетки
        private Piece? a;//sadasda
        private int b;                                                                  //a, b, c для обмена координатами и элементами массива
        private int c;
        private bool[,] possible_moves = new bool[8, 8];                                //массив для возможных ходов, предназначенных для выбранной фигуры
        private bool click = true;                                                      //первое нажатие (выбор фигуры), если true, второе нажатие (выбор места), если false
        private const string boardSavedGameFile = "savedGame.dat";      
        private Board board = new Board();     
        private void go(int k1, int k2, bool[,] possible_moves, Graphics g)             //отмечает возможные ходы для выбранной фигуры
        {                                                                               //и делает невозможным ход на неотмеченную клетку
            draw(GetFileName("blackout"), k1, k2, g);
            possible_moves[k1, k2] = true;
        }
        
        private void go(int k1, int k2)
        {
            go(k1, k2, this.possible_moves, this.boardPictureBox.CreateGraphics());
        }

        private void DrawBoard(Board board, Graphics g)                 //рисует новое поле поверх старого
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    //string name_of_piece = (i + j) % 2 == 0 ? "white_square" : "black_square"; 
                    string name_of_piece;
                    if ((i + j) % 2 == 0)
                    {
                        name_of_piece = "white_square";
                    }
                    else
                    {
                        name_of_piece = "black_square";
                    }
                    draw(GetFileName(name_of_piece), j, i, g);
                    DrawPiece(board[i, j], j, i, g);                    //рисование фигур с расположением из placement_of_pieces
                }
        }

        private void DrawBoard(Board board)
        {
            DrawBoard(board, boardPictureBox.CreateGraphics());
        }

        static string GetFileName(string name_of_piece)                 //создает полное имя файла из пути, имени картинки и расширения
        {
            const string path = "chess_pieces//";                       //путь к файлу
            const string filename_extension = ".png";                   //расширение файла
            return path + name_of_piece + filename_extension;           //полное имя файла
        }

        private void draw(string filename, int x, int y, Graphics g)     //рисует картинку из файла в заданной координате
        {
            Image picture = GetPiecePicture(filename);
            Point location = new Point(x * width, y * height);           //присваивает location координаты
            g.DrawImage(picture, location);                              //рисует картинку из данного файла с координатами
        }
        
        static Dictionary<string, Image> pictures = new Dictionary<string, Image>();

        private static Image GetPiecePicture(string filename)
        {
            Image picture;
            if (pictures.ContainsKey(filename))                         //если картинка уже была, то берет ее из коллекции, чтобы не загружать снова
                picture = pictures[filename];
            else
            {
                picture = Image.FromFile(filename);                     //первая закрузка картинки
                pictures.Add(filename, picture);
            }
            return picture;
        }

        private void DrawPiece(Piece? figure, int x, int y, Graphics g) //расстановка фигур
        {
            if (figure == null)
            {
                return;
            }
            string filename;
            switch (figure.Value)
            {
                case Piece.BlackPawn: filename = "black_pawn"; break;
                case Piece.BlackRook: filename = "black_rook"; break;
                case Piece.BlackKnight: filename = "black_knight"; break;
                case Piece.BlackBishop: filename = "black_bishop"; break;
                case Piece.BlackQueen: filename = "black_queen"; break;
                case Piece.BlackKing: filename = "black_king"; break;
                case Piece.WhitePawn: filename = "white_pawn"; break;
                case Piece.WhiteRook: filename = "white_rook"; break;
                case Piece.WhiteKnight: filename = "white_knight"; break;
                case Piece.WhiteBishop: filename = "white_bishop"; break;
                case Piece.WhiteQueen: filename = "white_queen"; break;
                case Piece.WhiteKing: filename = "white_king"; break;
                default:
                    throw new NotSupportedException();
            }
            draw(GetFileName(filename), x, y, g);
        }

        public Chess()
        {
            InitializeComponent();            
        }

        private static string GetFieldName(int x, int y)
        {
            return letters[x].ToString() + (8 - y).ToString();
        }
        
        private void boardPictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawBoard(board, e.Graphics);                    //первое рисование поля и фигур
        }

        private void boardPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X / height;
            int y = e.Y / width;
            if (click)//первое нажатие(выбор фигуры)
            {
                PieceColor? color = board.GetPieceColor(y, x);
                if ((color == PieceColor.White && board.IsMoveOfWhite) ||
                    (color == PieceColor.Black && board.IsMoveOfBlack))
                {                    
                    HandleFirstClickOnPiece(e, x, y);
                    click = false;
                }
            }
            else//второе нажатие(выбор места)
            {
                HandleSecondClickOnPiece(x, y);
            }
        }

        private void HandleFirstClickOnPiece(MouseEventArgs e, int x, int y)
        {
            draw(GetFileName("blackout"), x, y, boardPictureBox.CreateGraphics()); 
            a = board[y, x];
            b = e.X / height;
            c = e.Y / width;
            Piece? piece = board[y, x];
            switch (piece)//определение фигуры
            {
                case Piece.WhitePawn:
                case Piece.BlackPawn:
                    GetPossibleMovesForPawn(x, y, Board.GetPieceColor(piece.Value));
                    break;
                case Piece.WhiteRook:
                case Piece.BlackRook:
                    GetPossibleMovesForRook(x, y, Board.GetPieceColor(piece.Value));
                    break;
                case Piece.WhiteKnight:
                case Piece.BlackKnight:
                    GetPossibleMovesForKnight(x, y, Board.GetPieceColor(piece.Value));
                    break;
                case Piece.WhiteBishop:
                case Piece.BlackBishop:
                    GetPossibleMovesForBishop(x, y, Board.GetPieceColor(piece.Value));
                    break;
                case Piece.WhiteQueen:
                case Piece.BlackQueen:
                    GetPossibleMovesForQueen(x, y, Board.GetPieceColor(piece.Value));
                    break;
                case Piece.WhiteKing:
                case Piece.BlackKing:
                    GetPossibleMovesForKing(x, y, Board.GetPieceColor(piece.Value));
                    break;
            }
        }

        private static PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
                        
        private void GetPossibleMovesForPawn(int x, int y, PieceColor color)
        {
            PieceColor oppositeColor = GetOppositeColor(color);
            if (color == PieceColor.White)
            {
                if (y == 6 && board[y - 1, x] == null)                                         //ход с начальной позиции на 1 клетку
                    go(x, y - 1);
                if (y == 6 && board[y - 2, x] == null && board[y - 1, x] == null)              //ход с начальной позиции на 2 клетки
                    go(x, y - 2);
                if (y != 0 && x != 0 && board.GetPieceColor(y - 1, x - 1) == oppositeColor)    //спереди слева вражеская фигура
                    go(x - 1, y - 1);
                if (y != 0 && x != 7 && board.GetPieceColor(y - 1, x + 1) == oppositeColor)    //спереди справа вражеская фигура
                    go(x + 1, y - 1);
                if (y != 0 && board[y - 1, x] == null && y != 6)                               //спереди свободно
                    go(x, y - 1);             
            }
            else
            {
                if (y == 1 && board[y + 1, x] == null)                                         //ход с начальной позиции на 1 клетку
                    go(x, y + 1);
                if (y == 1 && board[y + 2, x] == null && board[y + 1, x] == null)              //ход с начальной позиции на 2 клетки
                    go(x, y + 2);
                if (y != 7 && x != 0 && board.GetPieceColor(y + 1, x - 1) == oppositeColor)    //спереди слева вражеская фигура
                    go(x - 1, y + 1);
                if (y != 7 && x != 7 && board.GetPieceColor(y + 1, x + 1) == oppositeColor)    //спереди справа вражеская фигура
                    go(x + 1, y + 1);
                if (y != 7 && board[y + 1, x] == null && y != 1)                               //спереди свободно
                    go(x, y + 1);
            }
        }

        private void GetPossibleMovesForRook(int x, int y, PieceColor color)
        {
            PieceColor oppositeColor = GetOppositeColor(color);
            int i = 1;
            while (x - i + 1 != 0)//свободно слева
            {
                if (board.GetPieceColor(y, x - i) == color)
                    break;
                if (board.GetPieceColor(y, x - i) == oppositeColor)
                {
                    go(x - i, y);
                    break;
                }
                if (board[y, x - i] == null)
                    go(x - i, y);
                i++;
            }//while (x - i + 1 != 0)//свободно слева
            i = 1;

            while (x + i - 1 != 7)//свободно справа
            {
                if (board.GetPieceColor(y, x + i) == color)
                    break;
                if (board.GetPieceColor(y, x + i) == oppositeColor)
                {
                    go(x + i, y);
                    break;
                }
                if (board[y, x + i] == null)
                    go(x + i, y);
                i++;
            }//while (x + i - 1 != 7)//свободно справа
            i = 1;

            while (y - i + 1 != 0)//свободно сверху
            {
                if (board.GetPieceColor(y - i, x) == color)
                    break;
                if (board.GetPieceColor(y - i, x) == oppositeColor)
                {
                    go(x, y - i);
                    break;
                }
                if (board[y - i, x] == null)
                    go(x, y - i);
                i++;
            }//while (y - i + 1 != 0)//свободно сверху
            i = 1;

            while (y + i - 1 != 7)//свободно снизу
            {
                if (board.GetPieceColor(y + i, x) == color)
                    break;
                if (board.GetPieceColor(y + i, x) == oppositeColor)
                {
                    go(x, y + i);
                    break;
                }
                if (board[y + i, x] == null)
                    go(x, y + i);
                i++;
            }//while (y + i - 1 != 7)//свободно снизу
        }

        private void GetPossibleMovesForKnight(int x, int y, PieceColor color)
        {
            if (x - 2 >= 0 && y - 1 >= 0)                                                   //левая верхняя клетка
                if (board.GetPieceColor(y - 1, x - 2) != color)
                    go(x - 2, y - 1);
            if (x - 2 >= 0 && y + 1 <= 7)                                                   //левая нижняя клетка
                if (board.GetPieceColor(y + 1, x - 2) != color)
                    go(x - 2, y + 1);
            if (x + 2 <= 7 && y - 1 >= 0)                                                   //правая верхняя клетка
                if (board.GetPieceColor(y - 1, x + 2) != color)
                    go(x + 2, y - 1);
            if (x + 2 <= 7 && y + 1 <= 7)                                                   //правая нижняя клетка
                if (board.GetPieceColor(y + 1, x + 2) != color)
                    go(x + 2, y + 1);
            if (y - 2 >= 0 && x - 1 >= 0)                                                   //верхняя левая клетка
                if (board.GetPieceColor(y - 2, x - 1) != color)
                    go(x - 1, y - 2);
            if (y - 2 >= 0 && x + 1 <= 7)                                                   //верхняя правая клетка
                if (board.GetPieceColor(y - 2, x + 1) != color)
                    go(x + 1, y - 2);
            if (y + 2 <= 7 && x - 1 >= 0)                                                   //нижняя левая клетка
                if (board.GetPieceColor(y + 2, x - 1) != color)
                    go(x - 1, y + 2);
            if (y + 2 <= 7 && x + 1 <= 7)                                                   //нижняя правая клетка
                if (board.GetPieceColor(y + 2, x + 1) != color)
                    go(x + 1, y + 2);
        }

        private void GetPossibleMovesForBishop(int x, int y, PieceColor color)
        {
            PieceColor oppositeColor = GetOppositeColor(color);
            int i = 1;
            while (x - i + 1 != 0 && y - i + 1 != 0)//свободно слева сверху
            {
                if (board.GetPieceColor(y - i, x - i) == color)
                    break;
                if (board.GetPieceColor(y - i, x - i) == oppositeColor)
                {
                    go(x - i, y - i);
                    break;
                }
                if (board[y - i, x - i] == null)
                    go(x - i, y - i);
                i++;
            }//while (x - i + 1 != 0 && y - i + 1 != 0)//свободно слева сверху
            i = 1;

            while (x + i - 1 != 7 && y - i + 1 != 0)//свободно справа сверху
            {
                if (board.GetPieceColor(y - i, x + i) == color)
                    break;
                if (board.GetPieceColor(y - i, x + i) == oppositeColor)
                {
                    go(x + i, y - i);
                    break;
                }
                if (board[y - i, x + i] == null)
                    go(x + i, y - i);
                i++;
            }//while (x + i - 1 != 7 && y - i + 1 != 0)//свободно справа сверху
            i = 1;

            while (x - i + 1 != 0 && y + i - 1 != 7)//свободно слева снизу
            {
                if (board.GetPieceColor(y + i, x - i) == color)
                    break;
                if (board.GetPieceColor(y + i, x - i) == oppositeColor)
                {
                    go(x - i, y + i);
                    break;
                }
                if (board[y + i, x - i] == null)
                    go(x - i, y + i);
                i++;
            }//while (x - i + 1 != 0 && y + i - 1 != 7)//свободно слева снизу
            i = 1;

            while (x + i - 1 != 7 && y + i - 1 != 7)//свободно справа снизу
            {
                if (board.GetPieceColor(y + i, x + i) == color)
                    break;
                if (board.GetPieceColor(y + i, x + i) == oppositeColor)
                {
                    go(x + i, y + i);
                    break;
                }
                if (board[y + i, x + i] == null)
                    go(x + i, y + i);
                i++;
            }//while (x + i - 1 != 7 && y + i - 1 != 7)//свободно справа снизу
        }
        
        private void GetPossibleMovesForQueen(int x, int y, PieceColor color)
        {
            PieceColor oppositeColor = GetOppositeColor(color);
            int i = 1;
            while (x - i + 1 != 0 && y - i + 1 != 0)//свободно слева сверху
            {
                if (board.GetPieceColor(y - i, x - i) == color)
                    break;
                if (board.GetPieceColor(y - i, x - i) == oppositeColor)
                {
                    go(x - i, y - i);
                    break;
                }
                if (board[y - i, x - i] == null)
                    go(x - i, y - i);
                i++;
            }//while (x - i + 1 != 0 && y - i + 1 != 0)//свободно слева сверху
            i = 1;

            while (x + i - 1 != 7 && y - i + 1 != 0)//свободно справа сверху
            {
                if (board.GetPieceColor(y - i, x + i) == color)
                    break;
                if (board.GetPieceColor(y - i, x + i) == oppositeColor)
                {
                    go(x + i, y - i);
                    break;
                }
                if (board[y - i, x + i] == null)
                    go(x + i, y - i);
                i++;
            }//while (x + i - 1 != 7 && y - i + 1 != 0)//свободно справа сверху
            i = 1;

            while (x - i + 1 != 0 && y + i - 1 != 7)//свободно слева снизу
            {
                if (board.GetPieceColor(y + i, x - i) == color)
                    break;
                if (board.GetPieceColor(y + i, x - i) == oppositeColor)
                {
                    go(x - i, y + i);
                    break;
                }
                if (board[y + i, x - i] == null)
                    go(x - i, y + i);
                i++;
            }//while (x - i + 1 != 0 && y + i - 1 != 7)//свободно слева снизу
            i = 1;

            while (x + i - 1 != 7 && y + i - 1 != 7)//свободно справа снизу
            {
                if (board.GetPieceColor(y + i, x + i) == color)
                    break;
                if (board.GetPieceColor(y + i, x + i) == oppositeColor)
                {
                    go(x + i, y + i);
                    break;
                }
                if (board[y + i, x + i] == null)
                    go(x + i, y + i);
                i++;
            }//while (x + i - 1 != 7 && y + i - 1 != 7)//свободно справа снизу
            i = 1;

            while (x - i + 1 != 0)//свободно слева
            {
                if (board.GetPieceColor(y, x - i) == color)
                    break;
                if (board.GetPieceColor(y, x - i) == oppositeColor)
                {
                    go(x - i, y);
                    break;
                }
                if (board[y, x - i] == null)
                    go(x - i, y);
                i++;
            }//while (x - i + 1 != 0)//свободно слева
            i = 1;

            while (x + i - 1 != 7)//свободно справа
            {
                if (board.GetPieceColor(y, x + i) == color)
                    break;
                if (board.GetPieceColor(y, x + i) == oppositeColor)
                {
                    go(x + i, y);
                    break;
                }
                if (board[y, x + i] == null)
                    go(x + i, y);
                i++;
            }//while (x + i - 1 != 7)//свободно справа
            i = 1;

            while (y - i + 1 != 0)//свободно сверху
            {
                if (board.GetPieceColor(y - i, x) == color)
                    break;
                if (board.GetPieceColor(y - i, x) == oppositeColor)
                {
                    go(x, y - i);
                    break;
                }
                if (board[y - i, x] == null)
                    go(x, y - i);
                i++;
            }//while (y - i + 1 != 0)//свободно сверху
            i = 1;

            while (y + i - 1 != 7)//свободно снизу
            {
                if (board.GetPieceColor(y + i, x) == color)
                    break;
                if (board.GetPieceColor(y + i, x) == oppositeColor)
                {
                    go(x, y + i);
                    break;
                }
                if (board[y + i, x] == null)
                    go(x, y + i);
                i++;
            }//while (y + i - 1 != 7)//свободно снизу
        }

        private void GetPossibleMovesForKing(int x, int y, PieceColor color)
        {
            if (x - 1 >= 0 && y - 1 >= 0)                                               //левая верхняя клетка
                if (board.GetPieceColor(y - 1, x - 1) != color)
                    go(x - 1, y - 1);
            if (y - 1 >= 0)                                                             //верхняя клетка
                if (board.GetPieceColor(y - 1, x) != color)
                    go(x, y - 1);
            if (x + 1 <= 7 && y - 1 >= 0)                                               //правая верхняя клетка
                if (board.GetPieceColor(y - 1, x + 1) != color)
                    go(x + 1, y - 1);
            if (x + 1 <= 7)                                                             //правая клетка
                if (board.GetPieceColor(y, x + 1) != color)
                    go(x + 1, y);
            if (x + 1 <= 7 && y + 1 <= 7)                                               //правая нижняя клетка
                if (board.GetPieceColor(y + 1, x + 1) != color)
                    go(x + 1, y + 1);
            if (y + 1 <= 7)                                                             //нижняя клетка
                if (board.GetPieceColor(y + 1, x) != color)
                    go(x, y + 1);
            if (y + 1 <= 7 && x - 1 >= 0)                                               //нижняя левая клетка
                if (board.GetPieceColor(y + 1, x - 1) != color)
                    go(x - 1, y + 1);
            if (x - 1 >= 0)                                                             //левая клетка
                if (board.GetPieceColor(y, x - 1) != color)
                    go(x - 1, y);
        }

        private void HandleSecondClickOnPiece(int x, int y)
        {
            bool moveMade = false;

            if (possible_moves[x, y] && board[y, x] == null)//ход на пустую клетку
            {
                
                board.ChangeMove();
                moveMade = true;
                board[c, b] = board[y, x];
                board[y, x] = a;
                click = true;
                Array.Clear(possible_moves, 0, 64);
                Promotion(x, y);
                DrawBoard(board);
            }

            if (possible_moves[x, y] && board[y, x] != null)//взятие фигуры
            {
                board.ChangeMove();
                moveMade = true;
                board[y, x] = board[c, b];
                board[c, b] = null;
                click = true;
                Array.Clear(possible_moves, 0, 64);
                Promotion(x, y);
                DrawBoard(board);
            }

            if (x == b && y == c)//отмена хода
            {
                DrawBoard(board);
                click = true;
                Array.Clear(possible_moves, 0, 64);
            }

            if (moveMade)
            {
                string text = GetFieldName(b, c) + GetFieldName(x, y) + "\r\n";
                textBox1.Text += text;
                board.AddToHistory(text);
            }
        }

        private void Promotion(int x, int y)
        {
            if (board[y, x] == Piece.BlackPawn && y == 7)
            {
                board[y, x] = Piece.BlackQueen;
            }
            if (board[y, x] == Piece.WhitePawn && y == 0)
            {
                board[y, x] = Piece.WhiteQueen;
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            board.Reset();
            textBox1.Clear();
            click = true;
            Array.Clear(possible_moves, 0, 64);
            DrawBoard(board);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(boardSavedGameFile))
            {
                try
                {
                    board.LoadFromFile(boardSavedGameFile);
                    textBox1.Text = board.History;
                }
                catch
                {
                    //Игнорируем ошибку загрузки.
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            board.SaveToFile(boardSavedGameFile);
        }        
    }
}
