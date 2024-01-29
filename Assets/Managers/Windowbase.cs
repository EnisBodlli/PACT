using System;
using System.Collections;
using System.Collections.Generic;
using Pact.WindowManager.ScriptableObjects.ScriptableEnums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Pact.WindowManager.Windows
{
    public interface IWindow
    {
        void Open(Transform parent);
        void SetOrderToTop();
        void Close();
        void CloseInternal();
    }
    public class WindowBase : MonoBehaviour, IWindow
    {

        enum WindowState
        {
            Opened,
            Closed,
            Opening,
            Closing,
            None,
        }

        private Action _onCloseAnimationComplete;
        [SerializeField] public WindowTypeEnumSO type;
        [SerializeField] private GameObject firstSelected;

        [SerializeField] private UnityEvent onOpening;
        [SerializeField] private UnityEvent onClosing;
        [SerializeField] private UnityEvent onClosed;

        private WindowState _state = WindowState.None;
        public virtual void Open(Transform parent)
        {
            _state = WindowState.Opening;

            SetOrderToTop();
            gameObject.SetActive(true);

            OnWindowOpened();


            onOpening.Invoke();
        }
        public void OnWindowOpened()
        {
            _state = WindowState.Opened;
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
        public virtual bool CanClose()
        {
            return _state == WindowState.Opened || _state == WindowState.Opening;
        }
        public virtual void Close()
        {
            if (!CanClose())
            {
                return;
            }

            _state = WindowState.Closing;


            CloseInternal();


            onClosing.Invoke();
        }
        public virtual void CloseInternal()
        {
            onClosed.Invoke();
            Hide();

            if (_onCloseAnimationComplete != null)
            {
                _onCloseAnimationComplete.Invoke();
                _onCloseAnimationComplete = null;
            }
        }
        public void Hide()
        {
            _state = WindowState.Closed;
            gameObject.SetActive(false);
        }



        public void Open()
        {
            throw new System.NotImplementedException();
        }

        public void SetOrderToTop()
        {
            transform.SetAsLastSibling();
        }


    }

}


