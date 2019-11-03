// HTML ENCODE DECODE
// ==================
function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}


// MATH
// =====
function randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}


// AJAX
// ==========
var isLoading = false;
function showLoading(speed, autoHide) {
    hideLoading();
    isLoading = true;
    $('body').append('<div id="loading" class="progress"><div class="progress-bar progress-bar-danger" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width:0%"><span class="sr-only">Loading...</span></div></div>');

    var $loading = $('#loading .progress-bar');
    if (!speed || jQuery.type(speed) !== 'number') {
        $loading.animate({ width: '50%' }, randomInt(1500, 5000));
        if (isLoading) $loading.animate({ width: '75%' }, randomInt(2500, 5000));
        if (isLoading) $loading.animate({ width: '85%' }, randomInt(2500, 5000));
        if (isLoading) $loading.animate({ width: '95%' }, randomInt(2500, 5000));
        if (isLoading) $loading.animate({ width: '100%' }, randomInt(15000, 30000));
    } else {
        $('#loading .progress-bar').animate({ width: '100%' }, speed);
    }

    autoHide = autoHide || false;
    if (autoHide) {
        setTimeout(hideLoading, speed);
    }
}
function hideLoading() {
    isLoading = false;
    $('#loading').remove();
}
$(document).ajaxStart(function () {
    showLoading();
});
$(document).ajaxStop(function () {
    hideLoading();
});
function showAjaxError(xhr, status, error) {
    console.log('Error: ' + error);
    console.log('Status: ' + status);
    console.dir(xhr);

    if (xhr.status === 400 && xhr.responseText !== '') {
        var response;
        var errors = '';

        try {
            response = $.parseJSON(xhr.responseText);
            $.each(response, function (key, value) {
                errors += value[0] + '\n';
            });
        } catch (e) {
            errors = xhr.responseText;
        }

        if ($.isFunction($.notify)) {
            $.notify(errors, 'danger');
        } else {
            alert(errors);
        }
    } else {
        if ($.isFunction($.notify)) {
            $.notify(error, 'danger');
        } else {
            alert('Error: ' + error);
        }
    }
}





// AJAX MODAL
// ==========
function showAjaxModal(option) {
    if (!option) {
        $('#ajax-modal').modal('show');
    }
    else {
        $('#ajax-modal').modal(option);
    }
}
function hideAjaxModal() {
    $('#ajax-modal').modal('hide');
}
$(document).on('shown.bs.modal', function () {
    $(this).find('[autofocus]').focus();
});
$(document).on('hidden.bs.modal', '#ajax-modal', function () {
    $(this).remove();
});



(function ($) {
    $.fn.equalHeight = function () {
        var tallest = 0;
        this.each(function () {
            var thisHeight = $(this).height();
            if (thisHeight > tallest) {
                tallest = thisHeight;
            }
        });
        console.log(tallest);
        return this.each(function () {
            console.log($(this));
             $(this).height(tallest);
        });
    };

}(jQuery));


// MVC ROUTE
// =========
(function ($) {
    var route;

    $.getRoute = function () {
        if (route) return route;
        var path = window.location.pathname.split('/');
        route = {
            controller: path[1],
            action: path.length >= 2 ? path[2] : 'Index',
            id: path.length >= 3 ? path[3] : null
        };
        return route;
    };

    $.urlAction = function (action, controller, values) {
        if (!action) {
            action = '';
        }
        if (!controller || controller.length === 0) {
            controller = $.getRoute().controller;
        }

        if (!values)
            values = '';
        else if (values.charAt(0) !== '/')
            values = '/' + values;

        return '/' + controller + '/' + action + (values.length > 0 ? values : '');
    };
}(jQuery));




// NOTIFY
// ======
(function ($) {
    var id = 0;

    $.notify = function (content, type) {
        if (!content)
            content = '';

        //Close previous alert
        $('#notify-' + id).alert('close');
        id++;

        var icon;
        switch (type) {
            case 'success':
                icon = 'check';
                break;
            case 'warning':
                icon = 'exclamation-triangle';
                break;
            case 'danger':
                icon = 'exclamation-circle';
                break;
            case 'error':
                type = 'danger';
                icon = 'exclamation-circle';
                break;
            case 'info':
                icon = 'info-circle';
                break;
            default:
                type = 'info';
                icon = 'info-circle';
                break;
        }

        $('body').append('<div id="notify-' + id + '" class="alert alert-' + type + ' alert-dismissible fade in col-xs-11 col-sm-3" style="position:fixed;right:10px;top:10px;z-index:999999;" role="alert"><button type="button" class ="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><span class ="fa fa-' + icon + '" aria-hidden="true"></span> ' + content + '</div>');
        $('#notify-' + id).delay(5000).fadeOut(1000, function () {
            $(this).alert('close');
        });
    };
}(jQuery));





