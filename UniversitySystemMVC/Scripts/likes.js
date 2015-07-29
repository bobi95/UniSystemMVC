$(document).ready(function () {
    var likeBtns = document.getElementsByClassName('button-like');

    function myFunc() {
        var articleId = this.getAttribute("data-article");
        var state = this.getAttribute("data-state");
        var count = this.getAttribute("data-likesCount");

        if (state == 0) {

            var serviceURL = '/Like/AddLike';

            $.ajax({
                type: "POST",
                url: serviceURL,
                data: "{articleId:'" + articleId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: addSuccessFunc,
                error: addErrorFunc
            });

            function addSuccessFunc(data, status) {
                $('#like-subject-' + articleId + '-info').html("You and <a href='#' data-toggle='modal' data-target='#likes'>" + count + " more</a> like this");
                $('#like-subject-' + articleId + ' img').attr("src", "/Content/images/unlike.png");
                $('#like-subject-' + articleId).attr("data-state", "1");
                $('#like-subject-' + articleId).attr("data-likesCount", parseInt(count) + 1);
            }

            function addErrorFunc() {
                alert('error');
            }
        }
        else {

            var serviceURL = '/Like/DeleteLike';

            $.ajax({
                type: "POST",
                url: serviceURL,
                data: "{articleId:'" + articleId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: removeSuccessFunc,
                error: removeErrorFunc
            });

            function removeSuccessFunc(data, status) {
                if (state == 1) {
                    count--;
                }
                $('#like-subject-' + articleId + '-info').html(count + " <a href='#' data-toggle='modal' data-target='#likes'>people</a> like this");
                $('#like-subject-' + articleId + ' img').attr("src", "/Content/images/like.png");
                $('#like-subject-' + articleId).attr("data-state", "0");
                $('#like-subject-' + articleId).attr("data-likesCount", parseInt(count));
            }

            function removeErrorFunc() {
                alert('error');
            }
        }
    }

    for (var i = 0; i < likeBtns.length; i++) {
        likeBtns[i].addEventListener('click', myFunc, false);
    }
});