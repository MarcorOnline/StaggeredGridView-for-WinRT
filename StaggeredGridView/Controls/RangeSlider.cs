using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Mitomato.Controls
{
    public class RangeSlider : Control
    {
        private const int defaultMin = 1;
        private const int defaultMax = 100;

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.RegisterAttached("Min", typeof(int), typeof(RangeSlider), new PropertyMetadata(defaultMin, OnRangeLimitsChanged));

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.RegisterAttached("Max", typeof(int), typeof(RangeSlider), new PropertyMetadata(defaultMax, OnRangeLimitsChanged));

        private static void OnRangeLimitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RangeSlider;

            if (control.Min >= control.Max)
                throw new Exception(string.Format("RangeSlider: 'Min' and 'Max' values must be different and 'Min' must be lower than 'Max'. {0} is greater or equal than {1}", control.Min, control.Max));

            control.Draw();
        }

        public int Value1
        {
            get { return (int)GetValue(Value1Property); }
            set { SetValue(Value1Property, value); }
        }

        public static readonly DependencyProperty Value1Property =
            DependencyProperty.RegisterAttached("Value1", typeof(int), typeof(RangeSlider), new PropertyMetadata(defaultMin, OnValueChanged));

        public int Value2
        {
            get { return (int)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }

        public static readonly DependencyProperty Value2Property =
            DependencyProperty.RegisterAttached("Value2", typeof(int), typeof(RangeSlider), new PropertyMetadata(defaultMax, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RangeSlider;

            if (control.Value1 > control.Value2)
                throw new Exception(string.Format("RangeSlider: 'Value1' must be lower or equal than 'Value2'. {0} is greater than {1}", control.Value1, control.Value2));

            if (control.leftValue != control.Value1 || control.rightValue != control.Value2)
                control.Draw();
        }

        private int leftValue;
        private int rightValue;

        private CompositeTransform leftTransform;
        private CompositeTransform rightTransform;
        private CompositeTransform fillTransform;

        private Rectangle Track;
        private Rectangle FillTrackGrid;
        private Grid LeftHandle;
        private Grid RightHandle;
        private TextBlock LeftHandleText;
        private TextBlock RightHandleText;

        public RangeSlider()
        {
            this.DefaultStyleKey = typeof(RangeSlider);

            this.SizeChanged += RangeSlider_SizeChanged;

            this.Loaded += RangeSlider_Loaded;
            this.Unloaded += RangeSlider_Unloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //get ui elements from template
            LeftHandle = (Grid)GetTemplateChild("LeftHandle");
            RightHandle = (Grid)GetTemplateChild("RightHandle");
            Track = (Rectangle)GetTemplateChild("Track");
            FillTrackGrid = (Rectangle)GetTemplateChild("FillTrackGrid");
            LeftHandleText = (TextBlock)GetTemplateChild("LeftHandleText");
            RightHandleText = (TextBlock)GetTemplateChild("RightHandleText");

            leftTransform = LeftHandle.RenderTransform as CompositeTransform;
            rightTransform = RightHandle.RenderTransform as CompositeTransform;
            fillTransform = FillTrackGrid.RenderTransform as CompositeTransform;

            Draw();
        }

        private void RangeSlider_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (LeftHandle == null || RightHandle == null)
                return;

            LeftHandle.ManipulationDelta += LeftHandle_ManipulationDelta;
            RightHandle.ManipulationDelta += RightHandle_ManipulationDelta;
        }

        private void RangeSlider_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (LeftHandle == null || RightHandle == null)
                return;

            LeftHandle.ManipulationDelta -= LeftHandle_ManipulationDelta;
            RightHandle.ManipulationDelta -= RightHandle_ManipulationDelta;
        }

        private void RangeSlider_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            if (FillTrackGrid == null || LeftHandleText == null || RightHandleText == null || FillTrackGrid.ActualWidth == 0)
                return;

            leftValue = Value1;
            rightValue = Value2;

            if (leftValue < Min)
                throw new Exception(string.Format("RangeSlider: 'Value1' must be greater or equal than 'Min'. {0} is lower than {1}", leftValue, Min));

            if (rightValue > Max)
                throw new Exception(string.Format("RangeSlider: 'Value2' must be lower or equal than 'Max'. {0} is greater than {1}", rightValue, Max));

            //LEFT
            var pos = SetPosition(leftValue);
            leftTransform.TranslateX = pos;
            LeftHandleText.Text = leftValue.ToString();

            //RIGHT
            pos = SetPosition(rightValue);
            rightTransform.TranslateX = pos;
            RightHandleText.Text = rightValue.ToString();

            //FILL
            FillTrack();
        }

        //Changes left thumb
        private void LeftHandle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var translate = Translate(leftTransform, e.Delta.Translation.X, true);
            leftTransform.TranslateX = translate;
            leftValue = CalculateValue(translate);
            Value1 = leftValue;
            LeftHandleText.Text = leftValue.ToString();

            FillTrack();
        }

        //Changes right thumb
        private void RightHandle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var translate = Translate(rightTransform, e.Delta.Translation.X, false);
            rightTransform.TranslateX = translate;
            rightValue = CalculateValue(translate);
            Value2 = rightValue;
            RightHandleText.Text = rightValue.ToString();

            FillTrack();
        }

        private double Translate(CompositeTransform s, double deltaTranslateX, bool left)
        {
            var uiRange = Track.ActualWidth;
            var minimum = left ? -uiRange / 2 : leftTransform.TranslateX;
            var maximum = left ? rightTransform.TranslateX : uiRange / 2;

            var target = s.TranslateX + deltaTranslateX;

            if (target < minimum)
                return minimum;
            if (target > maximum)
                return maximum;

            return target;
        }

        private int CalculateValue(double xTranslation)
        {
            var max = Max;
            var min = Min;

            var valueRange = max - min;
            var uiRange = Track.ActualWidth;

            var value = (xTranslation + uiRange / 2) * valueRange / uiRange;

            var rounded = (int)Math.Round(value, 0) + min;

            if (rounded > max)
                return max;
            if (rounded < min)
                return min;

            return rounded;
        }

        public double SetPosition(int value)
        {
            var min = Min;

            var valueRange = Max - min;
            var uiRange = Track.ActualWidth;

            var uiPosition = (value - min) * uiRange / valueRange;

            return uiPosition - (uiRange / 2);
        }

        public void FillTrack()
        {
            var fillWidth = rightTransform.TranslateX - leftTransform.TranslateX;
            FillTrackGrid.Width = fillWidth > 0 ? fillWidth : 0;

            var x = (Math.Abs(rightTransform.TranslateX) - Math.Abs(leftTransform.TranslateX)) / 2;

            if (leftTransform.TranslateX > 0)
                x += leftTransform.TranslateX;
            else if (rightTransform.TranslateX < 0)
                x += rightTransform.TranslateX;

            fillTransform.TranslateX = x;
        }
    }
}