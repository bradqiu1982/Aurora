var Aurora = function () {
    var common = function () {
        $('body').on('click', '.maineditor-time', function () {
            $('#mainduedatemodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.maineditor-people', function () {
            $('#relatedmodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.maineditor-proj', function () {
            $('#pjmodal').modal({ backdrop: 'static' });
        })
    }

    return {
        init: function ()
        {
            common();
        }
    };
}();