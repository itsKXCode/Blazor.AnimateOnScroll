using Blazor.AnimateOnScroll.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.AnimateOnScroll.Components
{
    public partial class AnimateOnScroll
    {
        [Inject] 
        public required IJSRuntime _jsRuntime { get; set; }

        [Parameter]
        public RenderFragment<bool>? ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> VisibilityChanged { get; set; }

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
        /// The Count, how often the Animation should be played
        /// </summary>
        [Parameter] 
        public int AnimationCount { get; set; } = 1;

        /// <summary>
        /// Gets Triggered when the Animation has finished playing
        /// </summary>
        [Parameter]
        public EventCallback OnAnimationEnd { get; set; }

        /// <summary>
        /// If True, the Animation will be reset once it gets out of view and will be played again if it gains visibility again
        /// </summary>
        [Parameter]
        public bool ResetOnNotVisible { get; set; } = true;

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
                                            animation-iteration-count: {AnimationCount};";
        private string AnimationClass => IsVisible ? $"animate__animated animate__{Animation.CssName}" : "";
        private string VisibilityStyle => !IsVisible ? "visibility:hidden;" : "";

        private DotNetObjectReference<AnimateOnScroll>? _thisReference;
        private ElementReference _divReference;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _thisReference = DotNetObjectReference.Create(this);
                await _jsRuntime.InvokeVoidAsync("AnimateOnScroll.Register", _thisReference, _divReference).ConfigureAwait(false);
            }
        }

        [JSInvokable]
        public void InternalOnVisible()
        {
            IsVisible = true;
            StateHasChanged();

            VisibilityChanged.InvokeAsync(true);
        }

        [JSInvokable]
        public void InternalOnNotVisible()
        {
            if (ResetOnNotVisible)
            {
                AnimationEnded = true;
                IsVisible = false;
                StateHasChanged();
            }

            VisibilityChanged.InvokeAsync(false);
        }

        [JSInvokable]
        public void InternalOnAnimationEnd()
        {
            AnimationEnded = true;
            OnAnimationEnd.InvokeAsync();
        }
    }
}
