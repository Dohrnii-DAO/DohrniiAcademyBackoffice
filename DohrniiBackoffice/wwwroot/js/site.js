$(document).ready(function () {
    $('.mtable').DataTable({
        "pageLength": 50
    });

    $(".stable").sortable({
        items: 'tr:not(tr:first-child)',
        cursor: 'pointer',
        axis: 'y',
        dropOnEmpty: false,
        start: function (e, ui) {
            ui.item.addClass("selected");
        },
        stop: function (e, ui) {
            ui.item.removeClass("selected");
        },
        update: function (e, ui) {
            $(".update-sequence").css("display", "block");
        }
    });
});
