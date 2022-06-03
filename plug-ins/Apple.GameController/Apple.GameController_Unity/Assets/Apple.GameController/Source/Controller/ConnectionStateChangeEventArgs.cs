using System;

namespace Apple.GameController.Controller
{
    public class ConnectionStateChangeEventArgs : EventArgs
    {
        public bool IsConnected;

        public ConnectionStateChangeEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}