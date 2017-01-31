using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Starcounter;
using Starcounter.Advanced;


namespace Colab.Public
{

    [Utils_json]
    partial class Utils : Page
    {
        protected Dictionary<Json, CallbackData> _callbacks = new Dictionary<Json, CallbackData>();
        public static ConcurrentDictionary<String, String> UtilsHtmls = new ConcurrentDictionary<String, String>();
        public static ConcurrentDictionary<String, Master> Masters = new ConcurrentDictionary<String, Master>();

        
        public void ShowUserMessage(ModalMessageType type, string message, Action<ModalResult> callback = null)
        {
            UserMessage.Message = message;
            UserMessage.Type = DisplayValueEnum.GetDescriptionValue(type);
            UserMessage.Show = true;

            if (_callbacks.ContainsKey(UserMessage))
            {
                _callbacks.Remove(UserMessage);
            }

            if (callback != null)
            {
                _callbacks.Add(UserMessage, new CallbackData()
                {
                    Action = callback,
                    Transaction = Starcounter.Transaction.Current
                });
            }
        }

        [Utils_json.UserMessage]
        partial class UserMessageJson: Json
        {
            void Handle(Input.Show input)
            {
                if (!input.Value) //Messagebox closing
                {
                    var utils = this.Parent as Utils;
                    if (utils._callbacks.ContainsKey(this))
                    {
                        try
                        {
                            var data = utils._callbacks[this];
                            data.Transaction.Scope(() =>
                            {
                                data.Action.Invoke(new ModalResult() { Value = ModalResultType.OK });
                            });
                            utils._callbacks.Remove(this);
                        }
                        catch (Exception e)
                        {
                            Master.SendUserError("User message callback error: " + e.Message);
                        }
                    }
                }
            }

        }

        void Handle(Input.Ping input)
        {
            input.Cancel();
        }

        public T GetPersistentApp<T>() where T : Json
        {
            return PersistantApp as T;
        }

        public T AddModal<T>(T modal) where T : Json
        {
            return AddModal(modal, null) as T;
        }

        public Json AddModal(Json modal, Action<ModalResult> callback)
        {
            if (modal != null)
            {
                if (callback != null)
                {
                    _callbacks.Add(modal, new CallbackData()
                    {
                        Action = callback,
                        Transaction = Starcounter.Transaction.Current
                    });
                }
                modal.Transaction.Scope(() =>
                {
                    Modals.Add(modal);
                });
            }
            return modal;
        }

        public T GetModal<T>() where T : Json
        {
            foreach (Json modal in Modals)
            {
                if (modal is T)
                    return modal as T;
            }
            return null;
        }

        public void RemoveModal<T>() where T : Json
        {
            var modal = GetModal<T>();
            if (modal != null)
                RemoveModal(modal);
        }

        public void RemoveModal(Json modal, ModalResult result)
        {
            result.Modal = modal;
            if (Modals.Contains(modal))
            {
                Modals.Remove(modal);
            }
            if (_callbacks.ContainsKey(modal))
            {
                try
                {
                    var data = _callbacks[modal];
                    data.Transaction.Scope(() =>
                    {
                        data.Action.Invoke(result);
                    });
                    _callbacks.Remove(modal);
                }
                catch (Exception e)
                {
                    Master.SendUserError("Ooooops: " + e.Message);
                }
            }
        }


        public void RemoveModalRecursive(Json modal)
        {
            RemoveModal(modal);
            if (modal.Parent != null)
            {
                RemoveModalRecursive(modal.Parent);
            }
        }

        public void RemoveModal(Json modal)
        {
            RemoveModal(modal, new ModalResult() { Value = ModalResultType.NORESULT });
        }

        public void RemoveModal(Json modal, ModalResultType type)
        {
            RemoveModal(modal, new ModalResult() { Value = type, Data = modal.Data });
        }

        public void SendUserInfo(string message)
        {
            ShowUserMessage(ModalMessageType.INFO, message);
        }

        public void SendUserWarning(string message)
        {
            ShowUserMessage(ModalMessageType.WARNING, message);
        }

        public void SendUserError(string message)
        {
            ShowUserMessage(ModalMessageType.ERROR, message);
        }

        public void SendUserError(Exception e)
        {
            SendUserError(e.Message);
        }

        public void SendCommand(ColabCommand command, params string[] pars)
        {
            string fullcmd = Enum<ColabCommand>.GetName(command).ToLower();
            fullcmd += (pars.Length > 0) ? "{p}" : "";
            foreach (string par in pars)
            {
                fullcmd += par + ";";
            }
            Command = fullcmd;
        }
    }
}
