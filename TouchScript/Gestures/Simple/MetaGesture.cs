﻿/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TouchScript.Gestures.Simple
{
    /// <summary>
    /// Converts touchpoint events for target object into separate events to be used somewhere else.
    /// </summary>
    [AddComponentMenu("TouchScript/Gestures/Meta Gesture")]
    public sealed class MetaGesture : Gesture
    {
        #region Constants

        public const string TOUCH_BEGAN_MESSAGE = "OnTouchBegan";
        public const string TOUCH_MOVED_MESSAGE = "OnTouchMoved";
        public const string TOUCH_ENDED_MESSAGE = "OnTouchEnded";
        public const string TOUCH_CANCELLED_MESSAGE = "OnTouchCancelled";

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a touch point is added.
        /// </summary>
        public event EventHandler<MetaGestureEventArgs> TouchBegan
        {
            add { touchBeganInvoker += value; }
            remove { touchBeganInvoker -= value; }
        }

        /// <summary>
        /// Occurs when a touch point is updated.
        /// </summary>
        public event EventHandler<MetaGestureEventArgs> TouchMoved
        {
            add { touchMovedInvoker += value; }
            remove { touchMovedInvoker -= value; }
        }

        /// <summary>
        /// Occurs when a touch point is removed.
        /// </summary>
        public event EventHandler<MetaGestureEventArgs> TouchEnded
        {
            add { touchEndedInvoker += value; }
            remove { touchEndedInvoker -= value; }
        }

        /// <summary>
        /// Occurs when a touch point is cancelled.
        /// </summary>
        public event EventHandler<MetaGestureEventArgs> TouchCancelled
        {
            add { touchCancelledInvoker += value; }
            remove { touchCancelledInvoker -= value; }
        }

        // iOS Events AOT hack
        private EventHandler<MetaGestureEventArgs> touchBeganInvoker, touchMovedInvoker,
            touchEndedInvoker, touchCancelledInvoker;

        #endregion

        #region Gesture callbacks

        /// <inheritdoc />
        protected override void touchesBegan(IList<ITouch> touches)
        {
            base.touchesBegan(touches);

            if (State == GestureState.Possible) setState(GestureState.Began);

            var length = touches.Count;
            if (touchBeganInvoker != null)
            {
                for (var i = 0; i < length; i++) touchBeganInvoker(this, new MetaGestureEventArgs(touches[i]));
            }
            if (UseSendMessage)
            {
                for (var i = 0; i < length; i++) SendMessageTarget.SendMessage(TOUCH_BEGAN_MESSAGE, touches[i], SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <inheritdoc />
        protected override void touchesMoved(IList<ITouch> touches)
        {
            base.touchesMoved(touches);

            if (State == GestureState.Began || State == GestureState.Changed) setState(GestureState.Changed);

            var length = touches.Count;
            if (touchMovedInvoker != null)
            {
                for (var i = 0; i < length; i++) touchMovedInvoker(this, new MetaGestureEventArgs(touches[i]));
            }
            if (UseSendMessage)
            {
                for (var i = 0; i < length; i++) SendMessageTarget.SendMessage(TOUCH_MOVED_MESSAGE, touches[i], SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <inheritdoc />
        protected override void touchesEnded(IList<ITouch> touches)
        {
            base.touchesEnded(touches);

            if ((State == GestureState.Began || State == GestureState.Changed) && activeTouches.Count == 0) setState(GestureState.Ended);

            var length = touches.Count;
            if (touchEndedInvoker != null)
            {
                for (var i = 0; i < length; i++) touchEndedInvoker(this, new MetaGestureEventArgs(touches[i]));
            }
            if (UseSendMessage)
            {
                for (var i = 0; i < length; i++) SendMessageTarget.SendMessage(TOUCH_ENDED_MESSAGE, touches[i], SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <inheritdoc />
        protected override void touchesCancelled(IList<ITouch> touches)
        {
            base.touchesCancelled(touches);

            if ((State == GestureState.Began || State == GestureState.Changed) && activeTouches.Count == 0) setState(GestureState.Ended);

            var length = touches.Count;
            if (touchCancelledInvoker != null)
            {
                for (var i = 0; i < length; i++) touchCancelledInvoker(this, new MetaGestureEventArgs(touches[i]));
            }
            if (UseSendMessage)
            {
                for (var i = 0; i < length; i++) SendMessageTarget.SendMessage(TOUCH_CANCELLED_MESSAGE, touches[i], SendMessageOptions.DontRequireReceiver);
            }
        }

        #endregion
    }

    /// <summary>
    /// EventArgs for MetaGesture events.
    /// </summary>
    public class MetaGestureEventArgs : EventArgs
    {
        /// <summary>
        /// Current touch point.
        /// </summary>
        public ITouch TouchPoint { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGestureEventArgs"/> class.
        /// </summary>
        /// <param name="touchPoint">Touch point the event is for.</param>
        public MetaGestureEventArgs(ITouch touchPoint)
        {
            TouchPoint = touchPoint;
        }
    }
}