
using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class MessageLog : MonoBehaviour
    {
        [SerializeField] private Text _messageLogText = default;

        // Limit the log to this many lines by removing the older messages.
        // Don't limit the log when LinesToKeep <= 0
        public int LinesToKeep { get; set; } = -1;

        public void AppendMessageToLog(string message)
        {
            string messageLog = _messageLogText.text;
            if (string.IsNullOrEmpty(messageLog))
            {
                messageLog = message;
            }
            else
            {
                // keep the last N messages
                if (LinesToKeep > 0)
                {
                    int numLines, index;
                    for (numLines = 1, index = messageLog.Length; 
                        numLines < LinesToKeep && index > 0; 
                        numLines++, index = messageLog.LastIndexOf('\n', index - 1))
                    {
                    }

                    if (index > 0)
                    {
                        messageLog = messageLog.Substring(index + 1);
                    }
                }

                messageLog += '\n' + message;
            }

            _messageLogText.text = messageLog;
        }

        public void ClearLog()
        {
            _messageLogText.text = string.Empty;
        }
    }
}
