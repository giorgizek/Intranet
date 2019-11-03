(function ($) {
    //var autofocus = false;
    //function changeUrl(page, url) {
    //    if (typeof (history.pushState) != 'undefined') {
    //        var obj = { Page: page, Url: url };
    //        history.pushState(null, obj.Page, obj.Url);
    //    } else {
    //        alert('Browser does not support HTML5.');
    //    }
    //}

    //function getUrlVars(url) {
    //    var vars = [], hash;
    //    if (typeof url === 'undefined' || url === null) {
    //        url = window.location.href;
    //    }
    //    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    //    for (var i = 0; i < hashes.length; i++) {
    //        hash = hashes[i].split('=');
    //        vars.push(hash[0]);
    //        vars[hash[0]] = hash[1];
    //    }
    //    return vars;
    //}

    function refreshGrid($form) {
        if (!$form)
            $form = $('#filter-form');

        var targetSelector = $form.data('ajax-update') || '#grid-container';
        //var $target = $(targetSelector).prepend('<div class="progress" id="progress" style="margin:15px" ><div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:100%">Loading...</div></div>');
        $.ajax({
            url: $form.data("ajax-url") || $form.attr("action"),
            type: $form.data("ajax-method") || $form.attr("method") || "GET",
            data: $form.serialize()
        }).done(function (data) {
            //changeUrl('index', url);
            $(targetSelector).html(data);
            //$target.html(data);
        }).fail(showAjaxError);
    }

    $.refreshGrid = refreshGrid;

    function getPage(e) {
        if (e) {
            e.preventDefault();
        }
        //var page = getUrlVars($(this).attr('href'))['page'];
        var page = $(this).attr("href").substr(1);//removes # symbol
        $('#Page').val(page);
        refreshGrid();
    }

    function filterForm(e) {
        if (e) {
            e.preventDefault();
        }
        $('#filter-modal').modal('hide');
        $('#Page').val('1');
        refreshGrid();
    }

    $.fn.autoFilter = function () {
        $(this).change(filterForm);
        //$(this).keydown(function (e) {
        //    if (e.which === 13) {
        //        e.preventDefault();
        //        filterForm();
        //    }
        //});
        return this;
    };

    $('#export-excel-button').click(function() {
        $('#filter-form').downloadForm($.urlAction('Export'));
        //var url = $form.data("ajax-url") || $form.attr("action");
        //var data = $form.serialize();

        //$.download()
    });

    $('#filter-button').click(filterForm);

    $('#refresh-button').click(function () { refreshGrid(); });

    if ($('#filter-panel-button').length) {
        $(document).keydown(function (e) {
            if (e.keyCode === 191 && e.ctrlKey) {//ctrl + /
                $('#filter-panel-button').click();
            }
        });
    }
    //$('#filter-modal').on('shown.bs.modal', function () {
    //    //if (autofocus)
    //    //    $(this).find('[autofocus]').focus();
    //    //else
    //    $(this).find('input,textarea,select').filter(':visible:first').focus();
    //});
    $('#filter-form').submit(filterForm);
    $('#filter-form').on("keypress", function (e) {
        if (e.which === 13)
            $(this).submit();
    });
    $(document).on('click', '.pagination a', getPage);
    $(document).on('change', '#PageSize', filterForm);




    $.showDeleteModal = function() {
        $('#ajax-modal').modal('show');
    }
    $.onDeleteAjaxSuccess = function (data) {
        //if (!data || data.legth === 0) {
            $('#ajax-modal').modal('hide');
            refreshGrid();
        //}
    }





    $('#delete-modal').on('show.bs.modal', function (e) {
        var $button = $(e.relatedTarget);
        var url = $button.data('ajax-url');
        //var id = $button.parents('tr').data('id');

        var $deleteButton = $('#delete-modal-ok-button');
        $deleteButton.data('ajax-url', url);
        //$deleteButton.data('id', id);

        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        //var modal = $(this);
        //modal.find('.modal-title').text('New message to ' + recipient);
        //modal.find('.modal-body input').val(recipient);
    });

    //Delete button is in modal form
    $('#delete-modal-ok-button').click(function () {
        var $this = $(this);
        var url = $this.data('ajax-url');

        $.post(url)
        .done(function () {
            $('#delete-modal').modal('hide');
            //var rowId = $this.data('id');
            //$('#row-' + rowId).fadeOut('fast', function () {
            //    $(this).remove();
            //});
            refreshGrid();
        })
        .fail(showAjaxError);
    });



    $('#approve-modal').on('show.bs.modal', function (e) {
        var $button = $(e.relatedTarget);
        var url = $button.data('ajax-url');
        //var id = $button.parents('tr').data('id');

        var $approveButton = $('#approve-modal-ok-button');
        $approveButton.data('ajax-url', url);
        //$approveButton.data('id', id);

        //$('#approveHidden').val(id);
    });

    //Approve button is in modal form
    $('#approve-modal-ok-button').click(function () {
        var $this = $(this);
        var url = $this.data('ajax-url');

        $.post(url)
        .done(function (data) {
            $('#approve-modal').modal('hide');

            refreshGrid();
            //if (data.error == '') {
            //    var $approveButton = $('tr[data-id="' + $('#approveHidden').val() + '"] .btn[data-target=\'#approve-modal\']');
            //    $approveButton.toggleClass('btn-success', !data.result).toggleClass('btn-warning', data.result);
            //    $approveButton.find('i').toggleClass('fa-check', !data.result).toggleClass('fa-close', data.result);
            //    $approveButton.parent().find("button[data-target='#deleteModal']").prop('disabled', function (i, v) { return !v; });

            //    var status = $("tr[data-id='" + $('#approveHidden').val() + "'] td[data-column='status']");
            //    status.text(data.status);
            //}
        })
        .fail(showAjaxError);
    });

}(jQuery));