// ALREADY DEFINED VARIABLES:
// 1) SERVER_BASE_URL (config.js)
// 2) FACEBOOK_ID (FacebookSDK.js)

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

var contextId = uuidv4();
function requestPageLoadBIEvent() {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", SERVER_BASE_URL + "/v0/facebook/pre_login");
    xhr.setRequestHeader('Content-Type', 'application/json');
    var queryString = "";
    var questionMarkIndex = document.location.href.indexOf('?');
    if (questionMarkIndex >= 0) {
        queryString = document.location.href.substring(questionMarkIndex + 1);
    }
    xhr.send(JSON.stringify({
        context_id: contextId,
        query_string: queryString,
    }));
}

function requestBookmarkButtonClickBIEvent() {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", SERVER_BASE_URL + "/v0/facebook/bookmark_click");
    xhr.setRequestHeader('Content-Type', 'application/json');
    var queryString = "";
    var questionMarkIndex = document.location.href.indexOf('?');
    if (questionMarkIndex >= 0) {
        queryString = document.location.href.substring(questionMarkIndex + 1);
    }
    xhr.send(JSON.stringify({
        context_id: contextId,
        query_string: queryString,
        facebook_id: FACEBOOK_ID
    }));
}

function requestFbAuthBIEvent() {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", SERVER_BASE_URL + "/v0/facebook/fb_auth");
    xhr.setRequestHeader('Content-Type', 'application/json');
    var queryString = "";
    var questionMarkIndex = document.location.href.indexOf('?');
    if (questionMarkIndex >= 0) {
        queryString = document.location.href.substring(questionMarkIndex + 1);
    }
    xhr.send(JSON.stringify({
        context_id: contextId,
        query_string: queryString,
        facebook_id: FACEBOOK_ID
    }));
}

function requestFbAppIconClickBIEvent(clickType) {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", SERVER_BASE_URL + "/v0/facebook/app_icon_click");
    xhr.setRequestHeader('Content-Type', 'application/json');
    var queryString = "";
    var questionMarkIndex = document.location.href.indexOf('?');
    if (questionMarkIndex >= 0) {
        queryString = document.location.href.substring(questionMarkIndex + 1);
    }
    xhr.send(JSON.stringify({
        context_id: contextId,
        query_string: queryString,
        facebook_id: FACEBOOK_ID,
        type: clickType
    }));
}