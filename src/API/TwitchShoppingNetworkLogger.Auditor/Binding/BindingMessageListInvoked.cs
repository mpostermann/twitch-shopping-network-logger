using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using TwitchShoppingNetworkLogger.Auditor.Impl;

namespace TwitchShoppingNetworkLogger.Auditor.Binding
{
    public class BindingMessageListInvoked : INotifyPropertyChanged
    {
        private ISynchronizeInvoke _invoke;
        private StringBuilder _concatenatedMessagesBuilder;
        private readonly List<ListWhisperModel> _whispers;

        public BindingMessageListInvoked(ISynchronizeInvoke invoke)
        {
            _invoke = invoke;
            _concatenatedMessagesBuilder = new StringBuilder();
            _whispers = new List<ListWhisperModel>();
        }
        
        private string _concatenatedMessages;
        public string ConcatenatedMessages {
            get {
                return _concatenatedMessages;
            }

            set {
                if (value != _concatenatedMessages)
                {
                    _concatenatedMessages = value;
                    NotifyPropertyChanged(nameof(ConcatenatedMessages));
                }
            }
        }

        public List<ListWhisperModel> Whispers
        {
            get { return _whispers; }
        }

        public void Add(ListWhisperModel model)
        {
            Whispers.Add(model);

            _concatenatedMessagesBuilder.AppendLine(model.TimeReceived.ToString(CultureInfo.InvariantCulture));
            _concatenatedMessagesBuilder.AppendLine(model.Message);
            ConcatenatedMessages = _concatenatedMessagesBuilder.ToString();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if ((_invoke != null) && (_invoke.InvokeRequired))
            {
                IAsyncResult ar = _invoke.BeginInvoke(PropertyChanged, new object[] {this, new PropertyChangedEventArgs(propertyName) });
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
