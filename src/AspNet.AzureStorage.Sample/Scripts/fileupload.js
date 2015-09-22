var data = new FormData();
var fileObj = new Object;
var listCount = 0;

$('#uploadfield').on('change', function (e) {
    var files = e.target.files;
    if (files.length > 0) {
        processFileSelection(e, files)
    }
});

$('#uploadsubmit').on('click', function (e) {
    // Prevent sending the form
    event.preventDefault();

    // Add all non-standard properties of the file object to the form data
    for (var key in fileObj) {
        if (fileObj.hasOwnProperty(key)) {
            data.append(fileObj[key].name, fileObj[key]);
        }
    }

    // Send the ajax call
    $.ajax({
        type: "POST",
        url: '/File/UploadAjax',
        contentType: false,
        processData: false,
        data: data,
        success: function (result) {
            console.log(result);
            // Clear the file object and file list
            fileObj = new Object;
            $('table').html("");
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] == "{")
                err = JSON.parse(xhr.responseText).Message;
            console.log(err);
        }
    });
});

var deleteFileFromList = function(element) {
    var fileId = $(element).parents('tr').attr('id');
    $("#" + fileId).remove();
    delete fileObj[fileId];
}

var compareFilesForDuplicate = function(file1, file2) {
    return file1.size == file2.size && file1.lastModified == file2.lastModified;
}

var processFileSelection = function (event, files) {
    if (window.FormData !== undefined) {
        for (var x = 0; x < files.length; x++) {
            var counter = 0;
            // Compare files for duplicates
            for (var key in fileObj) {
                if (fileObj.hasOwnProperty(key) && compareFilesForDuplicate(fileObj[key], files[x])) {
                    counter++;
                }
            }

            // Only process the files when no duplicates are found
            if (counter < 1) {
                var fileKey = "file-" + listCount;
                listCount++;

                // Add the file to object as property
                fileObj[fileKey] = files[x];

                // Generate new table row for file
                var row = $('<tr></tr>', {
                    id: fileKey
                    });

                $('<td></td>').html('<img class="imageUploadPreview" src="' + URL.createObjectURL(files[x]) + '">').appendTo(row);
                $('<td></td>').html(files[x].name).appendTo(row);
                $('<td></td>').html('<button class="remove" onclick="deleteFileFromList(this)">X</button>').appendTo(row);

                row.appendTo('table');
            }
        }
        // Reset the form field so the upload button in empty again
        $(event.target).parent('form').trigger('reset');
    } else {
        alert('This browser doesn\'t support HTML5 file uploads!');
    }
}

