var data = new FormData();
var fileObj = new Object;
var listCount = 0;

var deleteFileFromList = function(element) {
    var fileId = $(element).parent('li').attr('id');
    $("#" + fileId).remove();
    delete fileObj[fileId];
}

$('#uploadfield').on('change', function (e) {
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            for (var x = 0; x < files.length; x++) {
                var fileKey = "file-" + listCount;
                listCount++;

                // Add the file to object as property
                fileObj[fileKey] = files[x];

                $('<li></li>', {
                    id: fileKey
                }).html(files[x].name + '<button class="remove" onclick="deleteFileFromList(this)">X</button>')
                    .appendTo('#Filelist');
            }
            $(e.target).parent('form').trigger('reset');
        } else {
            alert('This browser doesn\'t support HTML5 file uploads!');
        }
    }
});


$('#uploadsubmit').on('click', function (e) {
    event.preventDefault();

    for (var key in fileObj) {
        if (fileObj.hasOwnProperty(key)) {
            data.append(fileObj[key].name, fileObj[key]);
        }
    }

    $.ajax({
        type: "POST",
        url: '/File/UploadAjax',
        contentType: false,
        processData: false,
        data: data,
        success: function (result) {
            console.log(result);
            fileObj = new Object;
            $('#Filelist').html("");
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
        }
    });
});