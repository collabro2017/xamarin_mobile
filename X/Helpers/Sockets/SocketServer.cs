using System;
using X.Pages;

namespace X
{
    public interface SocketServer
    {
        void ConnectServer(string token, MainPage homeView);
        void Write(string socketValue);
        void Exit();
    }
}