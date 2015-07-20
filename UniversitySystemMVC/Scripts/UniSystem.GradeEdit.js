var UniSystem = UniSystem || {};

UniSystem.GradeEdit = (function () {

    /*
     * These settigns are used by the form creating function of this object.
     * Settings members:
     *  - attributes: uses the given DOM object's attributes in the post request
     *      Array of Strings
     * 
     *  - url: url for the post request
     *      String
     * 
     *  - domObject: generated dom object's classes, attributes and events
     *          
     *  - TODO: more settings
     */

    var settings = {
        attributes: [],
        url: '',
        domObject: {
            tagName: 'button',
            classes: [],
            attributes: {},
            events: {}
        }
    };

    var publicAPI = {

        setSettings: function (values) {
            settings = values || {};
        },

        formCreate: function (item) {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item" domElement="true">DOM element with data attributes.</param>

            var data = {};

            for (var key in settings.attributes) {
                if (item.hasAttribute(settings.attributes[key])) {

                    data[settings.attributes[key]] = item.getAttribute(settings.attributes[key]);

                }
            }
            
            var obj = document.createElement(settings.domObject.tagName);
            for (var key in settings.domObject.classes) {
                obj.classList.add(settings.domObject.classes[key]);
            }


        }
    };

    return publicAPI;

})();