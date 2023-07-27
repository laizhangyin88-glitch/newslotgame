var OpenWindowPlugin = {
    OpenWindow: function(link)
    {
        var url = Pointer_stringify(link);
        document.onmouseup = function()
        {
            window.open(url, '_blank');
            document.onmouseup = null;
        }
    },
    OpenFBLikePopup: function()
    {
        document.onmouseup = function()
        {
            var url = 'https://www.facebook.com/plugins/error/confirm/like?iframe_referer=https%3A%2F%2Fapps-1881581215396162.apps.fbsbx.com%2Fbundle%2F887227178069179%2F1654558804607403%2Findex.html&kid_directed_site=false&secure=true&plugin=like&return_params=%7B%22href%22%3A%22https%3A%2F%2Fwww.facebook.com%2FClubVegasSlots%22%2C%22layout%22%3A%22button%22%2C%22action%22%3A%22like%22%2C%22size%22%3A%22large%22%2C%22show_faces%22%3A%22false%22%2C%22share%22%3A%22false%22%2C%22height%22%3A%2235%22%2C%22colorscheme%22%3A%22dark%22%2C%22appId%22%3A%221881581215396162%22%2C%22ret%22%3A%22sentry%22%2C%22act%22%3A%22like%22%7D';
            var title = '';
            var w = 486;
            var h = 268;

            // Fixes dual-screen position                         Most browsers      Firefox
            var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
            var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

            var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
            var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

            var left = ((width / 2) - (w / 2)) + dualScreenLeft;
            var top = ((height / 2) - (h / 2)) + dualScreenTop;
            var newWindow = window.open(url, title, 'noopener=yes, scrollbars=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);

            try
            {
                newWindow.focus();
            } catch (e)
            {
                console.log(e);
            }

            document.onmouseup = null;
        }
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);
