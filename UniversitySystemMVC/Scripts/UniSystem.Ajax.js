var UniSystem = UniSystem || {};

UniSystem.Ajax = UniSystem.Ajax || {};

(function () {

    var _private = {
        async: true
    };

    function _getData(data) {
        // avoid cache of response
        var dataString = "noCacheIndex=" + Math.random();
        var stringData = [];
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

        if (method === "GET")  {
            url = url + "?" + _getData(data);
        }

        xmlhttp.open(method, url, _private.async);

        if (_private.async === true) {

            xmlhttp.onreadystatechange = function () {

                if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                    callback(JSON.parse(xmlhttp.responseText));
                }

            }

        }

        if (method === "POST") {

            xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            xmlhttp.send(dataString);

        } else {

            xmlhttp.send();

        }

        if (_private.async === false) {
        	callback(JSON.parse(xmlhttp.responseText));
        }
    }

    UniSystem.Ajax.isAsync = function () {
        return _private.async;
    }

    UniSystem.Ajax.setAsync = function (value) {
        _private.async = value === true ? true : false;
    }

    UniSystem.Ajax.get = function (url, data, callback) {

        _sendAjax("GET", url, data, callback);

    };

    UniSystem.Ajax.post = function (url, data, callback) {

        _sendAjax("POST", url, data, callback);

    }

})();