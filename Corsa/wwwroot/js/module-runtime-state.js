var RuntimeStateModule = ( function (url, bar ,  delay) {

    var config = {
        barSelector: bar,
        period: delay, 
        urlRequest: url,        
    }

    var breakCheckPoint = false;
    var moduleRuntimeState = 0;
    
    function stop() {
        $(config.barSelector).hide();
        breakCheckPoint = true;
    }

    function start(initState) {        
        breakCheckPoint = false;

        if (initState != null) {
            syncUIState(initState.state, moduleRuntimeState, initState.message, initState.lastLogMessage);
        }
        
        updateModuleRuntimeState()
    }

    function showMessage(message) {
        $(config.barSelector).find("#moduleRuntimeStateMessage").text(message);
        $(config.barSelector).show();
    }

    function showLastLogMessage(message) {
        $(config.barSelector).find("#moduleRuntimeLastLogMessage").text(message);
        $(config.barSelector).show();
    }

    function hideLastLogMessage(message) {
        $(config.barSelector).find("#moduleRuntimeLastLogMessage").text("");
        $(config.barSelector).hide();
    }

    function hideMessage() {
        $(config.barSelector).find("#moduleRuntimeStateMessage").text("");
        $(config.barSelector).hide();
    }

    function syncUIState(newSate, oldState, message, lastLogMessage) {

        switch (newSate) {
            case 0: {
                hideMessage();
                hideLastLogMessage();
                if (oldState == 1 || oldState == 2) {
                    $(facade).trigger("completed");
                }

            } break;

            case 1: {                
                showMessage(message);
                showLastLogMessage(lastLogMessage);
            } break;

            case 2: {
                showMessage(message);
                showLastLogMessage(lastLogMessage);
            } break;

            case 3: {
                hideMessage();
                hideLastLogMessage();
                $(facade).trigger("completed");
            } break;
        }        
    }

    function forceLoop() {
        return !breakCheckPoint && ( moduleRuntimeState == 1 || moduleRuntimeState == 2)
    }

    function updateModuleRuntimeState() {

        $.getJSON(config.urlRequest,
            (function (data) {
                
                if (('state' in data) && ('message' in data)) {
                    syncUIState(data.state, moduleRuntimeState, data.message, data.lastLogMessage);
                    moduleRuntimeState = data.state;
                }
            }))

            .fail(function () {
                console.log("error");
            })
            .always(function () {

                if (forceLoop()) {
                    setTimeout(updateModuleRuntimeState, config.period);
                }
            });
    }

    //function updateContent() {

    //    var request = $.ajax({
    //        url: config.url,
    //        method: "POST",
    //        dataType: 'html',
    //    });

    //    request.always(function (msg) {
    //        if (!breakCheckPoint) {
    //            $('#requestStatistics').replaceWith(data);
    //        }
    //    });

    //    $.ajax({
    //        url: url,
    //        type: 'GET',
    //        dataType: 'html',

    //        success: function (data) {
    //            NProgress.done();
    //            $('#requestStatistics').replaceWith(data);
    //        }
    //    });
    //}

    var facade = {
        start: start,
        stop: stop,
        showMessage: showMessage,
        hideMessage: hideMessage,
        state: moduleRuntimeState
    };

    return facade;
}
)



