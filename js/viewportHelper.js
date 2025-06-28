var _objRef;

window.ViewPortChanges = {
    GetDims: function (objRef) {
        _objRef = objRef;
        _objRef.invokeMethodAsync('SetViewPortDimensions', window.innerWidth, window.innerHeight);
    }
}

window.addEventListener('resize', () => {
    if (_objRef != null) {
        _objRef.invokeMethodAsync('SetViewPortDimensions', window.innerWidth, window.innerHeight);
    }
});