// SIDEBAR SEARCH
// ==============
(function ($) {
    var navs;

    $('#sidebar-nav').find('a.active').each(function (i, a) {
        var parent = $(a).parent();
        parent.addClass('in');
        parent.prev().find('i').removeClass('fa-caret-right').addClass('fa-caret-down');
    });

    $('#sidebar-nav').find('a[data-toggle="collapse"]').click(function () {
        $(this).find('i').toggleClass('fa-caret-right').toggleClass('fa-caret-down');
    });

    $('#sidebar-search-textbox').keyup(function () {
        var filter = event.target.value.toUpperCase();
        if (filter !== '') {
            var i;
            if (!navs) {
                navs = new Array();
                var anchors = $('#sidebar-nav').find('a');//.find('a:not([href=""], [href^="#"])');

                for (i = 0; i < anchors.length; i++) {
                    var a = $(anchors[i]);
                    var text = a.text().trim();
                    var url = a.attr('href');
                    if (text === '' || url === '' || url.charAt(0) === '#')
                        continue;

                    var icon = $('<div>').append(a.find('i').clone()).html();
                    //var iconElement = a.find('i');
                    //var icon = iconElement.clone().wrap('<div>').parent().html();
                    //iconElement.unwrap();
                    navs.push({ text, url, icon });
                }
            }

            var html = '<ul class="nav nav-sidebar">';
            for (i = 0; i < navs.length; i++) {
                if (navs[i].text.toUpperCase().indexOf(filter) > -1) {
                    html += '<li role="presentation"><a href="' + navs[i].url + '">' + navs[i].icon + ' ' + navs[i].text + '</a></li>';
                }
            }
            html += '</ul>';

            $('#sidebar-nav').hide();
            $('#sidebar-search-nav').html(html);
        } else {
            $('#sidebar-search-nav').html('');
            $('#sidebar-nav').show();
        }
    });
}(jQuery));





// BUTTON-BROWSE
// =============
(function ($) {
    function showModal($buttonBrowseDiv) {
        var url = $buttonBrowseDiv.data('modal-ajax-url');
        if (!url) {
            alert('data-modal-ajax-url is undefined');
            return;
        }

        $.get(url, function (data) {
            $('body').append(data);
            $('#bb-modal').modal('show');
            var $form = $('#bb-form');
            //$form.data('name', $buttonBrowseDiv.data('name'));
            $form.data('id', $buttonBrowseDiv.attr('id'));
        }).fail(showAjaxError);
    }

    //Uncomment this if you want to show modal after double click on textbox
    //$(document).on('dblclick', 'input.bb-modal', function (e) {
    //    e.preventDefault();
    //    var div = $(this).closest('div.bb');
    //    showModal(div);
    //});

    //loads modal html at the end of html documents and shows
    $(document).on('click', 'button.bb-modal', function (e) {
        if (e) e.preventDefault();

        var $buttonBrowseDiv = $(this).closest('div.bb');
        if ($buttonBrowseDiv.data('readonly') === 'readonly')
            return;
        showModal($buttonBrowseDiv);
    });


    //submit form after button-browse modal window is shown
    $(document).on('shown.bs.modal', '#bb-modal', function () {

        var form = $(this).find('#bb-form');

        ajaxForm(form.attr('action'), 'get', form.attr('data-parameters'), form);
        //ajaxForm(form.attr('action'), 'get', form.serialize(), form);
    });


    //removes window tag after modal is closed
    $(document).on('hidden.bs.modal', '#bb-modal', function () {
        $(this).remove();
    });


    function del($txt) {
        var readonly = $txt.data('readonly');
        if (readonly)
            return;

        $txt.val('');
        $txt.attr('title', '');

        var inputId = $txt.attr('id');
        var hiddenTarget = '#' + inputId.slice(0, -4) + 'Id';
        $(hiddenTarget).val('').trigger('change');
    }

    //delete & ctrl + enter key
    $(document).on('keyup', '.bb input', function (e) {
        if (e.which === 46) {
            e.preventDefault();

            var $buttonBrowseDiv = $(this).closest('div.bb');
            if ($buttonBrowseDiv.data('readonly') === 'readonly')
                return;
            del($(this));
        }
        //else if (e.ctrlKey && e.keyCode == 13) {
        //    e.preventDefault();

        //}
    });
    $(document).on('click', 'a.bb-delete', function (e) {
        if (e) e.preventDefault();

        var $buttonBrowseDiv = $(this).closest('div.bb');
        if ($buttonBrowseDiv.data('readonly') === 'readonly')
            return;
        var txtTarget = '#' + $buttonBrowseDiv.attr('id') + '_Name';
        //var txtTarget = '#' + $buttonBrowseDiv.data('name') + '_Name';
        del($(txtTarget));
    });

    //filter button click event
    $(document).on('click', '#bb-filter-button', function (e) {
        if (e) e.preventDefault();

        var form = $('#bb-form');
        ajaxForm(form.attr('action'), form.attr('method'), form.serialize(), form.find('#bb-grid-container'));
    });


    //ajax form request & update
    function ajaxForm(url, type, data, target) {
        $.ajax({
            url: url,
            type: type,
            data: data
        }).done(function (data) {
            target.html(data);
        }).fail(showAjaxError);
    }


    //handles enter key up and executes ajax form
    $(document).on('keyup', '#bb-form input', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            var form = $('#bb-form');
            ajaxForm(form.attr('action'), form.attr('method'), form.serialize(), form.find('#bb-grid-container'));
        }
    });


    //pagination
    $(document).on('click', '#bb-paged-list a', function (e) {
        if (e) e.preventDefault();

        var page = $(this).attr("href").substr(1);
        var form = $('#bb-form');
        ajaxForm(form.attr('action') + '?page=' + page, form.attr('method'), form.serialize(), form.find('#bb-grid-container'));
    });


    //grid doubble click event
    $(document).on('dblclick', '#bb-grid tr', function () {
        var newValue = $(this).data('id');
        var $form = $('#bb-form');

        //var id = $form.data('name');
        var id = $form.data('id');
        var $hiddenElement = $('#' + id + '_Id');
        if ($hiddenElement.val() !== newValue)
            $hiddenElement.val(newValue).trigger('change');
        $('#bb-modal').modal('hide');
    });

    $(document).on('change', '.bb input:hidden', function () {
        var $this = $(this);
        var value = $this.val();
        var $bb = $this.closest('div.bb');
        //var id = $bb.data('name');
        var id = $bb.attr('id');

        var $textbox = $('#' + id + '_Name');
        if (value === '') {
            $textbox.attr('title', 'ID = ' + value);
            return;
        }
        $textbox.attr('title', 'ID = ' + value);
        var url = $bb.data('text-ajax-url');
        $.get(url + '/' + value)
            .done(function (data) {
                $textbox.val(data).trigger('change');;
            })
            .fail(function (xhr, status, error) {
                $textbox.val('ID = ' + value);
                showAjaxError(xhr, status, error);
            });
    });

}(jQuery));




