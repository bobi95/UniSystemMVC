/**
 * An API for creating and executing templates.
 *
 * The Templater can compile html code into templates.
 * The templates can be run multiple times with different
 * contexts (ViewModels).
 * These templates can echo text from the VM, run functions
 * which return text to be included into the result of the template,
 * make a partial template (sub-template) cycle multiple times, using
 * the VM or a part of it or just contain a partial template, which
 * can be retrieved at will from the template object.
 *
 * Template Code:
 * {{variable_name}} - Echoes a member of the VM with the required
 *                     name. 'this' can be used to reference
 *                     the VM of the executing template.
 *
 * {{#function_name [context_object]}}
 *                   - Will look for and run a function with the
 *                     required name. A context object may be
 *                     specified to be given to the function as a
 *                     parameter.
 *
 * {{#template_name#}}
 *                   - Create a plain sub-template with given name.
 *                     Enclose the desired templat html with
 *                     {{/#template-name#}}.
 *                     Warning: Be careful when creating 'trees' with
 *                     templates. If template A is opened before B,
 *                     always close B before closing A.
 *
 * {{#template_name# context_object_VM [number / context_object]}}
 *                   - Create a template cycle, which executes the
 *                     specified template number/context_object
 *                     number ot times or ONCE if no
 *                     number/context_object is specified.
 *                     The template is going to use the specified
 *                     context_object_VM as a VM.
 *
 *
 *
 * Example:
 *
 * Given HTML:
 * <p>
 *     This is the {{template_name}} template. <br>
 *     It's sub-template is going to execute {{number}} number of times:<br>
 * {{#SubTemplate# this.subtemplate this.number}}
 *     Executed: {{#MyFunction this}}<br>
 * {{/#SubTemplate#}}
 * </p>
 *
 * Javascript:
 * var template = Templater.compile(html);
 * 
 * template.registerFunction('MyFunction', function (context) {
 *    var result = context.time;
 *    context.time++;
 *    return result;
 * });
 * 
 * var vm = {
 *    template_name: "My Template",
 *    number: 10,
 *    subtemplate: {
 *        time: 1
 *    }
 * };
 * var result = Templater.run(template, vm);
 * 
 * document.getElementsByTagName('body')[0].innerHTML = result;
 *
 * Result:
 *
 * This is the My Template template. 
 * It's sub-template is going to execute 10 number of times:
 * Executed: 1
 * Executed: 2
 * Executed: 3
 * Executed: 4
 * Executed: 5
 * Executed: 6
 * Executed: 7
 * Executed: 8
 * Executed: 9
 * Executed: 10
 *
 */

/**
 * An API for creating and executing templates.
 * @type {Object}
 */
