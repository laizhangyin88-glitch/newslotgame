// ALREADY DEFINED VARIABLES:
// 1) FB (FacebookSDK.js)
// 2) ORIG_GAME_CONTAINER_WIDTH, ORIG_GAME_CONTAINER_HEIGHT (index.html)

var progressBarLength = 492;
var animatedLoadingInitiated = false;

function UnityProgress(gameInstance, progress) {
    var downloadLoadingRatio = 0.2;

    if (progress == "complete") {
        var loadingRelatedElementList = [];
        loadingRelatedElementList.push(document.getElementById('overlay'));
        loadingRelatedElementList.push(document.getElementById('ProgressLine'));
        loadingRelatedElementList.push(document.getElementById('loadingInfo'));
        loadingRelatedElementList.push(document.getElementsByClassName('counter')[0]);
        loadingRelatedElementList.push(document.getElementsByClassName('question')[0]);
        loadingRelatedElementList.push(document.getElementsByClassName('answer')[0]);
        loadingRelatedElementList.push(document.getElementsByClassName('answer')[1]);
        loadingRelatedElementList.push(document.getElementsByClassName('answer')[2]);
        loadingRelatedElementList.push(document.getElementsByClassName('answer')[3]);
        loadingRelatedElementList.push(document.getElementsByClassName('answer-instruction')[0]);
        loadingRelatedElementList.push(document.getElementById('qa-wrapper'));
        loadingRelatedElementList.push(document.getElementById('loading-screen'));
        for (var i = 0; i < loadingRelatedElementList.length; ++i) {
            var loadingRelatedElement = loadingRelatedElementList[i];
            if (loadingRelatedElement != null) {
                loadingRelatedElement.remove();
            }
        }
    } else if (progress >= 0.9 && !animatedLoadingInitiated) {
        document.getElementById("ProgressLine").style.width = '';
        document.getElementById('ProgressLine').classList.add('ProgressLine');
        animatedLoadingInitiated = true;
    } else if (!animatedLoadingInitiated) {
        document.getElementById("ProgressLine").style.width = (progressBarLength * (progress) * downloadLoadingRatio) + "px";
    }
}

