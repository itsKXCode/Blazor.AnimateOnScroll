let Components = [];

document.getElementsByTagName("head")[0].insertAdjacentHTML(
    "beforeend",
    "<link rel=\"stylesheet\" href=\"_content/Blazor.AnimateOnScroll/css/animate.css\" />");


window.addEventListener('scroll', OnScrollCallback);

const mutationCallback = (mutationList, observer) => {

    if (Components.length === 0)
        return;

    //Search for Deleted Nodes which are inside our Components Array and delete them
    for (const mutation of mutationList) {
        if (mutation.removedNodes.length > 0) {
            mutation.removedNodes.forEach((removed, id) => {
                Components.forEach((component, index) => {
                    if (component.observeElement === removed) {
                        Components.splice(index, 1);
                        return;
                    }
                })
            })
        }
    }
}

//Observer for watching for deleted Components
const observer = new MutationObserver(mutationCallback);
observer.observe(window.document.documentElement, {
    childList: true,
    subtree: true,
    removeNodes: true
});

function OnScrollCallback() {

    Components.forEach((component, index) => {

        if (IsElementInView(component.observeElement, component.offset)) {
            //Element is Visible

            if (component.isVisible == false) {
                component.isVisible = true;
                component.parentComponent.invokeMethodAsync('InternalOnVisible');
            }
        }
        else {
            //Element is NOT Visible

            if (component.isVisible == true) {
                component.isVisible = false;
                component.parentComponent.invokeMethodAsync('InternalOnNotVisible');
            }
        }

    });
}

function IsElementInView(element, triggerOffset) {

    const viewBottom = window.innerHeight + window.pageYOffset;
    const viewTop = window.pageYOffset;
    const offset = GetOffset(element);

    if (offset.top + triggerOffset < viewBottom) {
        return true;
    }

    return false;
}

function GetOffset(element) {
    let x = 0;
    let y = 0;

    while (element && !isNaN(element.offsetLeft) && !isNaN(element.offsetTop)) {
        x += element.offsetLeft - (element.tagName != 'BODY' ? element.scrollLeft : 0);
        y += element.offsetTop - (element.tagName != 'BODY' ? element.scrollTop : 0);
        element = element.offsetParent;
    }

    return {
        top: y,
        left: x
    };
}


window.AnimateOnScroll = {

    Register: function (instance, divReference, pOffset) {

        //Listen to animationend event to notify the Blazor Component
        divReference.addEventListener('animationend', function (e) {
            if (e.target === this)instance.invokeMethodAsync('InternalOnAnimationEnd');
        });

        Components.push({
            parentComponent: instance,
            observeElement: divReference,
            offset: pOffset,
            isVisible: false
        });
    },
}