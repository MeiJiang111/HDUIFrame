using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Async.UIFramework
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class UIBase : MonoBehaviour
    {
        /// <summary>
        /// UI�Ƿ��Զ�����
        /// </summary>
        public bool AutoDestroy = true;

        /// <summary>
        /// һ�����ڵ�
        /// </summary>
        public UIBase Parent;

        /// <summary>
        /// һ���ӽڵ�
        /// </summary>
        public List<UIBase> Children = new List<UIBase>();

        protected internal UniTask InnerOnCreate() => OnCreate();
        protected internal UniTask InnerOnRefresh() => OnRefresh();
        protected internal void InnerOnBind() => OnBind();
        protected internal void InnerOnUnbind() => OnUnbind();
        protected internal void InnerOnShow() => OnShow();
        protected internal void InnerOnHide() => OnHide();
        protected internal void InnerOnDied() => OnDied();

        private HashSet<UITimer> timers = new HashSet<UITimer>();

        /// <summary>
        /// ����ʱ���ã�����������ִֻ��һ��
        /// </summary>
        protected virtual UniTask OnCreate() => UniTask.CompletedTask;

        /// <summary>
        /// ˢ��ʱ����
        /// </summary>
        protected virtual UniTask OnRefresh() => UniTask.CompletedTask;

        /// <summary>
        /// ���¼�
        /// </summary>
        protected virtual void OnBind() { }

        /// <summary>
        /// ����¼�
        /// </summary>
        protected virtual void OnUnbind() { }

        /// <summary>
        /// ��ʾʱ����
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// ����ʱ����
        /// </summary>
        protected virtual void OnHide() { }

        /// <summary>
        /// ����ʱ���ã�����������ִֻ��һ��
        /// </summary>
        protected virtual void OnDied() { }

        /// <summary>
        /// ������ʱ����gameObject������ʱ���Զ�Cancel��ʱ��
        /// </summary>
        /// <param name="delay">�ӳٶ������ִ��callback</param>
        /// <param name="callback">�ӳ�ִ�еķ���</param>
        /// <param name="isLoop">�Ƿ���ѭ����ʱ��</param>
        protected UITimer CreateTimer(float delay, Action callback, bool isLoop = false)
        {
            var timer = UIFrame.CreateTimer(delay, callback, isLoop);
            timers.Add(timer);
            return timer;
        }

        /// <summary>
        /// ȡ�����ж�ʱ��
        /// </summary>
        public void CancelAllTimer()
        {
            foreach (var item in timers)
            {
                item.Cancel();
            }
            timers.Clear();
        }
    }
}