var previousPageInfo = {
    body_width: null,
    height: null
};
function resizeGameScreen(pageInfo) {
    var bodyWidth = document.body.clientWidth;
    if (previousPageInfo.body_width == bodyWidth && previousPageInfo.height == pageInfo.clientHeight) {
        return;
    } else {
        previousPageInfo.body_width = bodyWidth;
        previousPageInfo.height = pageInfo.clientHeight;
    }
    var gameContainer = document.getElementById('gameContainer');
    var gameLogo = document.getElementsByClassName('game-logo')[0];
    var backgroundGlowEffect = document.getElementsByClassName('circle-glow-effect')[0];
    var fullScreenButton = document.getElementsByClassName('fullscreen')[0];
    var header = document.getElementsByClassName('header')[0];
    var footer = document.getElementsByClassName('footer')[0];
    var canvas = document.getElementsByTagName('canvas')[0];
    var loadingScreen = document.getElementById('loading-screen');
    var facebookHeaderHeight = 42; // If facebook UI changes, this value should be changed accordingly if facebook UI change.
    var canvasPageHeight = pageInfo.clientHeight - facebookHeaderHeight;
    var footerTopMargin = 5;
    var minimumFooterSize = 36 + footerTopMargin;
    var minimumHeaderSize = 70;
    var minimumLogoSize = 100;
    var minimumSideGap = 10;
    var zoom;
    if ((canvasPageHeight - header.offsetHeight - footer.offsetHeight) > ORIG_GAME_CONTAINER_HEIGHT) {
        header.style.height = '';
        fullScreenButton.style.height = '';
        gameLogo.style.visibility = '';
        backgroundGlowEffect.style.visibility = '';
        gameLogo.style.height = '';
        zoom = Math.max(Math.min((bodyWidth - minimumSideGap * 2) / ORIG_GAME_CONTAINER_WIDTH, (canvasPageHeight - header.offsetHeight - footer.offsetHeight) / ORIG_GAME_CONTAINER_HEIGHT), 0);
    } else if ((canvasPageHeight - header.offsetHeight - minimumFooterSize) > ORIG_GAME_CONTAINER_HEIGHT) {
        header.style.height = '';
        fullScreenButton.style.height = (canvasPageHeight - header.offsetHeight - ORIG_GAME_CONTAINER_HEIGHT - footerTopMargin) + "px";
        gameLogo.style.visibility = '';
        backgroundGlowEffect.style.visibility = '';
        gameLogo.style.height = '';
        zoom = Math.min(1.0, (bodyWidth - minimumSideGap * 2) / ORIG_GAME_CONTAINER_WIDTH, (canvasPageHeight - header.offsetHeight - minimumFooterSize) / ORIG_GAME_CONTAINER_HEIGHT);
    } else if ((canvasPageHeight - minimumHeaderSize - minimumFooterSize) > ORIG_GAME_CONTAINER_HEIGHT) {
        header.style.height = (canvasPageHeight - minimumFooterSize - ORIG_GAME_CONTAINER_HEIGHT) + "px";
        fullScreenButton.style.height = (minimumFooterSize - footerTopMargin) + "px";
        if ((canvasPageHeight - minimumFooterSize - ORIG_GAME_CONTAINER_HEIGHT) > minimumLogoSize) {
            gameLogo.style.visibility = '';
            backgroundGlowEffect.style.visibility = '';
            gameLogo.style.height = (canvasPageHeight - minimumFooterSize - ORIG_GAME_CONTAINER_HEIGHT) / 9 * 10 + "px";
        } else {
            gameLogo.style.visibility = 'hidden';
            backgroundGlowEffect.style.visibility = 'hidden';
        }
        zoom = Math.min(1.0, (bodyWidth - minimumSideGap * 2) / ORIG_GAME_CONTAINER_WIDTH, (canvasPageHeight - header.offsetHeight - minimumFooterSize) / ORIG_GAME_CONTAINER_HEIGHT);
    } else {
        fullScreenButton.style.height = (minimumFooterSize - footerTopMargin) + "px";
        header.style.height = minimumHeaderSize + "px";
        gameLogo.style.visibility = 'hidden';
        backgroundGlowEffect.style.visibility = 'hidden';
        zoom = Math.min((bodyWidth - minimumSideGap * 2) / ORIG_GAME_CONTAINER_WIDTH, (canvasPageHeight - header.offsetHeight - minimumFooterSize) / ORIG_GAME_CONTAINER_HEIGHT);
    }
    zoom = Math.max(0.6, zoom);
    var resizedContainerWidth = ORIG_GAME_CONTAINER_WIDTH * zoom;
    var resizedContainerHeight = ORIG_GAME_CONTAINER_HEIGHT * zoom;
    gameContainer.style.width = resizedContainerWidth + "px";
    gameContainer.style.height = resizedContainerHeight + "px";
    if (canvas) {
        canvas.style.width = "100%";
        canvas.style.height = "100%";
    }
    if (loadingScreen) {
        loadingScreen.style.transform = 'scale(' + zoom + ')';
        loadingScreen.style['margin-top'] = header.offsetHeight + 'px';
    }
}

function startScreenResizeInterval() {
    var resizeFunc = function () {
        // TODO: replace hardcoded value with (window.outerHeight - clientHeight get by FB.Canvas.getPageInfo on init) after the issue that getPageInfo's message opens login page is resolved
        resizeGameScreen({
            clientHeight: window.outerHeight - 120,
        });
    };
    resizeFunc();
    return setInterval(resizeFunc, 750);
}

function fixLoadingScreenWidthHeightCssProperties() {
    var loadingScreen = document.getElementById('loading-screen');
    loadingScreen.style.width = ORIG_GAME_CONTAINER_WIDTH + 'px';
    loadingScreen.style.height = ORIG_GAME_CONTAINER_HEIGHT + 'px';
}

function wipeOutLoadingScene(gameInstance) {
    var animatedLoadingRatio = 0.5;
    document.getElementById("ProgressLine").style.width = (progressBarLength * animatedLoadingRatio) + "px";
    UnityProgress(gameInstance, "complete");
}

document.addEventListener("DOMContentLoaded", function (event) {
    fixLoadingScreenWidthHeightCssProperties();
    startScreenResizeInterval();
});
