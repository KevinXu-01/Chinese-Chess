using Chinese_Chess.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Chinese_Chess
{

    //走棋方枚举类型
    public enum Player { 无 = 0, 红 = 1, 黑 = 2 };

    //棋子枚举类型
    public enum Piece
    {
        无子 = 0,
        红车 = 1, 红马 = 2, 红相 = 3, 红士 = 4, 红帅 = 5, 红炮 = 6, 红兵 = 7,
        黑车 = 8, 黑马 = 9, 黑象 = 10, 黑士 = 11, 黑将 = 12, 黑炮 = 13, 黑卒 = 14
    }
    public partial class MainWnd : Form
    {
        public static int mode = 0; //0为双人离线，1为在线，2为人机
        public static Player CurrentPlayer;
        private static bool isStart = false;
        private const int CB_GAP_X = 85;
        private const int CB_GAP_Y = 85;
        private const int CB_OFFSET_X = 430;//棋盘左、上存在420单位的间隙
        private const int CB_OFFSET_Y = 212;//棋盘左、上存在205单位的间隙
        private const int Piece_R = 35;
        private Piece[,] ChessBoard = new Piece[11, 10];//10行9列，(0,0)弃用
        private List<Step> _stepList = new List<Step>();
        private Point currentMousePosition = new Point(0, 0);
        private bool HumanFirst = true;
        public static Player player;//人机和在线模式区分玩家
        private static bool isDropped = false;
        internal static string arg;//模式选择的参数，用于调用
        private static int[] _score = new int[7];
        private Piece[,] ChessBoard_temp = new Piece[11, 10];//10行9列，(0,0)弃用
        //保存拾起的棋子值（默认为 无子 ，即目前没有拾起棋子）
        private Piece _pickChess = Piece.无子;
        private bool isCopied = false;
        private bool isRestored = false;
        //保存拾起棋子的行号和列号
        private int _pickRow = 0;
        private int _pickCol = 0;
        private int mode_from_file;
        //保存落下棋子的行号和列号
        private int _dropRow = 0;
        private int _dropCol = 0;

        //保存红蓝双方14种棋子位图
        private Bitmap[] _pieceBmp = new Bitmap[15];

        public MainWnd()
        {
            _score[0] = 1000; _score[1] = 10; _score[2] = 10; _score[3] = 100; _score[4] = 50; _score[5] = 50; _score[6] = 20;
            mode = Convert.ToInt32(arg);
            InitializeComponent();
            load();
            Init();
            DoubleBuffered = true;

            //创建一个文件流对象，用于读文件
            FileStream fs = new FileStream(@"games.dat", FileMode.Open, FileAccess.Read);
            //创建一个与文件流对象相对应的二进制读入流对象
            BinaryReader br = new BinaryReader(fs);
            mode_from_file = br.ReadInt32();
            br.Close();
            fs.Close();

            if (mode == 0 || mode == 2)
            {
                common_sentences.Enabled = false;
                if (hasContent() && mode_from_file == mode)
                {
                    if (MessageBox.Show("检测到有保存的残局，你是否希望恢复它？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Restore();
                        isRestored = true;
                    }
                }
            }
            if (mode == 1)
            {
                common_sentences.Enabled = true;
                Hide();
                Connect connect_wnd = new Connect();
                connect_wnd.ShowDialog();
                Thread GameEventThread = new Thread(new ThreadStart(Game_Event_Handler));
                GameEventThread.Start();
                game_info.Text += "您是" + Convert.ToString(player) + "方。\n";
            }
            if (mode == 2 && !isRestored)
            {
                if (MessageBox.Show("您是否要先行？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HumanFirst = true;
                    player = Player.红;
                }
                else
                {
                    player = Player.黑;
                    HumanFirst = false;
                    AI_motion();
                }
            }
        }

        private bool hasContent()
        {
            FileStream file = new FileStream(@"games.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader file_read = new StreamReader(file);
            string temp = file_read.ReadLine();
            file_read.Close();
            file.Close();
            if (string.IsNullOrEmpty(temp))
                return false;
            else
                return true;
        }
        private void load()
        {
            //装载红蓝双方所有的棋子位图文件
            //无子 = 0,
            //红车 = 1, 红马 = 2, 红相 = 3, 红士 = 4, 红帅 = 5, 红炮 = 6, 红兵 = 7,
            //黑车 = 8, 黑马 = 9, 黑象 = 10, 黑士 = 11, 黑将 = 12, 黑炮 = 13, 黑卒 = 14
            _pieceBmp[1] = new Bitmap(Resources.Red_Rook, 70, 70); _pieceBmp[2] = new Bitmap(Resources.Red_Knight, 70, 70);
            _pieceBmp[3] = new Bitmap(Resources.Red_Bishop, 70, 70); _pieceBmp[4] = new Bitmap(Resources.Red_Mandarin, 70, 70);
            _pieceBmp[5] = new Bitmap(Resources.Red_King, 70, 70); _pieceBmp[6] = new Bitmap(Resources.Red_Cannon, 70, 70);
            _pieceBmp[7] = new Bitmap(Resources.Red_Pawn, 70, 70);
            _pieceBmp[8] = new Bitmap(Resources.Black_Rook, 70, 70); _pieceBmp[9] = new Bitmap(Resources.Black_Knight, 70, 70);
            _pieceBmp[10] = new Bitmap(Resources.Black_Bishop, 70, 70); _pieceBmp[11] = new Bitmap(Resources.Black_Mandarin, 70, 70);
            _pieceBmp[12] = new Bitmap(Resources.Black_King, 70, 70); _pieceBmp[13] = new Bitmap(Resources.Black_Cannon, 70, 70);
            _pieceBmp[14] = new Bitmap(Resources.Black_Pawn, 70, 70);
        }
        private void Init()
        {
            isRestored = false;
            game_info.Text = "游戏开始...\n提示：红方先行。\n";
            CurrentPlayer = Player.红;
            //初始化画图
            //把象棋棋盘的棋子数组初始化为“无子”
            for (int row = 1; row <= 10; row++)
                for (int col = 1; col <= 9; col++)
                    ChessBoard[row, col] = Piece.无子;

            //黑方
            ChessBoard[1, 1] = Piece.黑车; ChessBoard[1, 2] = Piece.黑马; ChessBoard[1, 3] = Piece.黑象;
            ChessBoard[1, 4] = Piece.黑士; ChessBoard[1, 5] = Piece.黑将; ChessBoard[1, 6] = Piece.黑士;
            ChessBoard[1, 7] = Piece.黑象; ChessBoard[1, 8] = Piece.黑马; ChessBoard[1, 9] = Piece.黑车;

            ChessBoard[3, 2] = Piece.黑炮; ChessBoard[3, 8] = Piece.黑炮;
            ChessBoard[4, 1] = Piece.黑卒; ChessBoard[4, 3] = Piece.黑卒; ChessBoard[4, 5] = Piece.黑卒;
            ChessBoard[4, 7] = Piece.黑卒; ChessBoard[4, 9] = Piece.黑卒;

            //红方
            ChessBoard[10, 1] = Piece.红车; ChessBoard[10, 2] = Piece.红马; ChessBoard[10, 3] = Piece.红相;
            ChessBoard[10, 4] = Piece.红士; ChessBoard[10, 5] = Piece.红帅; ChessBoard[10, 6] = Piece.红士;
            ChessBoard[10, 7] = Piece.红相; ChessBoard[10, 8] = Piece.红马; ChessBoard[10, 9] = Piece.红车;

            ChessBoard[8, 2] = Piece.红炮; ChessBoard[8, 8] = Piece.红炮;
            ChessBoard[7, 1] = Piece.红兵; ChessBoard[7, 3] = Piece.红兵; ChessBoard[7, 5] = Piece.红兵;
            ChessBoard[7, 7] = Piece.红兵; ChessBoard[7, 9] = Piece.红兵;

            _pickChess = Piece.无子;
            _pickRow = _pickCol = 0;
            _dropRow = _dropCol = 0;
            isStart = false;
            isDropped = false;
            Invalidate();
        }
        public void DrawPiece(Graphics g)
        {
            //逐行逐列绘制棋子
            for (int row = 1; row <= 10; row++)
                for (int col = 1; col <= 9; col++)
                {
                    //如果该位置存在棋子
                    if (ChessBoard[row, col] != Piece.无子)
                    {
                        g.DrawImage(_pieceBmp[(int)ChessBoard[row, col]], new Point(CB_OFFSET_X + CB_GAP_X * (col - 1) - Piece_R,
                                                        CB_OFFSET_Y + CB_GAP_Y * (row - 1) - Piece_R));
                    }
                }

            //在当前鼠标点击位置绘制拾起的棋子，以显示出拾起的棋子随鼠标而动的效果
            if (!(mode == 1 && CurrentPlayer != player) && _pickChess != Piece.无子)
                g.DrawImage(_pieceBmp[(int)_pickChess], new Point(currentMousePosition.X - Piece_R, currentMousePosition.Y - Piece_R));
        }
        //窗口绘制事件响应方法
        private void MainWnd_Paint(object sender, PaintEventArgs e)
        {
            //绘制棋子
            DrawPiece(e.Graphics);
            if (isStart && _pickChess != Piece.无子)
                DrawPickDropMark(e.Graphics, _pickRow, _pickCol);//绘制拾子位置标志
        }

        private void MainWnd_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)e;
            if (mouse.X >= 1874 && mouse.Y <= 45)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            if (MousePosition.X >= 1719 && MousePosition.X <= 1764 && MousePosition.Y <= 45)
            {
                WindowState = FormWindowState.Minimized;
                return;
            }
            if (MousePosition.X >= 1798 && MousePosition.X <= 1843 && MousePosition.Y <= 45)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                    return;
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                    return;
                }
            }
            if (MousePosition.X >= 1555 && MousePosition.X <= 1706 && MousePosition.Y >= 430 && MousePosition.Y <= 480)
            {
                Save_click();
                return;
            }
            if (MousePosition.X >= 1555 && MousePosition.X <= 1706 && MousePosition.Y >= 506 && MousePosition.Y <= 556)
            {
                GiveUp_click();
                return;
            }

            if (MousePosition.X >= 1730 && MousePosition.X <= 1881 && MousePosition.Y >= 506 && MousePosition.Y <= 556)
            {
                About about_wnd = new About();
                about_wnd.ShowDialog();
                return;
            }

            if (MousePosition.X >= 1730 && MousePosition.X <= 1881 && MousePosition.Y >= 430 && MousePosition.Y <= 480)
            {
                if (mode == 0)
                {
                    Undo();
                    return;
                }
                else if(mode == 2)
                {
                    MessageBox.Show("当前模式不支持此操作！");
                    return;
                }
                else if (mode == 1 && player != CurrentPlayer)
                {
                    Undo_request();
                    MessageBox.Show("已向对方发送请求，请稍后...");
                    return;
                }
                else if (mode == 1 && player == CurrentPlayer)
                {
                    MessageBox.Show("您此时无法悔棋！");
                    return;
                }
            }

            if (MousePosition.X >= 1725 && MousePosition.X <= 1875 && MousePosition.Y >= 997 && MousePosition.Y <= 1047)
            {
                Send_click();
                return;
            }

            if (MousePosition.X >= 1556 && MousePosition.X <= 1706 && MousePosition.Y >= 997 && MousePosition.Y <= 1047)
            {
                if (mode == 1)
                    common_sentences.Visible = true;
                else
                    MessageBox.Show("当前模式不支持此操作！");
                return;
            }
        }
        private void Undo_request()
        {
            LAN_connection.Lan_connection.SendMsg_Operation(3, 0, "想要悔棋");
        }
        private void Save_click()
        {
            if (!isStart)
            {
                MessageBox.Show("游戏尚未开始！");
                return;
            }
            if (mode == 1)
            {
                MessageBox.Show("当前模式不支持此操作！");
                return;
            }
            //创建一个文件流对象，用于写文件
            FileStream fs = new FileStream(@"games.dat", FileMode.Create);
            //创建一个与文件流对象相对应的二进制写入流对象
            BinaryWriter bw = new BinaryWriter(fs);
            //写入模式
            bw.Write(mode);
            //写入player（AI模式）
            bw.Write((int)player);
            //写入当前的走棋方
            bw.Write((int)CurrentPlayer);
            //逐一写入棋子值
            for (int row = 1; row <= 10; row++)
                for (int col = 1; col <= 9; col++)
                    bw.Write((int)ChessBoard[row, col]);

            //写入走棋步骤数
            bw.Write(_stepList.Count);
            //逐一写入走棋步骤
            for (int i = 0; i < _stepList.Count; i++)
            {
                bw.Write((int)_stepList[i]._Player);
                bw.Write((int)_stepList[i]._PickChess);
                bw.Write(_stepList[i]._PickRow);
                bw.Write(_stepList[i]._PickCol);
                bw.Write((int)_stepList[i]._DropChess);
                bw.Write(_stepList[i]._DropRow);
                bw.Write(_stepList[i]._DropCol);
            }

            //写入最后一步的拾子和落子标志
            bw.Write(_pickRow);
            bw.Write(_pickCol);
            bw.Write(_dropRow);
            bw.Write(_dropCol);

            //关闭所有文件流对象
            bw.Close();
            fs.Close();
            MessageBox.Show("保存成功！");
        }
        private void GiveUp_click()
        {
            if (!isStart)
            {
                MessageBox.Show("游戏尚未开始！");
                return;
            }
            if (mode != 1 && CurrentPlayer == Player.黑)
                MessageBox.Show("红方获胜！");
            else if (mode != 1 && CurrentPlayer == Player.红)
                MessageBox.Show("黑方获胜！");
            if (mode == 1)
            {
                LAN_connection.Lan_connection.SendMsg_Operation(2, 0, "text");
                MessageBox.Show("您输了。");
            }
            Restart();
        }
        private void Restore()
        {
            //创建一个文件流对象，用于读文件
            FileStream fs = new FileStream(@"games.dat", FileMode.Open, FileAccess.Read);
            //创建一个与文件流对象相对应的二进制读入流对象
            BinaryReader br = new BinaryReader(fs);
            mode_from_file = br.ReadInt32();
            player = (Player)br.ReadInt32();

            //读取当前的走棋方
            CurrentPlayer = (Player)br.ReadInt32();

            //逐一读取棋子值
            for (int row = 1; row <= 10; row++)
                for (int col = 1; col <= 9; col++)
                    ChessBoard[row, col] = (Piece)br.ReadInt32();

            //读取走棋步数
            int count = br.ReadInt32();

                //清空原有的走棋步骤
            _stepList.Clear();

            //逐一读取走棋步骤
            for (int i = 0; i < count; i++)
            {
                //读取走棋步骤到List中
                Step step = new Step();
                step._Player = (Player)br.ReadInt32();
                step._PickChess = (Piece)br.ReadInt32();
                step._PickRow = br.ReadInt32();
                step._PickCol = br.ReadInt32();
                step._DropChess = (Piece)br.ReadInt32();
                step._DropRow = br.ReadInt32(); ;
                step._DropCol = br.ReadInt32(); ;
                _stepList.Add(step);
            }

            //读取最后一步的拾子和落子位置
            _pickRow = br.ReadInt32();
            _pickCol = br.ReadInt32();
            _dropRow = br.ReadInt32();
            _dropCol = br.ReadInt32();

            //初始化拾起的棋子值为无子
            _pickChess = Piece.无子;

            //关闭所有文件流对象
            br.Close();
            fs.Close();

            //强制刷新
            Invalidate();
        }

        private void Restart()
        {
            if (!isStart)
            {
                MessageBox.Show("游戏尚未开始！");
                return;
            }
            //清空棋盘，清空游戏记录
            chat_info.Text = "";
            Init();
        }

        private void Undo()
        {
            if(!isStart)
            {
                MessageBox.Show("游戏尚未开始！");
                return;
            }
            if (_stepList.Count >= 1)
            {
                //显示提示确认框
                if (mode != 1)
                {
                    if (MessageBox.Show("您是否需要悔棋？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //取出_stepList的最后一步走棋步骤到lastStep中
                        Step lastStep = _stepList[_stepList.Count - 1];

                        //还原最后一步走棋步骤
                        CurrentPlayer = lastStep._Player;
                        ChessBoard[lastStep._PickRow, lastStep._PickCol] = lastStep._PickChess;
                        ChessBoard[lastStep._DropRow, lastStep._DropCol] = lastStep._DropChess;
                        game_info.Text += Convert.ToString(CurrentPlayer) + "方进行了悔棋，将棋子" +
                            Convert.ToString(lastStep._PickChess) + "还原到(" + Convert.ToString(_pickRow) + ", " +
                            Convert.ToString(_pickCol) + ")。\n";
                        //删除_stepList的最后一步走棋步骤
                        _stepList.RemoveAt(_stepList.Count - 1);

                        //清空拾起的棋子值、拾子行号、拾子列号、落子行号、落子列号
                        _pickChess = Piece.无子;
                        _pickRow = 0;
                        _pickCol = 0;
                        _dropRow = 0;
                        _dropCol = 0;

                        //如果_stepList还有走棋步骤，则需要显示上一步的拾子位置标记和落子位置标记
                        if (_stepList.Count >= 1)
                        {
                            //取出_stepList的最后一步走棋步骤到lastStep中
                            lastStep = _stepList[_stepList.Count - 1];
                            _pickRow = lastStep._PickRow;
                            _pickCol = lastStep._PickCol;
                            _dropRow = lastStep._DropRow;
                            _dropCol = lastStep._DropCol;
                        }
                        //强制刷新
                        Invalidate();
                    }
                }
                else
                {
                    //取出_stepList的最后一步走棋步骤到lastStep中
                    Step lastStep = _stepList[_stepList.Count - 1];

                    //还原最后一步走棋步骤
                    CurrentPlayer = lastStep._Player;
                    ChessBoard[lastStep._PickRow, lastStep._PickCol] = lastStep._PickChess;
                    ChessBoard[lastStep._DropRow, lastStep._DropCol] = lastStep._DropChess;

                    //删除_stepList的最后一步走棋步骤
                    _stepList.RemoveAt(_stepList.Count - 1);

                    //清空拾起的棋子值、拾子行号、拾子列号、落子行号、落子列号
                    _pickChess = Piece.无子;
                    _pickRow = 0;
                    _pickCol = 0;
                    _dropRow = 0;
                    _dropCol = 0;

                    //如果_stepList还有走棋步骤，则需要显示上一步的拾子位置标记和落子位置标记
                    if (_stepList.Count >= 1)
                    {
                        //取出_stepList的最后一步走棋步骤到lastStep中
                        lastStep = _stepList[_stepList.Count - 1];
                        _pickRow = lastStep._PickRow;
                        _pickCol = lastStep._PickCol;
                        _dropRow = lastStep._DropRow;
                        _dropCol = lastStep._DropCol;
                    }

                    //强制刷新
                    Invalidate();
                }
            }
        }
        private void Send_click()
        {
            if (mode == 0 || mode == 2)
            {
                MessageBox.Show("当前模式不支持此操作！");
                chat_txb.Text = string.Empty;
                return;
            }
            if (string.IsNullOrEmpty(chat_txb.Text))
            {
                MessageBox.Show("您发送的内容为空！");
                return;
            }
            LAN_connection.Lan_connection.SendMsg_Operation(1, 0, player.ToString() + "方：\n" + chat_txb.Text + "\n");
            chat_info.Text += player.ToString() + "方：\n" + chat_txb.Text + "\n";
            chat_txb.Text = "";
        }

        private void MainWnd_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePosition.X >= 1874 && MousePosition.Y <= 45 || MousePosition.X >= 1798 && MousePosition.X <= 1843 &&
                MousePosition.Y <= 45 || MousePosition.X >= 1719 && MousePosition.X <= 1764 && MousePosition.Y <= 45 ||
                MousePosition.X >= 1555 && MousePosition.X <= 1706 && MousePosition.Y >= 430 && MousePosition.Y <= 480 ||
                MousePosition.X >= 1730 && MousePosition.X <= 1881 && MousePosition.Y >= 430 && MousePosition.Y <= 480 ||
                MousePosition.X >= 1555 && MousePosition.X <= 1706 && MousePosition.Y >= 506 && MousePosition.Y <= 556 ||
                MousePosition.X >= 1730 && MousePosition.X <= 1881 && MousePosition.Y >= 506 && MousePosition.Y <= 556 ||
                MousePosition.X >= 1555 && MousePosition.X <= 1706 && MousePosition.Y >= 997 && MousePosition.Y <= 1047 ||
                MousePosition.X >= 1725 && MousePosition.X <= 1875 && MousePosition.Y >= 997 && MousePosition.Y <= 1047
                )
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;

            //保存当前鼠标所在点的坐标
            currentMousePosition = e.Location;
            //判断是否已经拾起棋子，以决定是否需要强制刷新窗口
            if (_pickChess != Piece.无子 && !(mode ==1 && CurrentPlayer != player))
            {
                //强制刷新窗口，以绘制出拾起的棋子随鼠标而动的效果
                Invalidate();
            }
        }
        public void DrawPickDropMark(Graphics g, int row, int col)
        {
            //如果拾子或落子位置不存在
            if (row != 0 && isStart)
            {
                Pen pen = new Pen(Color.Yellow, 4);
                //拾子或落子位置的点坐标
                Point p = new Point(CB_OFFSET_X + CB_GAP_X * (col - 1), CB_OFFSET_Y + CB_GAP_Y * (row - 1));

                //在拾子或落子位置的四个角绘制标记
                g.DrawLine(pen, p.X - Piece_R, p.Y - Piece_R, p.X - Piece_R / 2, p.Y - Piece_R);
                g.DrawLine(pen, p.X - Piece_R, p.Y - Piece_R, p.X - Piece_R, p.Y - Piece_R / 2);

                g.DrawLine(pen, p.X - Piece_R, p.Y + Piece_R, p.X - Piece_R / 2, p.Y + Piece_R);
                g.DrawLine(pen, p.X - Piece_R, p.Y + Piece_R, p.X - Piece_R, p.Y + Piece_R / 2);

                g.DrawLine(pen, p.X + Piece_R, p.Y - Piece_R, p.X + Piece_R / 2, p.Y - Piece_R);
                g.DrawLine(pen, p.X + Piece_R, p.Y - Piece_R, p.X + Piece_R, p.Y - Piece_R / 2);

                g.DrawLine(pen, p.X + Piece_R, p.Y + Piece_R, p.X + Piece_R / 2, p.Y + Piece_R);
                g.DrawLine(pen, p.X + Piece_R, p.Y + Piece_R, p.X + Piece_R, p.Y + Piece_R / 2);
            }
        }
        //自定义类方法：把鼠标点击位置坐标转换为棋盘的行号和列号
        public bool ConvertPointToRowCol(Point point, out int row, out int col)
        {
            //获取与鼠标点击位置距离最近的棋盘交叉点的行号
            row = (point.Y - CB_OFFSET_Y) / CB_GAP_Y + 1;
            //如果鼠标点Y坐标超过棋盘行高的中线，则行号需要再加1
            if (((point.Y - CB_OFFSET_Y) % CB_GAP_Y) >= CB_GAP_Y / 2)
                row = row + 1;

            //获取与鼠标点击位置距离最近的棋盘交叉点的列号
            col = (point.X - CB_OFFSET_X) / CB_GAP_X + 1;
            //如果鼠标点X坐标超过棋盘列宽的中线，则列号需要再加1
            if (((point.X - CB_OFFSET_X) % CB_GAP_X) >= CB_GAP_X / 2)
                col = col + 1;

            //获取与鼠标点击位置距离最近的棋盘交叉点的坐标
            Point chessP = new Point();
            chessP.X = CB_OFFSET_X + CB_GAP_X * (col - 1);
            chessP.Y = CB_OFFSET_Y + CB_GAP_Y * (row - 1);

            //判断是否落在棋子半径之内，且在10行9列之内
            double dist = Math.Sqrt(Math.Pow(point.X - chessP.X, 2) + Math.Pow(point.Y - chessP.Y, 2));
            if ((dist <= Piece_R) && (row <= 10) && (row >= 1) && (col <= 9) && (col >= 1))
            {
                //返回true，表示该点击为有效点击
                return true;
            }
            else
            {
                //把行号和列号设置为0，并返回false，表示该点击为无效点击
                return false;
            }
        }
        
        public void Game_Event_Handler()
        {
            /*
             * 1代表聊天
             * 2代表投降
             * 3代表悔棋
             * 4代表游戏数据
             */
            while (true)
            {
                if (LAN_connection.Lan_connection.received)
                {
                    switch (LAN_connection.Lan_connection.gameEvent.Iclass)
                    {
                        case "1":
                            chat_info.Text += LAN_connection.Lan_connection.gameEvent.content;
                            break;
                        case "2":
                            MessageBox.Show("对方投降，您赢了！");
                            Restart();
                            break;
                        case "3":
                            if (LAN_connection.Lan_connection.gameEvent.content == "想要悔棋")
                            {
                                if (MessageBox.Show("对方想要悔棋，是否允许？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                {
                                    LAN_connection.Lan_connection.SendMsg_Operation(3, 0, "Y");
                                    Undo();
                                }
                                else
                                    LAN_connection.Lan_connection.SendMsg_Operation(3, 0, "N");
                            }
                            else if (LAN_connection.Lan_connection.gameEvent.content == "N")
                            {
                                MessageBox.Show("对方不允许悔棋！");
                            }
                            else
                                Undo();
                            break;
                        case "4":
                            string operation = LAN_connection.Lan_connection.gameEvent.content.Split('+')[0];
                            if (operation == "落子")
                            {
                                isStart = true;
                                int number = Convert.ToInt32(LAN_connection.Lan_connection.gameEvent.content.Split('+')[1]);
                                int row = number / 10;
                                int col = number % 10;
                                GameOn(row, col);
                            }
                            else if (operation == "撤销")
                            {
                                //把拾子位置设置为拾起棋子的值
                                ChessBoard[_pickRow, _pickCol] = _pickChess;

                                //清空拾起的棋子值、拾子行号、拾子列号
                                _pickChess = Piece.无子;
                                _pickRow = 0;
                                _pickCol = 0;
                                //强制刷新
                                Invalidate();
                            }
                            break;
                    }
                    LAN_connection.Lan_connection.received = false;
                }
            }
        }
        private void GameOn(int row, int col)
        {
            if (_pickChess == Piece.无子)
            {
                //判断该位置是否有棋子（只有存在棋子时，才能拾起棋子），且棋子属于当前的走棋方
                if (ChessBoard[row, col] != Piece.无子 && ChessBoard[row, col].ToString().IndexOf(CurrentPlayer.ToString()) == 0)
                {
                    //保存拾起棋子的棋子值
                    _pickChess = ChessBoard[row, col];
                    //保存拾起棋子位置的行号和列号
                    _pickRow = row;
                    _pickCol = col;
                    //清空拾起棋子位置的棋子值
                    ChessBoard[_pickRow, _pickCol] = Piece.无子;

                    isDropped = false;
                    Invalidate();
                }
            }
            //处理落子动作
            else
            {
                //判断落子位置是否为无子，或者为对方棋子
                if (ChessBoard[row, col] == Piece.无子 || (ChessBoard[row, col].ToString().IndexOf(CurrentPlayer.ToString()) == -1 &&
                    mode != 1) || (mode == 1 && ChessBoard[row, col].ToString().IndexOf(player.ToString()) == -1 &&
                    !LAN_connection.Lan_connection.received) || (mode == 1 && ChessBoard[row, col].ToString().IndexOf(player.ToString()) == 0 &&
                    LAN_connection.Lan_connection.received))
                {
                    //判断是否可以落子
                    CanDrop(_pickRow, row, col, out bool canDrop);

                    if (canDrop == true)
                    {
                        //保存走棋步骤到_stepList中
                        Step tempStep = new Step();
                        tempStep._Player = CurrentPlayer;
                        tempStep._PickChess = _pickChess;
                        tempStep._PickRow = _pickRow;
                        tempStep._PickCol = _pickCol;
                        tempStep._DropChess = ChessBoard[row, col];
                        tempStep._DropRow = row;
                        tempStep._DropCol = col;
                        _stepList.Add(tempStep);

                        
                        //判断被吃的棋子是否为红帅或蓝将
                        if (ChessBoard[row, col] == Piece.红帅 || ChessBoard[row, col] == Piece.黑将)
                        {
                            //显示获胜方信息
                            MessageBox.Show(CurrentPlayer.ToString() + "方获胜", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Restart();
                        }
                        else
                        {
                            //交换走棋方
                            if (CurrentPlayer == Player.红)
                                CurrentPlayer = Player.黑;
                            else
                                CurrentPlayer = Player.红;
                        }
                        if (isStart)
                        {
                            //设置落子位置的棋子值为拾起的棋子值
                            ChessBoard[row, col] = _pickChess;
                            //清空_pickChess值
                            _pickChess = Piece.无子;
                            //保存落子位置的行号和列号
                            _dropRow = row;
                            _dropCol = col;
                            Player LastPlayer = Player.无;

                            if (CurrentPlayer == Player.红)
                                LastPlayer = Player.黑;
                            else if (CurrentPlayer == Player.黑)
                                LastPlayer = Player.红;

                            game_info.AppendText(Convert.ToString(LastPlayer) + "方将棋子" + Convert.ToString(ChessBoard[row, col]) +
                                "从(" + Convert.ToString(_pickRow) + ", " + Convert.ToString(_pickCol) + ")移动到(" +
                                Convert.ToString(_dropRow) + ", " + Convert.ToString(_dropCol) + ")。\n");
                            isDropped = true;
                        }
                        //强制刷新窗口
                        Invalidate();
                    }
                }
            }
        }
        private void CanDrop(int _pickRow, int row, int col, out bool canDrop)
        {
            canDrop = false;
            //根据拾起的棋子值，进行相应的走棋规则判定，如果通过，则canDrop为true，可以落子
            //象的走棋规则
            if (_pickChess == Piece.黑象)
            {
                //如果走田字、不绊象脚且不过河，则可以落子
                if (Math.Abs(_pickRow - row) == 2 && Math.Abs(_pickCol - col) == 2 && ChessBoard[(_pickRow + row) / 2, (_pickCol + col) / 2] == Piece.无子 && row <= 5)
                {
                    canDrop = true;
                }
            }
            if (_pickChess == Piece.红相)
            {
                //如果走田字、不绊象脚且不过河，则可以落子
                if (Math.Abs(_pickRow - row) == 2 && Math.Abs(_pickCol - col) == 2 && ChessBoard[(_pickRow + row) / 2, (_pickCol + col) / 2] == Piece.无子 && row >= 6)
                {
                    canDrop = true;
                }
            }

            //马的走棋规则
            if (_pickChess == Piece.红马 || _pickChess == Piece.黑马)
            {
                //如果横着走日字，且不绊马脚
                if (Math.Abs(_pickRow - row) == 1 && Math.Abs(_pickCol - col) == 2 && ChessBoard[_pickRow, (_pickCol + col) / 2] == Piece.无子)
                    canDrop = true;

                //如果竖着走日字，且不绊马脚
                if (Math.Abs(_pickRow - row) == 2 && Math.Abs(_pickCol - col) == 1 && ChessBoard[(_pickRow + row) / 2, _pickCol] == Piece.无子)
                    canDrop = true;
            }

            //车的走棋规则
            if (_pickChess == Piece.红车 || _pickChess == Piece.黑车)
            {
                //如果车横着走
                if (_pickRow == row)
                {
                    //比较起点和落点列的大小
                    int max = col > _pickCol ? col : _pickCol;
                    int min = col > _pickCol ? _pickCol : col;

                    //统计移动路径上（即起点列和落点列之间）棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard[row, i] != Piece.无子)
                            chessNum++;
                    //只有当移动路径上棋子数量为0时才可以落子
                    if (chessNum == 0)
                        canDrop = true;
                }
                //如果车竖着走
                if (_pickCol == col)
                {
                    //比较起点和落点行的大小
                    int max = row > _pickRow ? row : _pickRow;
                    int min = row > _pickRow ? _pickRow : row;

                    //统计移动路径上（即起点列和落点行之间）棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard[i, col] != Piece.无子)
                            chessNum++;
                    //只有当移动路径上棋子数量为0时才可以落子
                    if (chessNum == 0)
                        canDrop = true;
                }
            }

            //炮的走棋规则制定 
            if (_pickChess == Piece.红炮 || _pickChess == Piece.黑炮)
            {

                //如果炮横着走
                if (_pickRow == row)
                {
                    //比较起点列和落点列的大小
                    int max = col > _pickCol ? col : _pickCol;
                    int min = col > _pickCol ? _pickCol : col;

                    //统计移动路径上的棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard[row, i] != Piece.无子)
                            chessNum++;

                    //只有当移动路径上棋子数量为0且落子位置没有棋子，或者数量为1且落子位置有棋子，或者才允许落子
                    if ((chessNum == 0 && ChessBoard[row, col] == Piece.无子) || (chessNum == 1 && ChessBoard[row, col] != Piece.无子))
                        canDrop = true;
                }
                //如果炮竖着走
                if (_pickCol == col)
                {
                    //比较起点列和落点列的大小
                    int max = row > _pickRow ? row : _pickRow;
                    int min = row > _pickRow ? _pickRow : row;

                    //统计移动路径上的棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard[i, col] != Piece.无子)
                            chessNum++;

                    //只有当移动路径上棋子数量为0且落子位置没有棋子，或者数量为1且落子位置有棋子，或者才允许落子
                    if ((chessNum == 0 && ChessBoard[row, col] == Piece.无子) || (chessNum == 1 && ChessBoard[row, col] != Piece.无子))
                        canDrop = true;
                }
            }

            //兵的走棋规则制定
            if (_pickChess == Piece.黑卒)
            {
                //如果兵没过河
                if (_pickRow <= 5 && row - _pickRow == 1 && col == _pickCol)
                    canDrop = true;


                //如果兵过河了
                if (_pickRow >= 6 && Math.Abs(_pickRow - row) + Math.Abs(_pickCol - col) == 1 && row >= _pickRow)
                    canDrop = true;

            }
            if (_pickChess == Piece.红兵)
            {
                //如果兵没过河
                if (_pickRow >= 6 && _pickRow - row == 1 && col == _pickCol)
                    canDrop = true;

                //如果兵过河了 
                if (_pickRow <= 5 && Math.Abs(_pickRow - row) + Math.Abs(_pickCol - col) == 1 && row <= _pickRow)
                    canDrop = true;
            }

            //士的走棋规则
            //蓝士的走棋规则
            if (_pickChess == Piece.黑士)
            {
                if (row <= 3 && row >= 1 && col <= 6 && col >= 4 && Math.Abs(_pickRow - row) == 1 && Math.Abs(_pickCol - col) == 1)
                    canDrop = true;
            }
            //红士的走棋规则
            if (_pickChess == Piece.红士)
            {
                if (row <= 10 && row >= 8 && col <= 6 && col >= 4 && Math.Abs(_pickRow - row) == 1 && Math.Abs(_pickCol - col) == 1)
                    canDrop = true;
            }

            //将的走棋规则
            //蓝将的走棋规则
            if (_pickChess == Piece.黑将)
            {
                if (row <= 3 && row >= 1 && col <= 6 && col >= 4 && Math.Abs(_pickCol - col) + Math.Abs(_pickRow - row) == 1)
                    canDrop = true;
            }
            //红帅的走棋规则
            if (_pickChess == Piece.红帅)
            {
                if (row <= 10 && row >= 8 && col <= 6 && col >= 4 && Math.Abs(_pickCol - col) + Math.Abs(_pickRow - row) == 1)
                    canDrop = true;
            }

            if (row == _pickRow && col == _pickCol)
                canDrop = false;
        }

        private void _CanDrop(Piece _pickChess, int _pickRow, int _pickCol, int row, int col, out bool canDrop)
        {
            canDrop = false;
            //根据拾起的棋子值，进行相应的走棋规则判定，如果通过，则canDrop为true，可以落子
            //象的走棋规则
            if (_pickChess == Piece.黑象)
            {
                //如果走田字、不绊象脚且不过河，则可以落子
                if (Math.Abs(_pickRow - row) == 2 && Math.Abs(_pickCol - col) == 2 && ChessBoard_temp[(_pickRow + row) / 2, (_pickCol + col) / 2] == Piece.无子 && row <= 5)
                {
                    canDrop = true;
                }
            }
            if (_pickChess == Piece.红相)
            {
                //如果走田字、不绊象脚且不过河，则可以落子
                if (Math.Abs(_pickRow - row) == 2 && Math.Abs(_pickCol - col) == 2 && ChessBoard_temp[(_pickRow + row) / 2, (_pickCol + col) / 2] == Piece.无子 && row >= 6)
                {
                    canDrop = true;
                }
            }

            //马的走棋规则
            if (_pickChess == Piece.红马 || _pickChess == Piece.黑马)
            {
                //如果横着走日字，且不绊马脚
                if (Math.Abs(_pickRow - row) == 1 && Math.Abs(_pickCol - col) == 2 && ChessBoard_temp[_pickRow, (_pickCol + col) / 2] == Piece.无子)
                    canDrop = true;

                //如果竖着走日字，且不绊马脚
                if (Math.Abs(_pickRow - row) == 2 && Math.Abs(_pickCol - col) == 1 && ChessBoard_temp[(_pickRow + row) / 2, _pickCol] == Piece.无子)
                    canDrop = true;
            }

            //车的走棋规则
            if (_pickChess == Piece.红车 || _pickChess == Piece.黑车)
            {
                //如果车横着走
                if (_pickRow == row)
                {
                    //比较起点和落点列的大小
                    int max = col > _pickCol ? col : _pickCol;
                    int min = col > _pickCol ? _pickCol : col;

                    //统计移动路径上（即起点列和落点列之间）棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard_temp[row, i] != Piece.无子)
                            chessNum++;
                    //只有当移动路径上棋子数量为0时才可以落子
                    if (chessNum == 0)
                        canDrop = true;
                }
                //如果车竖着走
                if (_pickCol == col)
                {
                    //比较起点和落点行的大小
                    int max = row > _pickRow ? row : _pickRow;
                    int min = row > _pickRow ? _pickRow : row;

                    //统计移动路径上（即起点列和落点行之间）棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard_temp[i, col] != Piece.无子)
                            chessNum++;
                    //只有当移动路径上棋子数量为0时才可以落子
                    if (chessNum == 0)
                        canDrop = true;
                }
            }

            //炮的走棋规则制定 
            if (_pickChess == Piece.红炮 || _pickChess == Piece.黑炮)
            {

                //如果炮横着走
                if (_pickRow == row)
                {
                    //比较起点列和落点列的大小
                    int max = col > _pickCol ? col : _pickCol;
                    int min = col > _pickCol ? _pickCol : col;

                    //统计移动路径上的棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard_temp[row, i] != Piece.无子)
                            chessNum++;

                    //只有当移动路径上棋子数量为0且落子位置没有棋子，或者数量为1且落子位置有棋子，或者才允许落子
                    if ((chessNum == 0 && ChessBoard_temp[row, col] == Piece.无子) || (chessNum == 1 && ChessBoard_temp[row, col] != Piece.无子))
                        canDrop = true;
                }
                //如果炮竖着走
                if (_pickCol == col)
                {
                    //比较起点列和落点列的大小
                    int max = row > _pickRow ? row : _pickRow;
                    int min = row > _pickRow ? _pickRow : row;

                    //统计移动路径上的棋子的数量
                    int chessNum = 0;
                    for (int i = min + 1; i <= max - 1; i++)
                        if (ChessBoard_temp[i, col] != Piece.无子)
                            chessNum++;

                    //只有当移动路径上棋子数量为0且落子位置没有棋子，或者数量为1且落子位置有棋子，或者才允许落子
                    if ((chessNum == 0 && ChessBoard_temp[row, col] == Piece.无子) || (chessNum == 1 && ChessBoard_temp[row, col] != Piece.无子))
                        canDrop = true;
                }
            }

            //兵的走棋规则制定
            if (_pickChess == Piece.黑卒)
            {
                //如果兵没过河
                if (_pickRow <= 5 && row - _pickRow == 1 && col == _pickCol)
                    canDrop = true;


                //如果兵过河了
                if (_pickRow >= 6 && Math.Abs(_pickRow - row) + Math.Abs(_pickCol - col) == 1 && row >= _pickRow)
                    canDrop = true;

            }
            if (_pickChess == Piece.红兵)
            {
                //如果兵没过河
                if (_pickRow >= 6 && _pickRow - row == 1 && col == _pickCol)
                    canDrop = true;

                //如果兵过河了 
                if (_pickRow <= 5 && Math.Abs(_pickRow - row) + Math.Abs(_pickCol - col) == 1 && row <= _pickRow)
                    canDrop = true;
            }

            //士的走棋规则
            //蓝士的走棋规则
            if (_pickChess == Piece.黑士)
            {
                if (row <= 3 && row >= 1 && col <= 6 && col >= 4 && Math.Abs(_pickRow - row) == 1 && Math.Abs(_pickCol - col) == 1)
                    canDrop = true;
            }
            //红士的走棋规则
            if (_pickChess == Piece.红士)
            {
                if (row <= 10 && row >= 8 && col <= 6 && col >= 4 && Math.Abs(_pickRow - row) == 1 && Math.Abs(_pickCol - col) == 1)
                    canDrop = true;
            }

            //将的走棋规则
            //蓝将的走棋规则
            if (_pickChess == Piece.黑将)
            {
                if (row <= 3 && row >= 1 && col <= 6 && col >= 4 && Math.Abs(_pickCol - col) + Math.Abs(_pickRow - row) == 1)
                    canDrop = true;
            }
            //红帅的走棋规则
            if (_pickChess == Piece.红帅)
            {
                if (row <= 10 && row >= 8 && col <= 6 && col >= 4 && Math.Abs(_pickCol - col) + Math.Abs(_pickRow - row) == 1)
                    canDrop = true;
            }

            if (row == _pickRow && col == _pickCol)
                canDrop = false;

        }


        private void MainWnd_MouseDown(object sender, MouseEventArgs e)
        {
            if (mode != 0 && CurrentPlayer != player)
                return;
            //如果是左键下压，则拾起或落下棋子
            if (e.Button == MouseButtons.Left)
            {
                //把鼠标点击位置坐标转换为行号和列号
                int row, col;
                bool valid = ConvertPointToRowCol(new Point(e.X, e.Y), out row, out col);

                //判断鼠标点击是否有效
                if (valid == true)
                {
                    isStart = true;
                    //处理拾起棋子的动作(判断拾起的棋子是否为“无子”，“无子”表示目前没有拾起棋子)
                    GameOn(row, col);
                    if (mode == 1)
                        LAN_connection.Lan_connection.SendMsg_Operation(4, 0, "落子+" + Convert.ToString(10 * row + col));
                    if (mode == 2 && isDropped)
                        AI_motion();
                }
            }
            //如果是鼠标右键下压，则取消拾起的棋子
            else if (e.Button == MouseButtons.Right)
            {
                //判断是否已经拾子
                if (_pickChess != Piece.无子)
                {
                    //把拾子位置设置为拾起棋子的值
                    ChessBoard[_pickRow, _pickCol] = _pickChess;

                    //清空拾起的棋子值、拾子行号、拾子列号
                    _pickChess = Piece.无子;
                    _pickRow = 0;
                    _pickCol = 0;

                    if (mode == 1)
                        LAN_connection.Lan_connection.SendMsg_Operation(4, 0, "撤销+0");

                    isDropped = false;
                    //强制刷新
                    Invalidate();
                }
            }
        }
        private void AI_motion()
        {
            if (!isStart && HumanFirst && isRestored)
                return;
            if (!isStart && !HumanFirst)
            {
                isStart = true;
                CurrentPlayer = Player.红;
                GameOn(10, 8);
                GameOn(8, 7);
                return;
            }
            //计算出来后直接调用GameOn函数
            isCopied = false;
            Step step = GenerateOneMove();
            //进行维护
            GameOn(step._PickRow, step._PickCol);
            GameOn(step._DropRow, step._DropCol);
        }
        private void FakeMove(Step step)
        {
            //然后模拟
            ChessBoard_temp[step._DropRow, step._DropCol] = ChessBoard_temp[step._PickRow, step._PickCol];
            ChessBoard_temp[step._PickRow, step._PickCol] = Piece.无子;
        }

        private void UnFakeMove(Step step)
        {
            ChessBoard_temp[step._PickRow, step._PickCol] = step._PickChess;
            ChessBoard_temp[step._DropRow, step._DropCol] = step._DropChess;
        }

        private List<Step> GetAllPossibleSteps()
        {
            if (!isCopied)
            {
                //先复制棋盘
                for (int i = 1; i <= 10; i++)
                    for (int j = 1; j <= 9; j++)
                        ChessBoard_temp[i, j] = ChessBoard[i, j];
                isCopied = true;
            }
            List<Step> steps = new List<Step>();
            for (int i = 1; i <= 10; i++)
                for (int j = 1; j <= 9; j++)
                {
                    //保证是己方棋子
                    if (ChessBoard_temp[i, j] != Piece.无子 && ChessBoard_temp[i, j].ToString().IndexOf(player.ToString()) == -1)
                    {
                        //搜索落点
                        for (int k = 1; k <= 10; k++)
                            for (int m = 1; m <= 9; m++)
                            {
                                _CanDrop(ChessBoard_temp[i, j], i, j, k, m, out bool canDrop);
                                if (canDrop && 
                                    (ChessBoard_temp[k, m] == Piece.无子 || 
                                    ChessBoard_temp[k, m].ToString().IndexOf(player.ToString()) == 0))
                                {
                                    Step step = new Step();

                                    if (player == Player.红)
                                        step._Player = Player.黑;
                                    else if (player == Player.黑)
                                        step._Player = Player.红;

                                    step._PickChess = ChessBoard_temp[i, j];
                                    step._PickRow = i;
                                    step._PickCol = j;
                                    step._DropChess = ChessBoard_temp[k, m];
                                    step._DropRow = k;
                                    step._DropCol = m;
                                    steps.Add(step);
                                }
                            }
                    }

                }
            return steps;
        }

        private Step GenerateOneMove(int level = 2)
        {
            int maxScore = -10000;
            Step step = new Step();

            //获取所有可能的路径
            List<Step> steps = GetAllPossibleSteps();

            foreach(Step tmp in steps)
            {
                FakeMove(tmp);
                int score = GetMinValue(tmp, level - 1, maxScore);
                UnFakeMove(tmp);
                if (score > maxScore)
                {
                    maxScore = score;
                    step = tmp;
                }
            }
            return step;
        }

        private int GetMinValue(Step step, int level, int maxScore)
        {
            if (level == 0)
                return GetScore(step);
            int minScore = 10000;

            //获取所有可能的路径
            List<Step> steps = GetAllPossibleSteps();

            foreach (Step tmp in steps)
            {
                FakeMove(tmp);
                int score = GetMaxValue(tmp, level - 1, minScore);
                UnFakeMove(tmp);
                if (score <= maxScore)
                {
                    minScore = score;
                    return minScore;
                }
                if (score < minScore)
                    minScore = score;
            }
            return minScore;
        }

        private int GetMaxValue(Step step, int level, int minScore)
        {
            if (level == 0)
                return GetScore(step);
            int maxScore = -10000;

            //获取所有可能的路径
            List<Step> steps = GetAllPossibleSteps();

            foreach (Step tmp in steps)
            {
                FakeMove(tmp);
                int score = GetMinValue(tmp, level - 1, maxScore);
                UnFakeMove(tmp);
                if (score >= minScore)
                {
                    maxScore = score;
                    break;
                }

                if (score > maxScore)
                    maxScore = score;
            }
            return maxScore;
        }

        private int GetScore(Step step)
        {
            switch(step._PickChess)
            {
                case Piece.红兵: return _score[0];
                case Piece.红士: return _score[1];
                case Piece.红帅: return _score[2];
                case Piece.红炮: return _score[3];
                case Piece.红相: return _score[4];
                case Piece.红车: return _score[5];
                case Piece.红马: return _score[6];
                case Piece.黑卒: return _score[0];
                case Piece.黑士: return _score[1];
                case Piece.黑将: return _score[2];
                case Piece.黑炮: return _score[3];
                case Piece.黑象: return _score[4];
                case Piece.黑车: return _score[5];
                case Piece.黑马: return _score[6];
                default: return 0;
            }
        }

        private void common_sentences_SelectedIndexChanged(object sender, EventArgs e)
        {
            string content = (string)common_sentences.SelectedItem;
            LAN_connection.Lan_connection.SendMsg_Operation(1, 0, player.ToString() + "方：\n" + content + "\n");
            chat_info.Text += player.ToString() + "方：\n" + content + "\n";
            common_sentences.Visible = false;
        }

        private void game_info_TextChanged(object sender, EventArgs e)
        {
            game_info.ScrollToCaret();
        }

        private void chat_info_TextChanged(object sender, EventArgs e)
        {
            chat_info.ScrollToCaret();
        }
    }


    public class Step
    {
        private Player _player;

        public Player _Player
        {
            get { return _player; }
            set
            {

                if (value == Player.无)
                    throw new ArgumentOutOfRangeException("走棋方不能为无。");
                else
                    _player = value;

            }
        }
        private Piece _pickChess;

        public Piece _PickChess
        {
            get { return _pickChess; }
            set
            {
                if (value == Piece.无子)
                    throw new ArgumentOutOfRangeException("拾起的棋子不能为无子。");
                else
                    _pickChess = value;
            }
        }
        private int _pickRow;

        public int _PickRow
        {
            get { return _pickRow; }
            set
            {
                if (value < 1 || value > 10)
                    throw new ArgumentOutOfRangeException("行号必须在1和10之间。");
                else
                    _pickRow = value;
            }
        }
        private int _pickCol;

        public int _PickCol
        {
            get { return _pickCol; }
            set
            {
                if (value < 1 || value > 9)
                    throw new ArgumentOutOfRangeException("列号号必须在1和9之间。");
                else
                    _pickCol = value;
            }
        }
        private Piece _dropChess;
        public Piece _DropChess
        {
            get { return _dropChess; }
            set { _dropChess = value; }
        }
        private int _dropRow;

        public int _DropRow
        {
            get { return _dropRow; }
            set
            {
                if (value < 1 || value > 10)
                    throw new ArgumentOutOfRangeException("行号必须在1和10之间。");
                else
                    _dropRow = value;
            }
        }
        private int _dropCol;
        public int _DropCol
        {
            get { return _dropCol; }
            set
            {
                if (value < 1 || value > 9)
                    throw new ArgumentOutOfRangeException("列号号必须在1和9之间。");
                else
                    _dropCol = value;
            }
        }
    }
}
