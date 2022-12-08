function getWindowDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

function registerListener(dotnethelper) {
    addEventListener('resize', (event) => {
        dotnethelper.invokeMethodAsync('SetCanvasSize').then(() => {
            // success, do nothing
        }).catch(error => {
            console.log("Error during browser resize: " + error);
        });
    });
}