using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.AnimateOnScroll
{
    public partial class AnimateOnScroll
    {
        [Inject]
        public IJSRuntime _jsRuntime { get; set; } = default!;

        [Parameter]
        public RenderFragment<bool>? ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibilityChanged { get; set; }

        [Parameter]
        public string Style { get; set; } = string.Empty;

        [Parameter]
        public string Class { get; set; } = string.Empty;

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? AdditionalAttributes { get; set; }

        /// <summary>
        /// The Animation to be played
        /// </summary>
        [Parameter, EditorRequired] 
        public Animation Animation { get; set; } = Animations.SlideInRight;

        /// <summary>
        /// The Animation to be played when the Element gets scrolled back out of view
        /// </summary>
        [Parameter]
        public Animation OutAnimation { get; set; } = Animations.SlideOutLeft;
        /// <summary>
        /// The Duration, the Animation should take
        /// </summary>
        [Parameter]
        public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// The Delay before the Animation should play
        /// </summary>
        [Parameter]
        public TimeSpan AnimationDelay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The Count, how often the Animation should be played, any negative Number will make it infinite
        /// </summary>
        [Parameter] 
        public int AnimationCount { get; set; } = 1;

        /// <summary>
        /// Gets Triggered when the Animation has finished playing
        /// </summary>
        [Parameter]
        public EventCallback OnAnimationEnd { get; set; }

        /// <summary>
        /// Sets the Offset (in px) of the original trigger point
        /// </summary>
        [Parameter]
        public int Offset { get; set; }

        /// <summary>
        /// Indicates if the Animation will be played only on the first time the Element gets Visible (stays visible afterwards)
        /// </summary>
        [Parameter]
        public bool Once { get; set; }

        /// <summary>
        /// Indicates if the Component is currently Visible or not
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// Indicates if the Animation has been Played
        /// </summary>
        public bool AnimationEnded { get; set; }

        private string AnimationStyle => $@"animation-duration: {AnimationDuration.TotalMilliseconds}ms;
                                            animation-delay: {AnimationDelay.TotalMilliseconds}ms;
                                            animation-iteration-count: {(AnimationCount >= 0 ? AnimationCount : "infinite")};";
        private string AnimationClass => IsVisible ? 
                                            _isOutAnimation ? $"animate__animated animate__{OutAnimation.Name}" : $"animate__animated animate__{Animation.Name}" 
                                            : "";
        private string VisibilityStyle => !IsVisible ? "visibility:hidden;" : "";

        private bool _isOutAnimation = false;

        private DotNetObjectReference<AnimateOnScroll>? _thisReference;
        private ElementReference _divReference;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _thisReference = DotNetObjectReference.Create(this);
                await _jsRuntime.InvokeVoidAsync("AnimateOnScroll.Register", _thisReference, _divReference, Offset).ConfigureAwait(false);
            }
        }

        [JSInvokable]
        public void InternalOnVisible()
        {
            IsVisible = true;
            _isOutAnimation = false;
            StateHasChanged();

            OnVisibilityChanged.InvokeAsync(true);
        }

        [JSInvokable]
        public void InternalOnNotVisible()
        {
            if (!Once)
            {
                AnimationEnded = true;
                _isOutAnimation = true;
                StateHasChanged();
            }

            OnVisibilityChanged.InvokeAsync(false);
        }

        [JSInvokable]
        public void InternalOnAnimationEnd()
        {
            if (_isOutAnimation)
                IsVisible = false;

            AnimationEnded = true;
            StateHasChanged();
            OnAnimationEnd.InvokeAsync();
        }
    }
}
