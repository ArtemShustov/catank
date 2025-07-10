mergeInto(LibraryManager.library, {
    YSDK_IsReady: function() {
        return typeof window.ysdk !== 'undefined' && window.ysdk !== null;
    },
    YSDK_GetLang: function() {
        var lang;
        if (window.ysdk) {
            lang = window.ysdk.environment.i18n.lang;
        } else {
            console.warn('YandexGamesSDK: SDK not available.');
            lang = "";
        }
        
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    },
    YSDK_GameReady: function() {
        if (window.ysdk) {
            window.ysdk.features.LoadingAPI.ready();
        } else {
            console.warn('YandexGamesSDK: SDK not available.');
        }
    },
    YSDK_IsMobile: function() {
        if (window.ysdk) {
            return ysdk.deviceInfo.isMobile();
        } else {
            console.warn('YandexGamesSDK: SDK not available.');
            return false;
        }
    }
});