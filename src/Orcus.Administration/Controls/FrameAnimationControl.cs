using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Orcus.Administration.Controls
{
    public class FrameAnimationControl : ContentControl
    {
        public static readonly DependencyProperty FirstFrameResourceNameProperty =
            DependencyProperty.Register("FirstFrameResourceName", typeof(string), typeof(FrameAnimationControl),
                new PropertyMetadata(default(string), OnResourceNameChanged));

        public static readonly DependencyProperty LastFrameResourceNameProperty =
            DependencyProperty.Register("LastFrameResourceName", typeof(string), typeof(FrameAnimationControl),
                new PropertyMetadata(default(string), OnResourceNameChanged));

        public static readonly DependencyProperty AnimationIntervalProperty =
            DependencyProperty.Register("AnimationInterval", typeof(TimeSpan), typeof(FrameAnimationControl),
                new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

        private CancellationTokenSource _animationCancelSource;

        public TimeSpan AnimationInterval
        {
            get => (TimeSpan) GetValue(AnimationIntervalProperty);
            set => SetValue(AnimationIntervalProperty, value);
        }

        public string FirstFrameResourceName
        {
            get => (string) GetValue(FirstFrameResourceNameProperty);
            set => SetValue(FirstFrameResourceNameProperty, value);
        }

        public string LastFrameResourceName
        {
            get => (string) GetValue(LastFrameResourceNameProperty);
            set => SetValue(LastFrameResourceNameProperty, value);
        }

        private static void OnResourceNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animationControl = (FrameAnimationControl) d;
            animationControl.InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            ClearAnimation();

            var name = FirstFrameResourceName;
            var lastResource = LastFrameResourceName;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastResource))
                return;

            var numberPosition = name.IndexOf('1');
            if (numberPosition < lastResource.Length)
            {
                if (name.AsSpan().Slice(0, numberPosition).SequenceEqual(lastResource.AsSpan().Slice(0, numberPosition)))
                {
                    var i = numberPosition + 1;
                    while (char.IsNumber(lastResource[i]) && lastResource.Length > i - 1)
                        i++;

                    if (name.AsSpan().Slice(numberPosition + 1).SequenceEqual(lastResource.AsSpan().Slice(i)))
                        StartAnimation(name.Replace('1', '*'),
                            int.Parse(lastResource.Substring(numberPosition, i - numberPosition)));
                }
            }
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