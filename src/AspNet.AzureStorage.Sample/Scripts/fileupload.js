var data = new FormData();
var fileArr = [];

$('#uploadfield').on('change', function (e) {
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            for (var x = 0; x < files.length; x++) {
                fileArr.push(files[x]);
            }
            $(e.target).parent('form').trigger('reset');
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
});

$('#uploadsubmit').on('click', function (e) {
    event.preventDefault();

    for (var x = 0; x < fileArr.length; x++) {
        data.append(fileArr[x].name, fileArr[x]);
    }

    $.ajax({
        type: "POST",
        url: '/File/UploadAjax',
        contentType: false,
        processData: false,
        data: data,
        success: function (result) {
            console.log(result);
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
        }
    });
    filearr = [];
});