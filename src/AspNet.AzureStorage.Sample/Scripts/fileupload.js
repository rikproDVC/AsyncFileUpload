var data = new FormData();
var fileObj = new Object;

var deleteFileFromList = function(element) {
    var fileId = $(element).parent('li').attr('id');
    $("#" + fileId).slideUp();
    delete fileObj[fileId];
}

$('#uploadfield').on('change', function (e) {
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            for (var x = 0; x < files.length; x++) {
                //fileArr.push(files[x]);
                //var last = fileArr[filearr.length - 1];
                //$('#Filelist').append("<li id='file-" + last.name + "'>" + files[x].name + "<button id='remove'>X</button>");
                //$("#remove").click(function () {
                //    $("#filename").slideUp();
                //});

                var fileKey = "file-" + Object.keys(fileObj).length

                // Add the file to object as property
                fileObj[fileKey] = files[x];

                $('#Filelist').append("<li id='" + fileKey + "'>" + files[x].name + "<button class='remove' onclick='deleteFileFromList(this)'>X</button></li>");
            }
            $(e.target).parent('form').trigger('reset');
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
});


$('#uploadsubmit').on('click', function (e) {
    event.preventDefault();

    for (var key in fileObj) {
        if (fileObj.hasOwnProperty(key)) {
            data.append(fileObj[key].name, fileObj[key]);
            delete fileObj[key];
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
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
        }
    });
});