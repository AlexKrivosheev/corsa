var ResultDetailsModule = (function (modalWindow, title, content, progress, info) {

    var config = {
        modalWindow: modalWindow,        
        content: content,
        title: title,
        progressBar: progress,
        infoPanel: info                    
    }

    function showProgressBar() {
        var progressBar = $(config.progressBar);
        progressBar.show();
    }

    function hideProgressBar() {
        var progressBar = $(config.progressBar);
        progressBar.hide();
    }

    function openModalWindow() {
        var modalWindow = $(config.modalWindow);
        modalWindow.modal('show');
    }

    function setDescription(text) {
        $(config.infoPanel).text(text); 
    }

    function updateContent(data) {
        var targetContent = $(config.content);
        targetContent.html(data);
    }

    function updateTitle(text) {
        var targetTitle = $(config.title);
        targetTitle.text(text);
    }

    function open(title, urlRequest) {
        updateContent("");
        updateTitle(title);
        openModalWindow();
        showProgressBar();
        
        var request = $.ajax({
            url: urlRequest,
            method: "GET",
            dataType: "html"
        });

        request.done(function (data) {
            updateContent(data);
            hideProgressBar();
        });

        request.fail(function (jqXHR, textStatus) {            
            setDescription("Request failed: " + textStatus);   
            hideProgressBar();
        });

    }




    var facade = {
        open: open        
    };

    return facade;
}
)



