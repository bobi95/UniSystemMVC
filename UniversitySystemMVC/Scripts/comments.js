$(document).ready(function () {

    // Adding a comment
    document.getElementById('submit-comment').addEventListener('click', function (e) {

        var articleId = this.getAttribute("data-articleId");
        var title = $("#Title").val();
        var content = $("#Content").val();
        var parentId = $("#parentId").val();

        var serviceURL = '/Comment/CreateComment';

        if (title.length > 3 && content.length > 3) {
            $.ajax({
                type: "POST",
                url: serviceURL,
                data: "{articleId:'" + articleId + "',title:'" + title + "',content:'" + content + "',parentId:'" + parentId + "'}",
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
                    appendComment += '<p class="comment-date">Created: just now</p>';
                    appendComment += '<p class="comment-date">Last modified: just now</p>';
                    appendComment += '<p class="comment-content">' + data.Content + '</p>';
                    appendComment += '</div>';

                if (parentId != null) {
                    $("#comment-" + data.ParentId).append(appendComment)
                    setTimeout(function () {
                        document.getElementById("comment-" + data.Id).scrollIntoView();
                    }, 400);
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
        }
        else {
            alert('Title and Content of the comment must be at least 3 symbols ... fix this ugly alert later!');
        }
        
    });

    // Showing Edit Views Main Comments
    var editBtns = document.getElementsByClassName('btn-edit-comment');

    var showEditView = function () {
        var commentId = this.getAttribute("data-comment");
        var parentId = this.getAttribute("data-parent");

        
        var commentContent = $("#comment-" + commentId + " .comment-content").html();
        var titleContent = $("#title-comment-" + commentId).html();

        var appendedTitleTextBox = '<div id="edit-title-' + commentId + '">';
            appendedTitleTextBox += '<input type="text" id="Title" name="Title" value="' + titleContent + '" />';
            appendedTitleTextBox += '</div>';
        

        var appendedContentTextbox = '<div id="edit-comment-' + commentId + '-container">';
            appendedContentTextbox += '<textarea class="form-control" cols="20" id="Content" name="Content" rows="2">' + commentContent + '</textarea>';
            appendedContentTextbox += '<input type="hidden" name="Id" value="' + commentId + '" />';
            appendedContentTextbox += '</div>';

        var appendedEditBtn = '<button class="btn btn-edit-submit" id="edit-comment-' + commentId + '" data-comment="' + commentId + '">Edit</button>';

        if (parentId == null) {
            $("#comment-" + commentId + ".comment-entry-main > header").hide();
            $("#comment-" + commentId + ".comment-entry-main > header").after(appendedTitleTextBox);
            $("#comment-" + commentId + ".comment-entry-main > .comment-content").after(appendedContentTextbox);
            $("#comment-" + commentId + ".comment-entry-main #Content").after(appendedEditBtn);
            $("#comment-" + commentId + ".comment-entry-main > .comment-content").hide();
        }
        else {
            $("#comment-" + commentId + ".comment-entry-child > header").hide();
            $("#comment-" + commentId + ".comment-entry-child > header").after(appendedTitleTextBox);
            $("#comment-" + commentId + ".comment-entry-child > .comment-content").after(appendedContentTextbox);
            $("#comment-" + commentId + ".comment-entry-child #Content").after(appendedEditBtn);
            $("#comment-" + commentId + ".comment-entry-child > .comment-content").hide();
        }
        
        document.getElementById('edit-comment-' + commentId).addEventListener('click', editComment, false);
    };

    for (var i = 0; i < editBtns.length; i++) {
        editBtns[i].addEventListener('click', showEditView, false);
    }

    // EditPerform
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

    
    // Replying Comments
    var replyBtns = document.getElementsByClassName('btn-reply-comment');

    function showReplyView() {
        $("#parentId").remove();

        var parentId = this.getAttribute("data-comment");
        var articleId = this.getAttribute("data-articleId");
        console.log(articleId);
        $('#add-comment-toggle').click();
        setTimeout(function () {
            console.log($("#comment-add-" + articleId));
            document.getElementById("comment-add-" + articleId).scrollIntoView();
        }, 800);
        console.log(articleId);
        var hiddenParent = '<input type="hidden" name="parentId" id="parentId" value="' + parentId + '"/>';
        $("#comment-add-" + articleId + " form").append(hiddenParent);

    }

    for (var i = 0; i < replyBtns.length; i++) {
        replyBtns[i].addEventListener('click', showReplyView, false);
    }
});