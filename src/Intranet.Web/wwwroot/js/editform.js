(function ($) {
    $('#print-button').click(function (e) {
        e.preventDefault();
        var route = $.getRoute();
        var url = $.urlAction('Print', route.controller, route.id);
        window.open(url, '_blank');
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
        var $this = $(this);
        var target = $this.attr("href");
        if (target === '#info') {
            if ($this.data('ajax-done'))
                return;

            $this.data('ajax-done', true);
            var route = $.getRoute();
            //if (route.action !== 'Edit')
            //    return;

            var url = $.urlAction('', 'Infos');
            $.get(url, { id: route.id, name: route.controller })
                .done(function (data) {
                    if (data)
                        $(target).html(data);
                })
                .fail(showAjaxError);
        }
        var ajaxUrl = $this.data('ajax-url');
        var done = $this.data('ajax-done');
        if (ajaxUrl && ajaxUrl.length > 0 && !done) {
            $.get(ajaxUrl).done(function (data) {
                $this.data('ajax-done', true);
                if (data)
                    $(target).html(data);
            }).fail(showAjaxError);
        }
    });


    $(document).on('click', '#restore-modal-button', function () {
        $('body').append('<div class="modal fade" id="restore-modal" tabindex="-1" role="dialog" aria-labelledby="restore-modal-label"><div class="modal-dialog" role="document"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title" id="restore-modal-label">Restore</h4></div><div class="modal-body"><p>Are you sure you want to restore the record (s)?</p></div><div class="modal-footer"><button type="button" class="btn btn-success" id="restore-button"><i class="fa fa-check" aria-hidden="true"></i> OK</button><button type="button" class="btn btn-default" data-dismiss="modal"><i class="fa fa-ban" aria-hidden="true"></i> Cancel</button></div></div></div></div>');
        $('#restore-modal').modal('show');
    });

    $(document).on('click', '#restore-button', function () {
        var route = $.getRoute();
        var url = $.urlAction('Restore', route.controller, route.id);
        $.post(url)
       .done(function () {
           $('#restore-modal').modal('hide');
           $('#restore-modal-button').remove();
       })
       .fail(showAjaxError);
    });

    $(document).on('hidden.bs.modal', '#restore-modal', function () {
        $(this).remove();
    });

}(jQuery));