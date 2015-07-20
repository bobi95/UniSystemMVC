var UniSystem = UniSystem || {};

UniSystem.DnD = (function () {

    var dataName = 'UniSystem.DnD',
        useDataTransfer = false,
        dataCollection = {};

    var publicAPI = {
        setData: function (key, data) {
            /// <summary>
            /// Sets the given data to the inner Data Collection with given key.
            /// </summary>
            /// <param name="key" type="String">Key</param>
            /// <param name="value" type="Object">Data</param>
            dataCollection[key] = value;
        },

        getData: function (key) {
            /// <summary>
            /// Gets the data from the inner Data Collection, corresponding to the given key.
            /// </summary>
            /// <param name="key" type="String">Key</param>
            return dataCollection[key];
        },

        unsetData: function (key) {
            /// <summary>
            /// Unsets the data from the inner DataCollection.
            /// </summary>
            /// <param name="key" type="String">Key</param>
            dataCollection[key] = undefined;
        },

        useDataTransfer: function (value) {
            /// <summary>
            /// Sets the script to use DataTransfer or the inner Data Collection.
            /// You can attach any kind of object to the inner Data Collection.
            /// </summary>
            /// <param name="value" type="Boolean"></param>
            useDataTransfer = value === true ? true : false;
        },

        setDataTransferName: function (name) {
            /// <summary>Set the unique key to use to get the data from DataTransfer. Only use if conflict occurs with other scripts.</summary>
            /// <param name="name" type="String">Unique key to use to get the data from DataTransfer.</param>
            dataName = name
        },

        setDragable: function (id, dataOfItem) {
            /// <summary>
            /// Sets up the DOM object with given id as a draggable object
            /// </summary>
            /// <param name="id" type="String">Id of DOM object</param>
            /// <param name="dataOfItem" type="Object">Data to be given to the drop object callback function.</param>
            var item = document.getElementById(id);

            if (item.hasAttribute('draggable')) {
                if (item.getAttribute('draggable') === 'true') {
                    return;
                }
            }

            item.setAttribute('graggable', 'true');
            item.ondragstart = function (ev) {
                if (useDataTransfer) {
                    ev.dataTransfer.setData(dataName, dataOfItem);
                } else {
                    dataCollection[id] = dataOfItem;
                }
            }
        },

        setDroppable: function (id, useDataCallback) {
            /// <summary>
            /// Sets up the DOM object with given id as a droppable object
            /// </summary>
            /// <param name="id" type="String">Id of DOM object</param>
            /// <param name="useDataCallback" type="Function">Function to get the data of the dropped item.</param>
            var item = document.getElementById(id);

            item.ondragover = function (ev) {
                ev.preventDefault();
            }

            item.ondrop = function (ev) {

                ev.preventDefault();

                var data;

                if (useDataTransfer) {

                    data = DataTransfer.getData(dataName);

                } else {

                    data = dataCollection[id];

                }

                useDataCallback(data);
            }
        }
    };

    return publicAPI;
})();