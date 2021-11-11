using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;
using NP.Concepts.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class VisualFlowBehavior
    {

        #region Subscription Attached Avalonia Property
        private static IDisposable GetSubscription(IControl obj)
        {
            return obj.GetValue(SubscriptionProperty);
        }

        private static void SetSubscription(IControl obj, IDisposable value)
        {
            obj.SetValue(SubscriptionProperty, value);
        }

        private static readonly AttachedProperty<IDisposable> SubscriptionProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, IDisposable>
            (
                "Subscription"
            );
        #endregion Subscription Attached Avalonia Property


        #region TheVisualFlow Attached Avalonia Property
        public static VisualFlow GetTheVisualFlow(IControl obj)
        {
            return obj.GetValue(TheVisualFlowProperty);
        }

        public static void SetTheVisualFlow(IControl obj, VisualFlow value)
        {
            obj.SetValue(TheVisualFlowProperty, value);
        }

        public static readonly AttachedProperty<VisualFlow> TheVisualFlowProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, VisualFlow>
            (
                "TheVisualFlow",
                VisualFlow.Normal
            );
        #endregion TheVisualFlow Attached Avalonia Property

        static VisualFlowBehavior()
        {
            TheVisualFlowProperty.Changed.Subscribe(OnVisualFlowChanged);
        }

        private static void CheckTransform(this IControl control, bool isNormalFlow)
        {
            if (!isNormalFlow && control.RenderTransform != null && !control.RenderTransform.Value.IsIdentity)
            {
                throw new Exception("Error - VisualFlowBehavior might be ruining the current render transform");
            }
        }

        private static void SetTransform(this IControl control, bool isNormalFlow)
        {
            control.CheckTransform(isNormalFlow);

            RelativePoint renderTransformOrigin =
                new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

            if (isNormalFlow)
            {
                control.ClearValue(Visual.RenderTransformOriginProperty);
                control.ClearValue(Visual.RenderTransformProperty);
            }
            else
            {
                ScaleTransform flowTransform = isNormalFlow ? null : new ScaleTransform(-1, 1);

                control.RenderTransformOrigin = renderTransformOrigin;

                control.RenderTransform = flowTransform;
            }
        }

        private static void OnVisualFlowChanged(AvaloniaPropertyChangedEventArgs<VisualFlow> args)
        {
            IControl control = (IControl) args.Sender;

            bool isNormalFlow = (args.NewValue.Value == VisualFlow.Normal);

            control.SetTransform(isNormalFlow);

            if (isNormalFlow)
            {
                IDisposable disposable = GetSubscription(control);

                disposable?.Dispose();

                control.ClearValue(SubscriptionProperty);
            }
            else
            {
                IDisposable subscription =
                    control.LogicalChildren.AddBehavior(OnChildAdded, OnChildRemoved);

                SetSubscription(control, subscription);
            }
        }

        private static bool IsParentNormal(this IControl child)
        {
            IControl parent = (IControl)child.GetLogicalParent();

            bool isNormalFlow = GetTheVisualFlow(parent) == VisualFlow.Normal;

            return isNormalFlow;
        }

        private static void OnChildRemoved(ILogical child)
        {
            IControl childControl = (IControl)child;

            // clear the transform
            childControl.SetTransform(true);
        }

        private static void OnChildAdded(ILogical child)
        {
            IControl childControl = (IControl)child;

            bool isNormalFlow = childControl.IsParentNormal();

            childControl.SetTransform(isNormalFlow);
        }
    }
}