// CASCADE DROP DOWN
// =================
(function ($) {
    $.fn.cascadeDropdown = function () {
        $(this).change(dropDown);
        return this;
    };
    $.fn.cascadeDropdown.defaults = {
        optionHtml: ''
    };

    function dropDown() {
        var $this = $(this);
        var selector = $this.data('target');
        if (!selector) {
            alert('data-target attribute is not defined');
            return;
        }

        var action = $this.data('url') || $this.attr("action");
        if (!action) {
            alert('data-action attribute is not defined');
            return;
        }


        var $target = $(selector);
        $target.val(null);
        $target.prop('disabled', true);
        $.getJSON({
            url: action + '/' + $this.val()
        })
            .done(function (data) {
                var html = '<option>' + $.fn.cascadeDropdown.defaults.optionHtml + '</option>';

                var values = [];
                var tmpdata = {};
                $.each(data, function (key, value) {
                    values.push(value);
                    tmpdata[value] = key;
                });
                values.sort();
                //var sortedValues = values.sort();

                $.each(values, function (index, value) {
                    html += '<option value="' + tmpdata[value] + '">' + value + '</option>';
                });
                //$.each(sortedValues, function (index, value) {
                //    sorted[tmpdata[value]] = value;
                //});

                //$.each(tmpdata, function (val, key) {
                //    html += '<option value="' + key + '">' + val + '</option>';
                //});
                $target.html(html);
            })
            .fail(showAjaxError)
            .always(function () {
                $target.prop('disabled', false);
            });
    }
}(jQuery));




// AJAX
// ====
(function ($) {
    $.ajaxLink = function (selector, target, mode) {
        mode = (mode || '').toUpperCase();
        $(document).on('click', selector, function (e) {
            e.preventDefault();
            $.get(this.href, function (data) {
                $(target).each(function (i, update) {
                    var top;
                    switch (mode) {
                        case "BEFORE":
                            top = update.firstChild;
                            $("<div />").html(data).contents().each(function () {
                                update.insertBefore(this, top);
                            });
                            break;
                        case "AFTER":
                            $("<div />").html(data).contents().each(function () {
                                update.appendChild(this);
                            });
                            break;
                        case "REPLACE-WITH":
                            $(update).replaceWith(data);
                            break;
                        default:
                            $(update).html(data);
                            break;
                    }
                });
            }).fail(showAjaxError);
        });
    }


    $.fn.downloadForm = function (url) {
        var $this = $(this);
        url = url || $this.attr('action');
        var $form = $('<form></form>').attr('action', url).attr('method', 'post').attr('target', '_blank');
        $.each($this.serializeArray(), function (key, value) {
            console.log(value.name);
            console.log(value.value);

            $form.append($('<input></input>').attr('type', 'hidden').attr('name', value.name).attr('value', value.value));
        });
        $form.appendTo('body').submit().remove();
    };

}(jQuery));