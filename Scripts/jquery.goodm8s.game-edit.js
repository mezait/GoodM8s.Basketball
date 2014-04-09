$(function () {
    $('#game-edit').on('click', '#add-statistic', function (e) {
        e.preventDefault();

        $.ajax({
            url: this.href,
            success: function (html) {
                $('#statistics tbody').append(html);
            }
        });
    });

    $('#game-edit').on('click', 'a.remove-statistic', function (e) {
        e.preventDefault();

        $(this).parents('tr.statistic:first').remove();
    });
});