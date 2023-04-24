# Animate your Blazor Components as you Scroll

A Library for animating your Components as you scroll trough your Page. Uses [Animate.css](https://github.com/animate-css/animate.css) for animating.

## How to Install

Add the Nuget Package to your Solution

    Install-Package Blazor.AnimateOnScroll
    
Add the JavaScript File to your _Host.cshtml

    <script src="_content/Blazor.AnimateOnScroll/js/AnimateOnScroll.js"></script>

Add the Namespace to your _Imports.razor

    @using Blazor.AnimateOnScroll

## How to Use

Use the AnimateOnScroll Component and put the Element to Animate inside it

    <AnimateOnScroll Animation="Animations.SlideInUp">
            <img src="..."/>
    </AnimateOnScroll>
Now, as your img gets scrolled into View, the Animation will start. Once the img gets scrolled OUT of View, the Animation resets and will be played again, the next time it gets scrolled into View.

If you dont want your Animation to be reset, just set the Property

    ResetOnNotVisible="false"

Set the Duration the Animation will take

     AnimationDuration="TimeSpan.FromSeconds(2)"

Set a Delay to wait before the Animation plays

    AnimationDelay="TimeSpan.FromSeconds(2)"

Set the Count the Animation will be Played (any negative Value will make it repeat infinitely)

    AnimationCount="10"
### Events

You can get Notified, when the Components Visibility changed

    <AnimateOnScroll ... OnVisibilityChanged="VisibilityChanged">
            <img src="..."/>
    </AnimateOnScroll>
	
	@code{
		private void VisibilityChanged(bool IsVisible)
		{
			...
		}
	}

Or when the Animation finished playing

    <AnimateOnScroll ... OnAnimationEnd="AnimationEnd">
            <img src="..."/>
    </AnimateOnScroll>
	
	@code{
		private void AnimationEnd()
		{
			...
		}
	}

### Cascading Parameters
The following Parameters will be passed as CascadingValues to your Components

    bool IsVisible;
    bool AnimationEnded;

## Animations
As this Library is using Animate.css, every of its Animation is available
Just use one of the Pre-Defined Animation Propertys in the Static 'Animations' Class

    public static class Animations
    {
        public static Animation Bounce => new Animation("bounce");
        public static Animation Flash => new Animation("flash");
        public static Animation Pulse => new Animation("pulse");
        public static Animation RubberBand => new Animation("rubberBand");
        public static Animation ShakeX => new Animation("shakeX");
        public static Animation ShakeY => new Animation("shakeY");
        public static Animation HeadShake => new Animation("headShake");
        ...
    }

## License
Blazor.AnimateOnScroll is MIT licensed. The library uses the following other libraries:

-   [Animate.css](https://github.com/animate-css/animate.css)