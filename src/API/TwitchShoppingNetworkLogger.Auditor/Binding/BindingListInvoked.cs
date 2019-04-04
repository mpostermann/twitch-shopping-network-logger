using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TwitchShoppingNetworkLogger.Auditor.Binding
{
    public class BindingListInvoked<T> : BindingList<T>
    {
        private ISynchronizeInvoke _invoke;
        delegate void ListChangedDelegate(ListChangedEventArgs e);

        public BindingListInvoked(ISynchronizeInvoke invoke)
        {
            _invoke = invoke;
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if ((_invoke != null) && (_invoke.InvokeRequired))
            {
                IAsyncResult ar = _invoke.BeginInvoke(new ListChangedDelegate(base.OnListChanged), new object[] { e });
            }
            else
            {
                base.OnListChanged(e);
            }
        }

        public IList<T> DataSource {
            get {
                return this;
            }
            set {
                if (value != null)
                {
                    this.ClearItems();
                    RaiseListChangedEvents = false;

                    foreach (T item in value)
                    {
                        this.Add(item);
                    }
                    RaiseListChangedEvents = true;
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }
    }
}
