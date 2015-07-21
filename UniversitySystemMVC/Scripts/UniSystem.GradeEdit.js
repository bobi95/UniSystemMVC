var UniSystem = UniSystem || {};

UniSystem.GradeEdit = (function () {
    
    var containers = [],

        settings = {
            containerClass: 'grade-crud',
            studentIdAttribute: 'grade-student',
            subjectIdAttribute: 'grade-subject', 
            btnAddClass: 'add-grade-btn',
            btnCancelClass: 'remove-grade-btn',
            inputClass: 'add-grade-input'
        };

    var publicAPI = {

        setSettings: function(newSettings) {
            settings = newSettings;
        },

        addEvents: function () {

            var containers = document.querySelectorAll(settings.containerClass);
            
            for (var key in containers) {

                var addBtn = containers[key].querySelector(settings.btnAddClass),
                    cancelBtn = containers[key].querySelector(settings.btnCancelClass),
                    inputClass = containers[key].querySelector(settings.inputClass);

                // TODO: add event habdlers
            }

        }
        
    };

    return publicAPI;

})();