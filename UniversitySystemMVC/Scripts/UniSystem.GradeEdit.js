var UniSystem = UniSystem || {};

UniSystem.Templater = (function () {
    
    /*
     * Expressions built
     */

    var expressionsRAW = {
        findCode: '(.+?(?:{{[^{}]*}}))',
        templateCodeRaw: '[^{}]+',
        nameRaw: '[a-z_][a-z0-9_]*',
        objRaw: '(?:[a-z_](?:\\.[a-z0-9_]+?)*)|(?:[a-z_][a-z0-9_]*)',
        funcPrefix: '#',
    };

    expressionsRAW.func = expressionsRAW.funcPrefix + expressionsRAW.nameRaw;
    expressionsRAW.funcCapture = expressionsRAW.funcPrefix + '(' + expressionsRAW.nameRaw + ')';
    expressionsRAW.newTemplate = expressionsRAW.funcPrefix + '(' + expressionsRAW.nameRaw + ')' + expressionsRAW.funcPrefix;
    expressionsRAW.closeNewTemplate = function (name) {
        return '\\/' + expressionsRAW.funcPrefix + '(' + name + ')' + expressionsRAW.funcPrefix;
    }

    var expressions = {
        findCode: new RegExp(expressionsRAW.findCode, 'i'),
        templateCode: new RegExp('({{' + expressionsRAW.templateCodeRaw + '}})', 'g'),
        func: new RegExp('^{{' + expressionsRAW.funcCapture + '(?:\s+(' + expressionsRAW.objRaw + '))?}}$', 'i'),
        // ^{{#([a-z_][a-z0-9_]*)(?:\\s+((?:[a-z_][a-z0-9_.]*[a-z0-9_])|(?:[a-z_][a-z0-9_]*)))?}}$
        obj: new RegExp('^{{(' + expressionsRAW.objRaw + ')}}$', 'i'),
        newTemplate: new RegExp('^{{' + expressionsRAW.newTemplate + '}}$', 'i'),

        closeNewTemplate: function (name) {
            return new RegExp('^{{' + expressionsRAW.closeNewTemplate(name) + '}}$', 'i');
        }
    };



    function _compile(html) {
        /// <summary>
        /// Compiles the given html and returns an executable template function.
        /// </summary>
        /// <param name="html" type="String"></param>
        /// <returns type="template"></returns>

        var templateCode = [],
            templates = {};

        function _processCode(expr) {
            /// <summary>
            /// Processes the captured code from the html and returns the appropriate object.
            /// </summary>
            /// <param name="expr" type="String"></param>
            /// <returns type="Object"></returns>

            var match = expr.match(expressions.func);

            if (match) {

                html = html.substr(expr.length);

                var funcContext = match[2];

                if (funcContext) {
                    funcContext = funcContext.split('.');
                }

                return {
                    type: 'function',
                    key: match[1],
                    context: funcContext
                };

            }

            match = expr.match(expressions.obj);

            if (match) {

                html = html.substr(expr.length);

                return {
                    type: 'echo',
                    keys: match[1].split('.')
                };

            }

            match = expr.match(expressions.newTemplate);

            if (match) {

                var closingTagIndex = html.search(expressions.closeNewTemplate(match[1]));

                var newHTML = html.substring(closingTagIndex);

                html = html.substring(closingTagIndex);

                var newTemplate = _compile(newHTML);

                return {
                    type: 'template',
                    name: match[1],
                    template: newTemplate
                };
            }
        }

        
        while (true) {

            var index = html.search(expressions.templateCode);

            // if no more code is present
            if (index === -1) {

                if (html) {
                    templateCode.push(html);
                }

                break;
            }

            // if next text in "stream" is not code
            var noncode = html.substring(0, index);

            if (noncode) {

                templateCode.push(noncode);
                html = html.substring(noncode.length);
                continue;

            }

            // if next text in "stream" is code
            var code = html.match(expressions.templateCode);

            if (code) {

                var codeResult = _processCode(code[0]);

                if (codeResult.type === 'template') {

                    templates[codeResult.name] = codeResult.template;
                    continue;

                }

                templateCode.push();
                continue;

            }

            break;
        }

        return new template(templateCode, templates);
    }

    
    function _run(code, funcs, context) {
        /// <summary>
        /// Runs the code process, using the given funcs and context.
        /// </summary>
        /// <param name="code" type="Array"></param>
        /// <param name="funcs" type="Object"></param>
        /// <param name="context" type="Object"></param>
        /// <returns type="String"></returns>

        var result = [];

        code.forEach(function (item) {

            if (typeof item === 'string') {
                result.push(item);
                return;
            }

            if (typeof item === 'object') {

                if (item.type === 'echo') {

                    var answer = context;

                    for (var i = 0, n = item.keys.length; i < n; i++)
                    {
                        answer = answer[item[i]];
                    }

                    result.push(answer);
                    return;
                }

                if (item.type === 'function') {

                    if (funcs[item.key]) {
                        
                        if (item.context) {

                            var funcContext = context;

                            for (var i = 0, n = item.context.length; i < n; i++) {
                                funcContext = funcContext[item[i]];
                            }

                            funcs[item.key](funcContext);
                            return;
                        }

                        funcs[item.key]();
                        return;
                    }
                }
            }
        });

        return result.join('');
    }

    

    function template(templateCode, subTemplates) {

        var code = templateCode,
            funcs = {},
            templates = subTemplates; 

        return {

            registerFunction: function (name, func) {
                /// <summary>
                /// Add a function to be called during the render of the template.
                /// </summary>
                /// <param name="key" type="String">Name of function, used inside the template.</param>
                /// <param name="func" type="Function">Function to be executed. If a context object is given inside the template, it will be used as a parameter.</param>
                funcs[name] = func;
            },

            getSubTemplate: function (name) {
                /// <summary>
                /// Gets the template with given name, stored inside this template.
                /// </summary>
                /// <param name="key"></param>
                /// <returns type="template"></returns>
                return templates[name];
            },

            run: function (context) {
                /// <summary>
                /// Runs the template with given context.
                /// </summary>
                /// <param name="context" type="Object"></param>
                /// <returns type="String"></returns>
                return _run(code, funcs, context);
            }
        };
    }

    var templaterPublicAPI = {
        compile: _compile
    };

    return templaterPublicAPI;

})();