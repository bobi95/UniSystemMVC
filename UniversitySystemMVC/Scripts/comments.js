﻿$(document).ready(function() {
    $(function () {
        document.getElementById('submit-comment').addEventListener('click', function (e) {

            var serviceURL = '/Comment/CreateComment';

            var title = $("#Title").val();
            var content = $("#Content").val();
            var parentId = $("#parentId").val();

            $.ajax({
                type: "POST",
                url: serviceURL,
                data: "{articleId:'@Model.Id',userId:'@Model.UserId',title:'" + title + "',content:'" + content + "',userType:'@UniversitySystemMVC.Models.AuthenticationManager.UserType',parentId:'" + parentId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: successFunc,
                error: errorFunc
            });

            function successFunc(data, status) {
                var appendComment = "";
                if (parentId != null) {
                    appendComment += '<div style="display:none" class="comment-entry comment-entry-child" id="comment-' + data.Id + '">';
                }
                else {
                    appendComment += '<div style="display:none" class="comment-entry" id="comment-' + data.Id + '">';
                }
                appendComment += '<header>';
                appendComment += '<h5 class="pull-left"><span class="ui-icon ui-icon-comment pull-left"></span><strong>' + data.Title + '</strong></h5>';
                //appendComment += '<div class="pull-right">Edit | Delete</div>';
                appendComment += '</header>';
                appendComment += '<p class="comment-creator">Added by: ' + data.Name + '</p>';
                appendComment += '<p class="comment-date">Created: ' + data.DateCreated + '</p>';
                appendComment += '<p class="comment-date">Last modified: ' + data.DateModified + '</p>';
                appendComment += '<p class="comment-content">' + data.Content + '</p>';
                appendComment += '</div>';

                if (parentId != null) {
                    $("#comment-" + data.ParentId).append(appendComment)
                }
                else {
                    $('#comments').append(appendComment);
                }


                $('#comment-' + data.Id).slideDown();

                $("#Title").val('');
                $("#Content").val('');
            }

            function errorFunc() {
                alert('error');
            }
        });

        var editBtns = document.getElementsByClassName('btn-edit-comment');

        var showEditView = function () {
            var commentId = this.getAttribute("data-comment");

            var commentContent = $("#comment-" + commentId + " .comment-content").html();
            var titleContent = $("#title-comment-" + commentId).html();

            var appendedTitleTextBox = '<div id="edit-title-' + commentId + '">';
            appendedTitleTextBox += '<input type="text" id="Title" name="Title" value="' + titleContent + '" />';
            appendedTitleTextBox += '</div>';
            $("#comment-" + commentId + " header").hide();
            $("#comment-" + commentId + " header").after(appendedTitleTextBox);

            var appendedContentTextbox = '<div id="edit-comment-' + commentId + '-container">';
            appendedContentTextbox += '<textarea class="form-control" cols="20" id="Content" name="Content" rows="2">' + commentContent + '</textarea>';
            appendedContentTextbox += '<input type="hidden" name="Id" value="' + commentId + '" />';
            appendedContentTextbox += '</div>';

            $("#comment-" + commentId + " .comment-content").after(appendedContentTextbox);

            var appendedEditBtn = '<button class="btn btn-edit-submit" id="edit-comment-' + commentId + '" data-comment="' + commentId + '">Edit</button>';
            $("#comment-" + commentId + " #Content").after(appendedEditBtn);

            $("#comment-" + commentId + " .comment-content").hide();

            document.getElementById('edit-comment-' + commentId).addEventListener('click', editComment, false);
        };

        for (var i = 0; i < editBtns.length; i++) {
            editBtns[i].addEventListener('click', showEditView, false);
        }

        function editComment() {
            var commentId = this.getAttribute("data-comment");

            var serviceURL = '/Comment/EditComment';

            var content = $("#edit-comment-" + commentId + "-container #Content").val();
            var title = $("#comment-" + commentId + " #Title").val();

            if (title.length > 3 && content.length > 3) {
                $.ajax({
                    type: "POST",
                    url: serviceURL,
                    data: "{commentId:'" + commentId + "',title:'" + title + "',content:'" + content + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: successFunc,
                    error: errorFunc
                });

                function successFunc(data, status) {
                    $("#edit-comment-" + commentId + "-container").hide();
                    $("#edit-title-" + commentId).hide();
                    $("#comment-" + commentId + " header").show();
                    $("#title-comment-" + commentId).html(title);
                    $("#comment-" + commentId + " .comment-content").html(content);
                    $("#comment-" + commentId + " .comment-content").show();
                }

                function errorFunc() {
                    alert('error');
                }
            }
            else {
                alert('Title and Content of the comment must be at least 3 symbols ... fix this ugly alert later!');
            }
        };

        // Deleting Comments
        var deleteBtns = document.getElementsByClassName('btn-delete-comment');

        function deleteComment() {
            var commentId = this.getAttribute("data-comment");

            var serviceURL = '/Comment/DeleteComment';

            $.ajax({
                type: "POST",
                url: serviceURL,
                data: "{commentId:'" + commentId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: successFunc,
                error: errorFunc
            });

            function successFunc(data, status) {
                $("#comment-" + commentId).remove();
            }

            function errorFunc() {
                alert('error');
            }
        }

        for (var i = 0; i < deleteBtns.length; i++) {
            deleteBtns[i].addEventListener('click', deleteComment, false);
        }

        // Replying Comments
        var replyBtns = document.getElementsByClassName('btn-reply-comment');

        function showReplyView() {
            $("#parentId").remove();

            // to make it good

            var parentId = this.getAttribute("data-comment");

            $('#add-comment-toggle').click();
            setTimeout( function() {
                document.getElementById("comment-add-"+@Model.Id).scrollIntoView();
            }, 800 );

            var hiddenParent = '<input type="hidden" name="parentId" id="parentId" value="' + parentId + '"/>';
            $("#comment-add-"+@Model.Id + " form").append(hiddenParent);

        }

        for (var i = 0; i < replyBtns.length; i++) {
            replyBtns[i].addEventListener('click', showReplyView, false);
        }
    });

});
