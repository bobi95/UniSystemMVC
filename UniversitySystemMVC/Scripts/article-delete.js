$('#article-delete').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var articleId = button.data('article');

    var modal = $(this);
    document.querySelector('#form-delete-article .articleId').value = articleId;

    document.getElementById('delete-btn').onclick = function () {
        document.getElementById('form-delete-article').submit();
    }
})