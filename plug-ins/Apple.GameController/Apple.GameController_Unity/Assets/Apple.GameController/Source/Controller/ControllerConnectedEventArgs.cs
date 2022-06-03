using System;

namespace Apple.GameController.Controller
{
    public class ControllerConnectedEventArgs : EventArgs
    {
        public GCController Controller;

        public ControllerConnectedEventArgs(GCController controller)
        {
            Controller = controller;
        }
    }
}