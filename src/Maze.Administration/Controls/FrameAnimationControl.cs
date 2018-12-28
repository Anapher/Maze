using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Maze.Administration.Controls
{
    public class FrameAnimationControl : ContentControl
    {
        public static readonly DependencyProperty FrameResourceNameProperty = DependencyProperty.Register("FrameResourceName",
            typeof(string), typeof(FrameAnimationControl),
            new PropertyMetadata(default(string), OnResourceNameChanged));

        public static readonly DependencyProperty AnimationIntervalProperty =
            DependencyProperty.Register("AnimationInterval", typeof(TimeSpan), typeof(FrameAnimationControl),
                new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

        private CancellationTokenSource _animationCancelSource;

        public string FrameResourceName
        {
            get => (string) GetValue(FrameResourceNameProperty);
            set => SetValue(FrameResourceNameProperty, value);
        }

        public TimeSpan AnimationInterval
        {
            get => (TimeSpan) GetValue(AnimationIntervalProperty);
            set => SetValue(AnimationIntervalProperty, value);
        }

        private static void OnResourceNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animationControl = (FrameAnimationControl) d;
            animationControl.InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            ClearAnimation();

            var resourceName = FrameResourceName;

            if (string.IsNullOrEmpty(resourceName))
                return;

            var parts = resourceName.Split('/');
            var name = parts[0];
            var count = int.Parse(parts[1]);

            StartAnimation(name, count);
        }

        private async void StartAnimation(string name, int count)
        {
            _animationCancelSource = new CancellationTokenSource();
            var token = _animationCancelSource.Token;
            var framesCache = new object[count];
            var currentFrame = 1;

            while (!token.IsCancellationRequested)
            {
                if (framesCache[currentFrame - 1] == null)
                {
                    var resourcenName = name.Replace("*", currentFrame.ToString());
                    var resource = FindResource(resourcenName);
                    framesCache[currentFrame - 1] = resource;

                    Content = resource;
                }
                else
                {
                    Content = framesCache[currentFrame - 1];
                }

                currentFrame++;
                if (currentFrame > count)
                    currentFrame = 1;

                try
                {
                    await Task.Delay(AnimationInterval, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private void ClearAnimation()
        {
            _animationCancelSource?.Dispose();
        }
    }
}