using Kashkeshet.LogicBll;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Kashkeshet.ConsoleUI
{
    public class ChatScreen
    {
        public readonly Guid Id;
        protected ServerCommunicator _communicator;
        protected ChatUpdater _updater;
        protected Process _windowProcess;
        protected StreamWriter _writer;
        protected StreamReader _reader;

        public ChatScreen(Guid chatId, ServerCommunicator communicator, ChatUpdater updater)
        {
            Id = chatId;
            _communicator = communicator;
            _updater = updater;            
        }

        public void Start()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = $"Chat ID {Id}"
            };

            _windowProcess = Process.Start(processStartInfo);
            _writer = _windowProcess.StandardInput;
            _reader = _windowProcess.StandardOutput;
        }
    }
}
