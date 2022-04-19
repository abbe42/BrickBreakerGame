using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// observable collections
using System.Collections.ObjectModel;

// debug output
using System.Diagnostics;

// timer, sleep
using System.Threading;

// Game Timer
using System.Windows.Threading;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

// Rectangle
// Must update References manually
using System.Drawing;

// INotifyPropertyChanged
using System.ComponentModel;

namespace BrickBreaker
{
    public partial class Model : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private Thread paddleThread = null;
        private Thread ballThread = null;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static UInt32 _numBalls = 1;
        private UInt32[] _buttonPresses = new UInt32[_numBalls];
        Random _randomNumber = new Random();


        private double _ballXMove = 1;
        private double _ballYMove = 1;
        System.Drawing.Rectangle _ballRectangle;
        System.Drawing.Rectangle _paddleRectangle;
        bool _movepaddleLeft = false;
        bool _movepaddleRight = false;
        private bool _moveBall = false;
        public bool MoveBall
        {
            get { return _moveBall; }
            set { _moveBall = value; }
        }

        private double _windowHeight = 100;
        public double WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; }
        }

        private double _windowWidth = 100;
        public double WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; }
        }

  
        public Model()
        {
        }

        public void InitModel()
        {

            if (paddleThread == null)
            {
                paddleThread = new Thread(new ThreadStart(PaddleThread));
                paddleThread.Start();
            }

            if (ballThread == null)
            {
                ballThread = new Thread(new ThreadStart(BallThread));
                ballThread.Start();
            }
        }

    

        public void CleanUp()
        {

        }

        public void SetStartPosition()
        {

            BallHeight = 50;
            BallWidth = 50;
            paddleWidth = 120;
            paddleHeight = 10;

            ballCanvasLeft = _windowWidth / 2 - BallWidth / 2;
            ballCanvasTop = _windowHeight / 5;

            _moveBall = false;

            paddleCanvasLeft = _windowWidth / 2 - paddleWidth / 2;
            paddleCanvasTop = _windowHeight - paddleHeight;
            _paddleRectangle = new System.Drawing.Rectangle((int)paddleCanvasLeft, (int)paddleCanvasTop, (int)paddleWidth, (int)paddleHeight);
        }

        

            public void MoveLeft(bool move)
        {
            _movepaddleLeft = move;
        }

        public void MoveRight(bool move)
        {
            _movepaddleRight = move;
        }

        void PaddleThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(5);
                    if (_movepaddleLeft && paddleCanvasLeft > 0)
                        paddleCanvasLeft -= 2;
                    else if (_movepaddleRight && paddleCanvasLeft < _windowWidth - paddleWidth)
                        paddleCanvasLeft += 2;

                    _paddleRectangle = new System.Drawing.Rectangle((int)paddleCanvasLeft, (int)paddleCanvasTop, (int)paddleWidth, (int)paddleHeight);
                }
            }
            catch (ThreadAbortException) { }
        }

        void BallThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(5);

                    ballCanvasLeft += _ballXMove;
                    ballCanvasTop += _ballYMove;

                    // check to see if ball has it the left or right side of the drawing element
                    if ((ballCanvasLeft + BallWidth >= _windowWidth) ||
                        (ballCanvasLeft <= 0))
                        _ballXMove = -_ballXMove;


                    // check to see if ball has it the top of the drawing element
                    if (ballCanvasTop <= 0)
                        _ballYMove = -_ballYMove;

                    if (ballCanvasTop + BallWidth >= _windowHeight)
                    {
                        // we hit bottom. stop moving the ball
                        _moveBall = false;
                    }

                    // see if we hit the paddle
                    _ballRectangle = new System.Drawing.Rectangle((int)ballCanvasLeft, (int)ballCanvasTop, (int)BallWidth, (int)BallHeight);
                    if (_ballRectangle.IntersectsWith(_paddleRectangle))
                    {
                        // hit paddle. reverse direction in Y direction
                        _ballYMove = -_ballYMove;

                        // move the ball away from the paddle so we don't intersect next time around and
                        // get stick in a loop where the ball is bouncing repeatedly on the paddle
                        ballCanvasTop += 2 * _ballYMove;

                        // add move the ball in X some small random value so that ball is not traveling in the same 
                        // pattern
                        ballCanvasLeft += _randomNumber.Next(5);
                    }
                }
            }
            catch (ThreadAbortException) { }
        }
    }
}
