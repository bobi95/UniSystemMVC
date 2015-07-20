var UniSystem = UniSystem || {};

UniSystem.Ajax = (function () {

    var _async = true;

    function _getData(data) {

        // avoid cache of response
        var dataString = "noCacheIndex=" + Math.random(),
            stringData = [];

        for (var index in data) {
            stringData.push(index + "=" + data[index]);
        }

        stringData = stringData.join('&');

        if (stringData) {
            dataString += '&';
        }

        return dataString + stringData;
    }

    function _sendAjax(method, url, data, callback) {

        var xmlhttp;

        if (window.XMLHttpRequest) {

            xmlhttp = new XMLHttpRequest();

        } else {

            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");

        }

        if (method === "GET") {
            url = url + "?" + _getData(data);
        }

        xmlhttp.open(method, url, _async);

        if (_async === true && callback) {
            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                    callback(xmlhttp.responseText);
                }
            }
        }

        if (method === "POST") {

            xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            xmlhttp.send(dataString);

        } else {

            xmlhttp.send();

        }

        if (_async === false && callback) {
            callback(xmlhttp.responseText);
        }
    }

    var publicAPI = {

        isAsync: function () {
            /// <summary>
            /// Is the Ajax script using async callbacks.
            /// </summary>
            /// <returns type="Boolean">True if async is enabled.</returns>
            return _async;
        },

        setAsync: function (value) {
            /// <summary>
            /// Set true to use async callbacks.
            /// </summary>
            /// <param name="value" type="Boolean">True for async callbacks.</param>
            _async = value === true ? true : false;
        },

        get: function (url, data, callback) {
            /// <summary>
            /// Send data using a get request.
            /// </summary>
            /// <param name="url" type="String">Url to send data to</param>
            /// <param name="data" type="Object|Array">Data to be sent. Must be key-value.</param>
            /// <param name="callback" type="Function">Function to be called when response is gotten.</param>
            _sendAjax('GET', url, data, callback);
        },

        post: function (url, data, callback) {
            /// <summary>
            /// Send data using a post request.
            /// </summary>
            /// <param name="url" type="String">Url to send data to</param>
            /// <param name="data" type="Object|Array">Data to be sent. Must be key-value.</param>
            /// <param name="callback" type="Function">Function to be called when response is gotten.</param>
            _sendAjax('POST', url, data, callback);
        }
    };

    return publicAPI;

})();