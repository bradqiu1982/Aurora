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
            alert($(this).attr('topicid'));
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