var Templater = (function() {

    /**
     * Collection of handlers for items from a template's run collection.
     * @type {Object}
     */
    var objectHandlers = {

        echo: function (item, context) {
            item.context = context;
            return _echo(item);
        },

        func: function (item, context, template) {
            item.context = context;
            return _func(item, template);
        },

        templateCycle: function(item, context, template) {
            item.context = context;
            return _templateCycle(item, template);
        }
    };

    /**
     * Handler for the 'echo' action type
     * @param  {Object} item 'echo' item from a template's run collection
     * @return {String}      Result of the 'echo' action
     */
    function _echo (item) {
        return _getItemFromObject(item.path, item.context);
    }

    /**
     * Handler for the 'func' action type. Executes a function in a template
     * @param  {Object}   item     'func' item from a template's run collection
     * @param  {Template} template Executing template
     * @return {String}            Result of the executed function
     */
    function _func (item, template) {
        
        if (template.funcs[item.name]) {
            return template.funcs[item.name](_getItemFromObject(item.path, item.context));
        }

        return '';
    }

    /**
     * Hander for the 'templateCycle' action type.
     * Runs a subtemplate the specified number of times.
     * If no times is given, runs once.
     * @param  {Object}   item     'templateCycle' item from a template's run collection
     * @param  {Template} template Executing template
     * @return {String}            Result of the template's execution
     */
    function _templateCycle (item, template) {
        
        if (template.subTemplates[item.name]) {

            var sub = template.subTemplates[item.name],
                templateContext = _getItemFromObject(item.path, item.context),
                times = item.times,
                result = [];

            if (typeof times !== 'number') {
                times = Number(_getItemFromObject(item.times, item.context));
                if (!times) {
                    throw new Error('Template cycle needs a number to be able to cycle');
                }
            }

            for (var i = 0; i < times; i++) {
                result.push(Templater.run(sub, templateContext));
            }

            return result.join('');
        }

        return '';
    }

    /*
     * Expression construction
     */

    var expressionsRAW = {
        findCode: '(.+?(?:{{[^{}]*}}))',
        templateCodeRaw: '[^{}]+',
        nameRaw: '[a-z_][a-z0-9_]*',
        funcPrefix: '#',
    };

    expressionsRAW.objRaw = expressionsRAW.nameRaw + '(?:\\.' + expressionsRAW.nameRaw + ')*';
    // [a-z_][a-z0-9_]*(?:\\.[a-z_][a-z0-9_]*)*

    expressionsRAW.func = expressionsRAW.funcPrefix + expressionsRAW.nameRaw;
    // #[a-z_][a-z0-9_]*

    expressionsRAW.funcCapture = expressionsRAW.funcPrefix + '(' + expressionsRAW.nameRaw + ')';
    // #([a-z_][a-z0-9_]*)

    expressionsRAW.partial =
        expressionsRAW.funcPrefix + '(' + expressionsRAW.nameRaw + ')' + expressionsRAW.funcPrefix + //partial's name
        '(?:\\s+(' + expressionsRAW.objRaw + ')(?:\\s+((?:\\d+)|(?:' + expressionsRAW.objRaw + ')))?)?';
    // ^{{#([a-z_][a-z0-9_]*)#(?:\s+([a-z_][a-z0-9_]*(?:\.[a-z_][a-z0-9_]*)*)(?:\s+((?:\d+)|(?:[a-z_][a-z0-9_]*(?:\.[a-z_][a-z0-9_]*)*)))?)?}}$

    expressionsRAW.closePartial = function (name) {
        return '\\/' + expressionsRAW.funcPrefix + '(' + name + ')' + expressionsRAW.funcPrefix;
    };
    // #(name)#

    /**
     * The expressions used to capture template code from the given html
     * @type {Object}
     */
    var expressions = {
        findCode: new RegExp(expressionsRAW.findCode, 'i'),
        templateCode: new RegExp('({{' + expressionsRAW.templateCodeRaw + '}})', 'g'),
        func: new RegExp('^{{' + expressionsRAW.funcCapture + '(?:\\s+(' + expressionsRAW.objRaw + '))?}}$', 'i'),
        // ^{{#([a-z_][a-z0-9_]*)(?:\\s+((?:[a-z_][a-z0-9_.]*[a-z0-9_])|(?:[a-z_][a-z0-9_]*)))?}}$
        obj: new RegExp('^{{(' + expressionsRAW.objRaw + ')}}$', 'i'),
        partial: new RegExp('^{{' + expressionsRAW.partial + '}}$', 'i'),
        closePartial: function (name) {
            return new RegExp('{{' + expressionsRAW.closePartial(name) + '}}', 'i');
        }
    };

    /**
     * Compiles the given html to a Template object
     * @param  {String}   html Html to be compiled
     * @return {Template}      Result of compilation
     */
    function _compile(html) {
        var templateCode = [],
            funcs = {},
            partials = {};

        /**
         * Processes the captured code from the html and returns the appropriate object.
         * @param  {String} expr Captured code
         * @return {Object}      Object, describing the captured code
         */
        function _processCode(expr) {

            // match a function
            var match = expr.match(expressions.func);

            if (match) {

                html = html.substr(expr.length);

                var funcContext = match[2];

                if (funcContext) {
                    funcContext = funcContext.split('.');
                }

                return {
                    type: 'func',
                    name: match[1],
                    path: funcContext
                };

            }

            // match a context object
            match = expr.match(expressions.obj);

            if (match) {

                html = html.substr(expr.length);

                return {
                    type: 'echo',
                    path: match[1].split('.')
                };

            }

            // match a subtemplate
            match = expr.match(expressions.partial);

            if (match) {

                var closeTagExpression = expressions.closePartial(match[1]);

                var closingTagIndex = html.search(closeTagExpression);

                var newHTML = html.substring(match[0].length, closingTagIndex);

                html = html.substring(closingTagIndex + html.match(closeTagExpression)[0].length);

                var partial = _compile(newHTML);

                // handle a template cycle
                if (match[2] || match[3]) {

                    var times = Number(match[3] || 1);

                    if (!times) {

                        times = match[3].split('.');

                    }

                    return {
                        type: 'templateCycle',
                        name: match[1],
                        template: partial,
                        path: match[2].split('.'),
                        times: times
                    };

                }

                // handle a plain template
                return {
                    type: 'partial',
                    name: match[1],
                    template: partial
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

                if (codeResult.type === 'templateCycle') {

                    templateCode.push({
                        type: 'templateCycle',
                        name: codeResult.name,
                        path: codeResult.path,
                        times: codeResult.times
                    });

                    partials[codeResult.name] = codeResult.template;
                    continue;
                }

                if (codeResult.type === 'partial') {

                    partials[codeResult.name] = codeResult.template;
                    continue;

                }

                templateCode.push(codeResult);
                continue;

            }

            break;
        }

        return new Template(templateCode, funcs, partials);
    }

    /**
     * Uses the required handler according to the item and returns its result.
     * @param  {Object}   item      Item from a template's run collection.
     * @param  {Object}   context   Context / View Model for the action
     * @param  {Template} template  The executing template
     * @return {[type]}             Result of the action
     */
    function _getResult (item, context, template) {

        //console.log(scope.runCollection);

        if (typeof item === 'string') {
            return item;
        }

       if (typeof item === 'object' && objectHandlers[item.type]) {
            return objectHandlers[item.type](item, context, template);
       }

    }

    /**
     * Template Class
     * @param {Array} runCollection The workflow of this template.
     * @param {Array} funcs         Collections of functions, that could be used in the template.
     * @param {Array} subTemplates  Inner templates.
     */
    var Template = function (runCollection, funcs, subTemplates) {

        /**
         * The workflow of the template
         * @type {Array}
         */
        this.runCollection = runCollection || [];

        /**
         * Funcstions for the template
         * @type {Array}
         */
        this.funcs = funcs || {};

        /**
         * Inner templates
         * @type {Array}
         */
        this.subTemplates = subTemplates || {};
    };

    /**
     * Register a function for the template. If the template doesn't use a
     * function with given name, it will try to register it for a sub-template.
     * @param  {String}   name Name of function in the template
     * @param  {Function} func Function to be registered
     * @return {Bool}          Specifies the success of the function registration.
     */
    Template.prototype.registerFunction = function(name, func) {

        for (var i = 0, n = this.runCollection.length; i < n; i++) {
            
            if (typeof this.runCollection[i] === 'object' &&
                this.runCollection[i].type === 'func' &&
                this.runCollection[i].name === name) {

                this.funcs[name] = func;

                return true;
            }
        }

        for (var key in this.subTemplates) {
            if (this.subTemplates[key].registerFunction(name, func)) {
                return true;
            }
        }

        // for (i = 0, n = this.subTemplates.length; i < n; i++) {
        //     if (this.subTemplates[i].registerFunction(name, func)) {
        //         return true;
        //     }
        // }

        return false;
    };

    /**
     * Retrieve a partial template
     * @param  {String}   name Name of template
     * @return {Template}      Wanted template
     */
    Template.prototype.getPartial = function(name) {

        if (this.subTemplates[name]) {
            return this.subTemplates[name];
        }

        return null;
    };

    /**
     * Gets the value stored at the path in the given object.
     * @param  {Array}  path Array of strings / integers to follow.
     * @param  {Object} obj  Object to explore.
     * @return {Mixed}       Result of search.
     */
    function _getItemFromObject(path, obj) {

        var result = obj;

        path.forEach(function (name) {
            if (name !== 'this') {
                result = result[name];
            }
        });

        return result;
    }



    var publicAPI = {
        compile: function(html) {
            return _compile(html);
        },
        run: function(template, context) {
            var result = [];

            //console.log(template.runCollection);

            template.runCollection.forEach(function (item) {

                result.push(_getResult(item, context, template));

            });

            return result.join('');
        },
        demo: function() {
            var html =
            '<p>This is the {{template_name}} template. <br>' +
            'Its sub-template is going to execute {{number}} number of times:<br>' +
            '{{#SubTemplate# this.subtemplate this.number}}' +
            'Executed: {{#MyFunction this}}<br>' +
            '{{/#SubTemplate#}}</p>';
 
            var template = Templater.compile(html);

            template.registerFunction('MyFunction', function (context) {
                var result = context.time;
                context.time++;
                return result;
            });

            var vm = {
                template_name: "My Template",
                number: 10,
                subtemplate: {
                    time: 1
                }
            };

            var result = Templater.run(template, vm);

            document.getElementsByTagName('body')[0].innerHTML = result;
       }
    };

    return publicAPI;

})();