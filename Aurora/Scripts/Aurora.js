var Aurora = function () {
    var createpage = function () {
        $('body').on('click', '.maineditor-time', function () {
            $('#mainduedatemodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.maineditor-people', function () {
            $('#relatedmodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.maineditor-proj', function () {
            $('#pjmodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.logo', function () {
            window.location.href = '/CoWork/Home';
        })
    };

    var mainpage = function () {

        $('body').on('click', '.topiclistcls', function () {
            var topicid = $(this).attr('topicid');
            $.post('/CoWork/TopicByID',
                { topicid: topicid }
                , function (output) {
                    if (output.sucess)
                    {
                        $('.itemdetailinfo').empty();
                        $('.itemdetailinfo').append(output.data.topiccontent);
                        $('.itemdetailinfo').removeClass("itemdetailhide");
                    }

                })
        })

        $('body').on('click', '.nav-sidebar-icon', function () {
            var ndhide = $('.item-panels').hasClass('hide');

            if ($(this).children().eq(0).hasClass('sidebar-show')) {
                $('.nav-sidebar').addClass('nav-sidebar-narrow');
                $(this).children().eq(0).removeClass('sidebar-show').addClass('sidebar-hide');
                $('.item-show').addClass('hidden');

                if (ndhide) {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-34');
                }
                else {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-9');
                }
            }
            else {
                $('.nav-sidebar').removeClass('nav-sidebar-narrow');
                $(this).children().eq(0).removeClass('sidebar-hide').addClass('sidebar-show');
                $('.item-show').removeClass('hidden');

                if (ndhide) {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-25');
                }
                else {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7');
                }
            }
        })

        
        $('body').on('click', '.itemlistswitchclose', function () {
            var sthide = $('.nav-sidebar').hasClass('nav-sidebar-narrow');

            $('.item-panels').addClass('hide');
            $('.itemlistswitchopen').removeClass('hide');

            if (sthide) {

                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-34');
            }
            else {
                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-25');
            }
        })

        $('body').on('click', '.itemlistswitchopen', function () {
            var sthide = $('.nav-sidebar').hasClass('nav-sidebar-narrow');

            $('.item-panels').removeClass('hide');
            $('.itemlistswitchopen').addClass('hide');

            if (sthide) {
                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-9');
            }
            else {
                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7');
            }
        })
    };

    return {
        createpageinit: function ()
        {
            createpage();
        },
        mainpageinit:function()
        {
            mainpage();
        }
    };
}();