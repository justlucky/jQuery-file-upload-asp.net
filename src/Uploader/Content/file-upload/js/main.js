/*jslint nomen: true, unparam: true, regexp: true */
/*global $, window, document */
$(function () {

    'use strict';

    var api = 'api/upload';

    //
    // Initialize the jQuery File Upload widget
    //
    $('#fileupload').fileupload({
        url: api,
        dropZone: $('#dropzone'),
        sequentialUploads: true
    });

    //
    // Enable iframe cross-domain access via redirect option
    //
    $('#fileupload').fileupload(
                    'option',
                    'redirect',
                    window.location.href.replace(/\/[^\/]*$/, '/cors/result.html?%s')
            );

    //
    // Load existing files
    //
    $('#fileupload').each(function () {
        var that = this;
        $.getJSON(api, function (result) {
            if (result && result.length) {
                $(that).fileupload('option', 'done')
                        .call(that, null, { result: result });
            }
        });
    });

    //
    // Callbacks (if you want to use them refer to the documentation)
    //
    $('#fileupload')
            .bind('fileuploadadd', function (e, data) { /* ... */ })
            .bind('fileuploadsubmit', function (e, data) { /* ... */ })
            .bind('fileuploadsend', function (e, data) { /* ... */ })
            .bind('fileuploaddone', function (e, data) { /* ... */ })
            .bind('fileuploadfail', function (e, data) { /* ... */ })
            .bind('fileuploadalways', function (e, data) { /* ... */ })
            .bind('fileuploadprogress', function (e, data) { /* ... */ })
            .bind('fileuploadprogressall', function (e, data) { /* ... */ })
            .bind('fileuploadstart', function (e) { /* ... */ })
            .bind('fileuploadstop', function (e) { /* ... */ })
            .bind('fileuploadchange', function (e, data) { /* ... */ })
            .bind('fileuploadpaste', function (e, data) { /* ... */ })
            .bind('fileuploaddrop', function (e, data) { /* ... */ })
            .bind('fileuploaddragover', function (e) { /* ... */ });
});

//
//  dragover event
//
$(document).bind('dragover', function (e) {
    var dropZone = $('#dropzone'), timeout = window.dropZoneTimeout;
    if (!timeout) {
        dropZone.addClass('in');
    } else {
        clearTimeout(timeout);
    }
    if (e.target === dropZone[0]) {
        dropZone.addClass('thumbnail hover');
    } else {
        dropZone.removeClass('hover');
    }
    window.dropZoneTimeout = setTimeout(function () {
        window.dropZoneTimeout = null;
        dropZone.removeClass('in hover');
    }, 100);
});