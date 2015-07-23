var JSSerializer = JSSerializer || {};

JSSerializer = (function(){

	function _rawData(item) {

        var type = typeof item;

        if (item === null) {

            return 'null';

        } else if (item === undefined) {

            return 'undefined';

        } else if (type === 'string') {

            return '"' + item + '"';

        } else if (Object.prototype.toString.call(item) === '[object Array]') {

            return _rawArray(item);

        } else if (type === 'object') {

            return _rawObject(item);

        } else if (type === 'function') {

            return item.toString().replace(/((?:{)|(?:})|(?:;))\s+(\S)/ig, '$1 $2');

        } else {

            return item.toString();
        }

    }

    function _rawArray (collection) {

        var result = [];

        result.push('[');

        for (var i = 0, n = collection.length; i < n; i++) {

            result.push(_rawData(collection[i]));

            if (i < (n - 1)) {
                result.push(',');
            }

        }

        result.push(']');

        return result.join('');
    }

    function _rawObject (item) {
        var result = [];

        result.push('{');

        var hasItems = false;

        for (var key in item) {

            hasItems = true;

            result.push(key + ':');
            result.push(_rawData(item[key]));
            result.push(',');

        }

        if (hasItems) {
            result.pop(); // remove last ','            
        }

        result.push('}');

        return result.join('');
    }

    var publicAPI = {

    	object: function(item) {
    		return _rawObject(item);
    	},

    	array: function(item) {
    		return _rawArray(item);
    	},

    	data: function(item) {
    		return _rawData(item);
    	}

    };

    return publicAPI;

})();