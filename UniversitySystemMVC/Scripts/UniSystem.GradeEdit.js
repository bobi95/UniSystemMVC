var UniSystem = UniSystem || {};

UniSystem.Templater = (function () {
    
    var expressionsRAW = {
        findCode: '(.+?(?:{{[^{}]*}}))',
        templateCodeRaw: '[^{}]+',
        nameRaw: '[a-z_][a-z0-9_]*',
        objRaw: '(?:[a-z_][a-z0-9_.]*[a-z0-9_])|(?:[a-z_][a-z0-9_]*)',
        funcPrefix: '#',
    };

    expressionsRAW.func = expressionsRAW.funcPrefix + expressionsRAW.nameRaw;
    expressionsRAW.funcCapture = expressionsRAW.funcPrefix + '(' + expressionsRAW.nameRaw + ')';

    var expressions = {
        findCode: new RegExp(expressionsRAW.findCode, 'i'),
        templateCode: new RegExp('({{' + expressionsRAW.templateCodeRaw + '}})', 'g'),
        func: new RegExp('^{{' + expressionsRAW.funcCapture + '}}$', 'i'),
        obj: new RegExp('^{{(?:' + expressionsRAW.func + '\\s)?(' + expressionsRAW.objRaw + ')}}$', 'i')
    };

    function _compile(html) {
        /// <summary>
        /// Compiles the given html and returns an executable template function.
        /// </summary>
        /// <param name="html" type="String"></param>
        /// <returns type="template"></returns>

        var templateCode = [];


        while (true) {

            var index = html.search(expressions.templateCode);

            if (index === -1) {

                if (html) {
                    templateCode.push(html);
                }

                break;
            }
            
            var noncode = html.substring(0, index);

            if (noncode) {

                templateCode.push(noncode);
                html = html.substring(noncode.length);
                continue;

            }

            var code = html.match(expressions.templateCode);

            if (code) {

                templateCode.push(_processCode(code[0]));
                continue;

            }

            break;
        }

        return new template(templateCode);
    }

    function _processCode(expr) {

        

    }

    function _run(code, funcs, context) {
        
    }

    

    function template(templateCode) {

        var code = templateCode,
            funcs = {};

        return {

            registerFunction: function(key, func) {
                funcs[key] = func;
            },

            run: function (context) {
                return _run(code, funcs, context);
            }
        };
    }

    var templaterPublicAPI = {
        compile: _compile
    };

    return templaterPublicAPI;

})();